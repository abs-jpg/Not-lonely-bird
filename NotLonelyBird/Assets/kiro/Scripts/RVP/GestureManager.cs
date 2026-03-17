using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// RVP (快速视觉信息处理) 核心游戏管理器
/// 三阶段流程：指令 → 测试 → 结果
/// 随机呈现手势图片，玩家在目标手势出现时模仿对应手势
/// </summary>
public class GestureManager : MonoBehaviour
{
    [Header("测试参数")]
    public float testDuration = 60f;
    public int gesturesPerMinute = 30;
    public int targetCount = 3;

    [Header("手势库")]
    public GestureData[] allGestures;

    [Header("显示参数")]
    public float flashDuration = 0.1f;
    [Tooltip("反馈文字停留时间")]
    public float feedbackDuration = 0.8f;

    [Header("Prefab")]
    public GameObject rvpPrefabs;

    [Header("组件引用")]
    public RVPSettlementScreen settings;
    public RectTransform contentPanel;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI tfHint;
    public Image stimulusImage;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI resultText;

    [Header("面板引用")]
    public GameObject instructionPanel;
    public GameObject gesturePanel;
    public GameObject resultPanel;

    // --- 运行时状态 ---
    private GestureInputController _inputController;
    private List<GestureData> _targetGestures = new List<GestureData>();
    private List<GestureData> _sequence = new List<GestureData>();
    private float _stimulusInterval;
    private int _totalStimuli;

    // 计分
    private int _hits;
    private int _misses;
    private int _falseAlarms;
    private int _correctRejections;
    private List<float> _reactionTimes = new List<float>();

    // 当前试次的手势输入记录
    private bool _respondedThisTrial;
    private bool _currentIsTarget;
    private float _currentStimulusTime;
    private GestureData.Hand _respondedHand;
    private GestureData.CustomGestureType _respondedType;
    private float _respondedRT;
    private bool _testRunning;

    private void Awake()
    {
        _inputController = GetComponent<GestureInputController>();

        if (AllSettingCtr.Instance != null)
        {
            gesturesPerMinute = AllSettingCtr.Instance.attentionGesturesPerMinute;
            targetCount = AllSettingCtr.Instance.attentionTargetCount;
            flashDuration = AllSettingCtr.Instance.attentionFlashDuration;
        }

        _stimulusInterval = 60f / gesturesPerMinute;
        _totalStimuli = Mathf.RoundToInt(testDuration / _stimulusInterval);
    }

    private void Start()
    {
        GestureInputController.OnGesturePerformed += OnGestureDetected;
        StartCoroutine(RunGame());
    }

    private void OnDestroy()
    {
        GestureInputController.OnGesturePerformed -= OnGestureDetected;
    }

    private IEnumerator RunGame()
    {
        // === 阶段1：指令 ===
        ShowPanel(instructionPanel);
        PickTargetGestures();
        ShowInstructions();

        for (int i = 5; i > 0; i--)
        {
            if (timeText != null)
                timeText.text = $"{i}秒后开始...";
            yield return new WaitForSeconds(1f);
        }

        // === 阶段2：测试 ===
        ShowPanel(gesturePanel);
        if (_inputController != null)
            _inputController.SetDetectionEnabled(true);

        GenerateSequence();
        _testRunning = true;

        for (int i = 0; i < _sequence.Count; i++)
        {
            GestureData current = _sequence[i];
            _currentIsTarget = _targetGestures.Contains(current);
            _respondedThisTrial = false;
            _currentStimulusTime = Time.time;

            // 清空反馈
            SetHint("", Color.white);

            // 显示手势图片
            if (stimulusImage != null)
            {
                stimulusImage.sprite = current.gestureImage;
                stimulusImage.gameObject.SetActive(true);
            }

            // 等待刺激间隔（玩家在此期间做手势）
            yield return new WaitForSeconds(_stimulusInterval);

            // 隐藏图片
            if (stimulusImage != null)
                stimulusImage.gameObject.SetActive(false);

            // === 统一判定并显示反馈 ===
            if (_respondedThisTrial)
            {
                // 玩家做了手势，判断对错
                bool matchedTarget = false;
                foreach (var target in _targetGestures)
                {
                    if (target.hand == _respondedHand && target.gestureType == _respondedType)
                    {
                        matchedTarget = true;
                        break;
                    }
                }

                if (_currentIsTarget && matchedTarget)
                {
                    _hits++;
                    _reactionTimes.Add(_respondedRT);
                    SetHint("正确", Color.green);
                }
                else if (!_currentIsTarget)
                {
                    _falseAlarms++;
                    SetHint("错误", Color.red);
                }
                else
                {
                    _misses++;
                    SetHint("错误", Color.red);
                }
            }
            else
            {
                // 玩家没做手势
                if (_currentIsTarget)
                {
                    _misses++;
                    SetHint("漏掉", Color.yellow);
                }
                else
                {
                    _correctRejections++;
                }
            }

            // 停留让玩家看到反馈
            yield return new WaitForSeconds(feedbackDuration);
        }

        _testRunning = false;
        if (_inputController != null)
            _inputController.SetDetectionEnabled(false);

        // === 阶段3：结果 ===
        ShowPanel(resultPanel);
        ShowResults();
    }

    /// <summary>
    /// 手势检测回调 — 只记录输入，不做判定
    /// </summary>
    private void OnGestureDetected(GestureData.Hand hand, GestureData.CustomGestureType gestureType)
    {
        if (!_testRunning || _respondedThisTrial) return;

        _respondedThisTrial = true;
        _respondedHand = hand;
        _respondedType = gestureType;
        _respondedRT = Time.time - _currentStimulusTime;
    }

    private void SetHint(string text, Color color)
    {
        if (tfHint == null) return;
        tfHint.text = text;
        tfHint.color = color;
    }

    private void PickTargetGestures()
    {
        _targetGestures.Clear();
        if (allGestures == null || allGestures.Length == 0) return;

        List<GestureData> pool = new List<GestureData>(allGestures);
        int count = Mathf.Min(targetCount, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, pool.Count);
            _targetGestures.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
    }

    private void GenerateSequence()
    {
        _sequence.Clear();
        if (allGestures == null || allGestures.Length == 0) return;

        List<GestureData> nonTargets = new List<GestureData>();
        foreach (var g in allGestures)
        {
            if (!_targetGestures.Contains(g))
                nonTargets.Add(g);
        }

        for (int i = 0; i < _totalStimuli; i++)
        {
            bool isTarget = Random.value < 0.4f;

            if (isTarget && _targetGestures.Count > 0)
                _sequence.Add(_targetGestures[Random.Range(0, _targetGestures.Count)]);
            else if (nonTargets.Count > 0)
                _sequence.Add(nonTargets[Random.Range(0, nonTargets.Count)]);
            else
                _sequence.Add(allGestures[Random.Range(0, allGestures.Length)]);
        }
    }

    private void ShowInstructions()
    {
        if (instructionText == null) return;

        instructionText.text = "以下图片出现时\n模仿手势:";

        if (contentPanel != null)
        {
            for (int i = contentPanel.childCount - 1; i >= 0; i--)
                Destroy(contentPanel.GetChild(i).gameObject);

            foreach (var target in _targetGestures)
            {
                if (target.gestureImage == null) continue;

                GameObject imgObj = new GameObject(target.gestureName);
                Image img = imgObj.AddComponent<Image>();
                img.sprite = target.gestureImage;
                img.preserveAspect = true;
                imgObj.transform.SetParent(contentPanel, false);

                RectTransform rt = imgObj.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(150, 150);
            }
        }
    }

    private void ShowResults()
    {
        if (resultText == null) return;

        int totalTargets = _hits + _misses;
        float correctRate = totalTargets > 0 ? (float)_hits / totalTargets * 100f : 0f;
        float avgRT = 0f;
        if (_reactionTimes.Count > 0)
        {
            float sum = 0f;
            foreach (float t in _reactionTimes) sum += t;
            avgRT = sum / _reactionTimes.Count;
        }

        resultText.text = $"测试结束!\n\n" +
                          $"正确率: {correctRate:F1}%\n" +
                          $"平均反应时间: {avgRT:F2}秒\n" +
                          $"命中: {_hits}  漏报: {_misses}\n" +
                          $"虚报: {_falseAlarms}  正确拒绝: {_correctRejections}";
    }

    private void ShowPanel(GameObject panel)
    {
        if (instructionPanel != null) instructionPanel.SetActive(panel == instructionPanel);
        if (gesturePanel != null) gesturePanel.SetActive(panel == gesturePanel);
        if (resultPanel != null) resultPanel.SetActive(panel == resultPanel);
    }
}
