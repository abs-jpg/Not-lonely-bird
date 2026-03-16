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
