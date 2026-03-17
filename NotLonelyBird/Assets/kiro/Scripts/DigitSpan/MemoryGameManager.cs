using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// DigitSpan 核心游戏管理器
/// 数字序列生成、音频播放、按钮/语音输入、答案校验、计分
/// </summary>
public class MemoryGameManager : MonoBehaviour
{
    [Header("组件引用")]
    public ASRManager asrManager;
    public Button startGameButton;
    public TextMeshProUGUI feedbackText;

    [Header("数字按钮 (0-9)")]
    public Button[] numberButtons = new Button[10];

    [Header("计数器")]
    public TextMeshProUGUI counterDisplay;

    [Header("数字音频 (0-9)")]
    public AudioClip[] digitAudioClips = new AudioClip[10];

    [Header("提示音频")]
    public AudioClip audioClipOrder;    // 顺序提示
    public AudioClip audioClipReverse;  // 逆序提示
    public AudioClip audioClipCorrect;  // 正确提示
    public AudioClip audioClipWrong;    // 错误提示
    public AudioClip audioClipRest;     // 休息提示

    // --- 运行时参数 ---
    private int _memoryMode;       // 0=自动, 1=顺序, 2=逆序
    private int _memoryDifficulty; // 0=随机, 1-4 → 2-5位
    private bool _isReverseMode;
    private int _consecutiveCorrect;
    private int _completedRoundsCounter;
    private float _gameStartTime;

    // --- 当前回合状态 ---
    private List<int> _currentSequence = new List<int>();
    private string _correctAnswer;
    private string _playerInput;
    private bool _waitingForInput;
    private bool _gameRunning;
    private bool _answerChecked;
    private string _submittedAnswer;
    private bool _submittedIsVoice;

    private AudioSource _audioSource;

    private static readonly string[] ChineseDigits =
        { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        // 从全局单例读取设置
        if (AllSettingCtr.Instance != null)
        {
            _memoryMode = AllSettingCtr.Instance.memoryMode;
            _memoryDifficulty = AllSettingCtr.Instance.memoryDifficulty;
        }

        // 绑定开始按钮
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);

        // 绑定数字按钮
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int digit = i;
            if (numberButtons[i] != null)
                numberButtons[i].onClick.AddListener(() => OnDigitPressed(digit));
        }

        // 绑定语音识别回调
        if (asrManager != null)
            asrManager.OnASRResultReady += OnASRResult;

        // 初始状态：隐藏输入区域
        SetInputActive(false);
    }

    /// <summary>
    /// 开始游戏按钮回调
    /// </summary>
    public void StartGame()
    {
        if (_gameRunning) return;
        _gameRunning = true;
        _gameStartTime = Time.time;
        _completedRoundsCounter = 0;
        _consecutiveCorrect = 0;

        // 自动模式默认从顺序开始
        _isReverseMode = _memoryMode == 2;

        if (startGameButton != null)
            startGameButton.gameObject.SetActive(false);

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (_gameRunning)
        {
            // 每5分钟提醒休息
            if (Time.time - _gameStartTime > 300f)
            {
                _gameStartTime = Time.time;
                yield return StartCoroutine(PlayClipAndWait(audioClipRest));
                yield return new WaitForSeconds(1f);
            }

            yield return StartCoroutine(PlayRound());
        }
    }

    private IEnumerator PlayRound()
    {
        _playerInput = "";
        _waitingForInput = false;

        // 决定数字位数
        int digitCount = GetDigitCount();

        // 生成随机数字序列
        _currentSequence.Clear();
        for (int i = 0; i < digitCount; i++)
            _currentSequence.Add(Random.Range(0, 10));

        // 生成正确答案（中文）
        _correctAnswer = BuildChineseAnswer(_currentSequence, _isReverseMode);

        // 播放模式提示音，等播完再继续
        yield return StartCoroutine(PlayClipAndWait(_isReverseMode ? audioClipReverse : audioClipOrder));
        yield return new WaitForSeconds(0.3f);

        // 依次播放数字音频，确保前一个播完再播下一个
        for (int i = 0; i < _currentSequence.Count; i++)
        {
            int digit = _currentSequence[i];
            if (digit < digitAudioClips.Length && digitAudioClips[digit] != null)
            {
                yield return StartCoroutine(PlayClipAndWait(digitAudioClips[digit]));
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }

        // 激活输入
        if (feedbackText != null)
            feedbackText.text = "请回答";
        SetInputActive(true);
        _waitingForInput = true;
        _submittedAnswer = null;

        // 等待玩家输入完成
        while (_waitingForInput)
            yield return null;

        // 在同一协程链中校验答案，确保提示音播完再进入下一轮
        yield return StartCoroutine(CheckAnswer(_submittedAnswer, _submittedIsVoice));
    }

    /// <summary>
    /// 数字按钮点击回调
    /// </summary>
    private void OnDigitPressed(int digit)
    {
        if (!_waitingForInput) return;

        _playerInput += ChineseDigits[digit];

        // 输入位数达到序列长度时自动提交
        if (_playerInput.Length >= _currentSequence.Count)
        {
            _submittedAnswer = _playerInput;
            _submittedIsVoice = false;
            _waitingForInput = false;
            SetInputActive(false);
        }
    }

    /// <summary>
    /// 语音识别结果回调
    /// </summary>
    private void OnASRResult(string result)
    {
        if (!_waitingForInput) return;

        string cleaned = System.Text.RegularExpressions.Regex.Replace(result, @"[^\u4e00-\u9fa5]", "");
        _submittedAnswer = cleaned;
        _submittedIsVoice = true;
        _waitingForInput = false;
        SetInputActive(false);
    }

    private IEnumerator CheckAnswer(string answer, bool isVoice)
    {
        _completedRoundsCounter++;

        if (answer == _correctAnswer)
        {
            // 正确
            if (feedbackText != null)
                feedbackText.text = "正确！";

            _consecutiveCorrect++;
            yield return StartCoroutine(PlayClipAndWait(audioClipCorrect));

            // 自动模式：连续答对3次切换到逆序
            if (_memoryMode == 0 && !_isReverseMode && _consecutiveCorrect >= 3)
            {
                _isReverseMode = true;
                _consecutiveCorrect = 0;
            }
        }
        else
        {
            // 错误
            if (isVoice)
            {
                if (feedbackText != null)
                    feedbackText.text = $"错误。识别到: {answer}。正确答案是: {_correctAnswer}";
            }
            else
            {
                if (feedbackText != null)
                    feedbackText.text = $"错误。正确答案是: {_correctAnswer}";
            }
            yield return StartCoroutine(PlayClipAndWait(audioClipWrong));

            // 自动模式：答错重置，回到顺序
            if (_memoryMode == 0)
            {
                _consecutiveCorrect = 0;
                _isReverseMode = false;
            }
        }

        // 更新计数器
        if (counterDisplay != null)
            counterDisplay.text = $"你已经做了 {_completedRoundsCounter} 道题！";

        yield return new WaitForSeconds(2f);
    }

    private int GetDigitCount()
    {
        switch (_memoryDifficulty)
        {
            case 1: return 2;
            case 2: return 3;
            case 3: return 4;
            case 4: return 5;
            default: return Random.Range(2, 6); // 0=随机 2-5位
        }
    }

    private string BuildChineseAnswer(List<int> sequence, bool reverse)
    {
        string result = "";
        if (reverse)
        {
            for (int i = sequence.Count - 1; i >= 0; i--)
                result += ChineseDigits[sequence[i]];
        }
        else
        {
            for (int i = 0; i < sequence.Count; i++)
                result += ChineseDigits[sequence[i]];
        }
        return result;
    }

    private void SetInputActive(bool active)
    {
        foreach (var btn in numberButtons)
        {
            if (btn != null)
                btn.gameObject.SetActive(active);
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.Stop();
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }

    /// <summary>
    /// 播放音频并等待播放完毕，确保不会重叠
    /// </summary>
    private IEnumerator PlayClipAndWait(AudioClip clip)
    {
        if (_audioSource == null || clip == null) yield break;

        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();

        // 等待播放完毕
        while (_audioSource.isPlaying)
            yield return null;
    }

    private void OnDestroy()
    {
        if (asrManager != null)
            asrManager.OnASRResultReady -= OnASRResult;
    }
}
