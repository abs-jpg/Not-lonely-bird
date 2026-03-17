using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Direction（方向判断）核心游戏管理器
/// 箭头出现在左/右侧，玩家根据箭头指向举起对应手掌
/// </summary>
public class DirectionManager : MonoBehaviour
{
    private enum GameState { Ready, Playing, RoundOver, GameOver }

    [Header("UI 引用")]
    public GameObject startButton;
    public TextMeshProUGUI countdownText;

    [Header("箭头图标")]
    public Image cueLeftLeft;    // 左侧-指左
    public Image cueLeftRight;   // 左侧-指右
    public Image cueRightLeft;   // 右侧-指左
    public Image cueRightRight;  // 右侧-指右

    [Header("音频")]
    public AudioSource audioSource;
    public AudioClip gameStartSound;
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    [Header("游戏参数")]
    public float gameDuration = 10f;
    public int gameRound = 40;
    public bool isRandomMode = false;
    public float feedbackDelay = 2f;

    [Header("结算")]
    public RVPSettlementScreen settings;

    // --- 运行时状态 ---
    private GameState _state = GameState.Ready;
    private int _trialsCompleted;
    private int _score;
    private float _roundTimer;
    private bool _correctAnswerIsLeft; // true=左手, false=右手

    // Rokid SDK 反射
    private bool _rokidAvailable;
    private System.Reflection.MethodInfo _getLeftHandInfo;
    private System.Reflection.MethodInfo _getRightHandInfo;
    private System.Reflection.FieldInfo _gestureTypeField;

    private void Awake()
    {
        TryInitRokidSDK();
    }

    private void Start()
    {
        // 从全局单例读取设置
        if (AllSettingCtr.Instance != null)
        {
            gameDuration = AllSettingCtr.Instance.directionGameDuration;
            isRandomMode = AllSettingCtr.Instance.directionIsRandomMode;
            gameRound = Mathf.RoundToInt(AllSettingCtr.Instance.directionGameRounds);
        }

        HideAllCues();
        EnterReady();
    }

    private void Update()
    {
        if (_state != GameState.Playing) return;

        // 倒计时
        _roundTimer -= Time.deltaTime;
        if (countdownText != null)
        {
            countdownText.color = Color.white;
            countdownText.text = Mathf.CeilToInt(_roundTimer).ToString();
        }

        if (_roundTimer <= 0f)
        {
            // 时间到，没做手势 → 漏掉
            ProcessMissed();
            return;
        }

        // 检测手势
        DetectGesture();
    }

    // ========== 状态流转 ==========

    private void EnterReady()
    {
        _state = GameState.Ready;
        if (startButton != null) startButton.SetActive(true);
        if (countdownText != null)
        {
            countdownText.color = Color.white;
            countdownText.text = "准备好了吗？\n开始之后保持握拳\n直到题目出现";
        }
    }

    /// <summary>
    /// 开始按钮回调（Inspector 或代码绑定）
    /// </summary>
    public void OnStartButtonPressed()
    {
        if (_state != GameState.Ready) return;

        _trialsCompleted = 0;
        _score = 0;
        if (startButton != null) startButton.SetActive(false);

        if (audioSource != null && gameStartSound != null)
        {
            audioSource.clip = gameStartSound;
            audioSource.Play();
        }

        _state = GameState.Playing;
        StartNewRound();
    }

    private void StartNewRound()
    {
        if (_trialsCompleted >= gameRound)
        {
            EnterGameOver();
            return;
        }

        HideAllCues();
        _roundTimer = gameDuration;

        // 决定题目
        bool showOnLeft = UnityEngine.Random.value < 0.5f;
        bool arrowPointsLeft;

        if (!isRandomMode || _trialsCompleted < 3)
        {
            // 一致性：箭头方向 = 出现位置
            arrowPointsLeft = showOnLeft;
        }
        else
        {
            // 随机：箭头方向与位置无关
            arrowPointsLeft = UnityEngine.Random.value < 0.5f;
        }

        // 正确答案 = 箭头指向的方向
        _correctAnswerIsLeft = arrowPointsLeft;

        // 激活对应图标
        if (showOnLeft && arrowPointsLeft && cueLeftLeft != null)
            cueLeftLeft.gameObject.SetActive(true);
        else if (showOnLeft && !arrowPointsLeft && cueLeftRight != null)
            cueLeftRight.gameObject.SetActive(true);
        else if (!showOnLeft && arrowPointsLeft && cueRightLeft != null)
            cueRightLeft.gameObject.SetActive(true);
        else if (!showOnLeft && !arrowPointsLeft && cueRightRight != null)
            cueRightRight.gameObject.SetActive(true);

        if (countdownText != null)
            countdownText.text = Mathf.CeilToInt(_roundTimer).ToString();
    }

    private void ProcessAnswer(bool isCorrect)
    {
        _state = GameState.RoundOver;
        _trialsCompleted++;

        if (isCorrect)
        {
            _score++;
            SetHint("正确", Color.green);
            if (audioSource != null && correctSound != null)
            {
                audioSource.clip = correctSound;
                audioSource.Play();
            }
        }
        else
        {
            SetHint("错误", Color.red);
            if (audioSource != null && incorrectSound != null)
            {
                audioSource.clip = incorrectSound;
                audioSource.Play();
            }
        }

        StartCoroutine(WaitAndNextRound());
    }

    private void ProcessMissed()
    {
        _state = GameState.RoundOver;
        _trialsCompleted++;
        SetHint("漏掉", Color.yellow);

        StartCoroutine(WaitAndNextRound());
    }

    private void SetHint(string text, Color color)
    {
        if (countdownText == null) return;
        countdownText.text = text;
        countdownText.color = color;
    }

    private IEnumerator WaitAndNextRound()
    {
        yield return new WaitForSeconds(feedbackDelay);
        _state = GameState.Playing;
        StartNewRound();
    }

    private void EnterGameOver()
    {
        _state = GameState.GameOver;
        HideAllCues();
        if (startButton != null) startButton.SetActive(true);

        float correctRate = _trialsCompleted > 0
            ? (float)_score / _trialsCompleted * 100f
            : 0f;

        // 读取历史最佳
        float bestScore = 0f;
        if (settings != null)
            bestScore = settings.LoadBestScore("Direction");

        if (countdownText != null)
        {
            countdownText.color = Color.white;
            countdownText.text = $"测试结束！\n总回合: {_trialsCompleted}\n" +
                                 $"正确率: {correctRate:F1}%\n最佳记录: {bestScore:F1}%";
        }

        // 保存（仅当高于历史记录）
        if (settings != null)
            settings.SaveScore("Direction", correctRate);
    }

    // ========== 手势检测 ==========

    private void DetectGesture()
    {
        bool leftPalm = false;
        bool rightPalm = false;

        if (_rokidAvailable)
        {
            try
            {
                var leftHand = _getLeftHandInfo.Invoke(null, null);
                if (leftHand != null)
                {
                    int t = (int)_gestureTypeField.GetValue(leftHand);
                    if (t == 3) leftPalm = true; // PalmForward
                }

                var rightHand = _getRightHandInfo.Invoke(null, null);
                if (rightHand != null)
                {
                    int t = (int)_gestureTypeField.GetValue(rightHand);
                    if (t == 3) rightPalm = true; // PalmForward
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Direction] 手势检测异常: {e.Message}");
            }
        }
        else
        {
            // 键盘模拟: A=左手, D=右手
            leftPalm = Input.GetKeyDown(KeyCode.A);
            rightPalm = Input.GetKeyDown(KeyCode.D);
        }

        // 双手都张开 → 忽略
        if (leftPalm && rightPalm) return;

        if (leftPalm)
            ProcessAnswer(_correctAnswerIsLeft);
        else if (rightPalm)
            ProcessAnswer(!_correctAnswerIsLeft);
    }

    private void HideAllCues()
    {
        if (cueLeftLeft != null) cueLeftLeft.gameObject.SetActive(false);
        if (cueLeftRight != null) cueLeftRight.gameObject.SetActive(false);
        if (cueRightLeft != null) cueRightLeft.gameObject.SetActive(false);
        if (cueRightRight != null) cueRightRight.gameObject.SetActive(false);
    }

    // ========== Rokid SDK 反射初始化 ==========

    private void TryInitRokidSDK()
    {
        try
        {
            var gesApiType = Type.GetType("Rokid.UXR.Interaction.GesApi, Assembly-CSharp")
                          ?? Type.GetType("Rokid.UXR.Interaction.GesApi, Rokid.UXR.Interaction");

            if (gesApiType != null)
            {
                var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static;
                _getLeftHandInfo = gesApiType.GetMethod("GetLeftHandInfo", flags);
                _getRightHandInfo = gesApiType.GetMethod("GetRightHandInfo", flags);

                if (_getLeftHandInfo != null && _getRightHandInfo != null)
                {
                    _gestureTypeField = _getLeftHandInfo.ReturnType.GetField("gesture_type");
                    if (_gestureTypeField != null)
                    {
                        _rokidAvailable = true;
                        return;
                    }
                }
            }
        }
        catch (Exception) { }

        _rokidAvailable = false;
    }
}
