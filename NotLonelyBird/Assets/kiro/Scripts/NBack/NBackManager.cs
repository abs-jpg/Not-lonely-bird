using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// NBack 核心游戏管理器
/// 序列生成、协程驱动游戏流程、计分、结算
/// </summary>
public class NBackManager : MonoBehaviour
{
    [Header("网格格子 (由 SettlementScreen 生成或手动拖入)")]
    public GameObject[] gridCells;

    [Header("3D 刺激物")]
    public GameObject stimulus3DObject;

    [Header("UI 引用")]
    public TextMeshProUGUI feedbackText;
    public Button matchButton;
    public TextMeshProUGUI buttonText;
    public TextMeshPro scoreText;

    [Header("结算/生成配置")]
    public SettlementScreen settings;

    // --- 运行时参数（从 AllSettingCtr 读取） ---
    private int _nValue;
    private float _stimulusDuration;
    private float _interStimulusInterval;
    private int _totalTrials;
    private int _targetIndexN0;
    private float _matchProbability;

    // --- 游戏状态 ---
    private List<int> _positionSequence = new List<int>();
    private int _currentTrial;
    private int _score;
    private bool _responded;
    private bool _isMatch;
    private bool _gameRunning;

    private void Awake()
    {
        // 从全局单例读取设置参数，若不存在则用默认值
        if (AllSettingCtr.Instance != null)
        {
            var s = AllSettingCtr.Instance;
            _nValue = s.nValue;
            _stimulusDuration = s.stimulusDuration;
            _interStimulusInterval = s.interStimulusInterval;
            _totalTrials = s.totalTrials;
            _targetIndexN0 = s.targetIndexN0;
            _matchProbability = s.matchProbability;
        }
        else
        {
            _nValue = 1;
            _stimulusDuration = 2f;
            _interStimulusInterval = 2.5f;
            _totalTrials = 40;
            _targetIndexN0 = 4;
            _matchProbability = 0.33f;
        }
    }

    private void Start()
    {
        if (matchButton != null)
            matchButton.onClick.AddListener(OnMatchButtonClicked);

        GenerateSequence();
        StartCoroutine(RunGame());
    }

    /// <summary>
    /// 生成位置序列，按匹配概率插入匹配试次
    /// </summary>
    private void GenerateSequence()
    {
        _positionSequence.Clear();
        int cellCount = gridCells.Length;

        for (int i = 0; i < _totalTrials; i++)
        {
            if (_nValue == 0)
            {
                // N=0 模式：按概率出现在目标位置
                if (Random.value < _matchProbability)
                    _positionSequence.Add(_targetIndexN0);
                else
                {
                    int pos;
                    do { pos = Random.Range(0, cellCount); }
                    while (pos == _targetIndexN0);
                    _positionSequence.Add(pos);
                }
            }
            else
            {
                // N>=1 模式：按概率与 N 步前相同
                if (i >= _nValue && Random.value < _matchProbability)
                    _positionSequence.Add(_positionSequence[i - _nValue]);
                else
                    _positionSequence.Add(Random.Range(0, cellCount));
            }
        }
    }

    /// <summary>
    /// 主游戏协程：逐试次展示刺激物
    /// </summary>
    private IEnumerator RunGame()
    {
        _gameRunning = true;
        _score = 0;
        UpdateScoreText();

        // 开始前 2 秒延迟
        if (feedbackText != null)
            feedbackText.text = "准备开始...";
        yield return new WaitForSeconds(2f);

        for (_currentTrial = 0; _currentTrial < _totalTrials; _currentTrial++)
        {
            _responded = false;
            _isMatch = CheckMatch(_currentTrial);

            int posIndex = _positionSequence[_currentTrial];

            // 将刺激物移动到对应格子位置并激活
            if (stimulus3DObject != null && posIndex < gridCells.Length)
            {
                stimulus3DObject.transform.position = gridCells[posIndex].transform.position;
                stimulus3DObject.SetActive(true);
            }

            if (feedbackText != null)
                feedbackText.text = $"试次 {_currentTrial + 1} / {_totalTrials}";

            // 刺激持续时间
            yield return new WaitForSeconds(_stimulusDuration);

            // 隐藏刺激物
            if (stimulus3DObject != null)
                stimulus3DObject.SetActive(false);

            // 未响应时的判定
            if (!_responded)
            {
                if (!_isMatch)
                    _score++; // 正确拒绝（不该按且没按）
                // 漏报不加分
            }

            UpdateScoreText();

            // 间隔时间
            yield return new WaitForSeconds(_interStimulusInterval);
        }

        _gameRunning = false;
        OnGameEnd();
    }

    /// <summary>
    /// 检查当前试次是否为匹配
    /// </summary>
    private bool CheckMatch(int trialIndex)
    {
        if (_nValue == 0)
            return _positionSequence[trialIndex] == _targetIndexN0;

        if (trialIndex < _nValue)
            return false;

        return _positionSequence[trialIndex] == _positionSequence[trialIndex - _nValue];
    }

    /// <summary>
    /// 匹配按钮点击回调
    /// </summary>
    private void OnMatchButtonClicked()
    {
        if (!_gameRunning || _responded) return;

        _responded = true;

        if (_isMatch)
        {
            _score++;
            if (buttonText != null) buttonText.text = "正确!";
        }
        else
        {
            if (buttonText != null) buttonText.text = "错误!";
        }

        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"{_score} / {_currentTrial + 1}";
    }

    /// <summary>
    /// 游戏结束结算
    /// </summary>
    private void OnGameEnd()
    {
        float accuracy = (float)_score / _totalTrials;

        string result = $"游戏结束!\n正确率: {accuracy:P1}\n得分: {_score} / {_totalTrials}";

        if (feedbackText != null)
            feedbackText.text = result;

        if (buttonText != null)
            buttonText.text = "已结束";

        Debug.Log(result);
    }
}
