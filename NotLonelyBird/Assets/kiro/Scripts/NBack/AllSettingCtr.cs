using UnityEngine;

/// <summary>
/// 全局单例，DontDestroyOnLoad，跨场景传递设置参数
/// </summary>
public class AllSettingCtr : MonoBehaviour
{
    public static AllSettingCtr Instance { get; private set; }

    [Header("NBack 设置")]
    public int nValue = 1;
    public float stimulusDuration = 2f;
    public float interStimulusInterval = 2.5f;
    public int totalTrials = 40;
    public int targetIndexN0 = 4; // N=0 模式的目标位置（默认中心）
    public float matchProbability = 0.33f;
    public bool isOneBlockMode = false;

    [Header("DigitSpan 设置")]
    public int memoryMode = 0;       // 0=自动, 1=固定顺序, 2=固定逆序
    public int memoryDifficulty = 0; // 0=随机(2-5位), 1→2位, 2→3位, 3→4位, 4→5位

    [Header("RVP (注意力) 设置")]
    public int attentionGesturesPerMinute = 40;  // 每分钟手势呈现数
    public int attentionTargetCount = 3;          // 目标手势数量
    public float attentionFlashDuration = 0.1f;   // 闪烁时长（秒）

    [Header("Direction (方向判断) 设置")]
    public float directionGameDuration = 10f;     // 每回合时长（秒）
    public bool directionIsRandomMode = false;    // 是否随机模式
    public float directionGameRounds = 40f;       // 总回合数

    [Header("Emotion (情感识别) 设置")]
    public int emotionCount = 10;                 // 题目数量
    public float emotionDisplayTime = 3f;         // 情绪显示时间（秒）

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
