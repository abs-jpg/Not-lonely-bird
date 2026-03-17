using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// DigitSpan 设置界面
/// 在 MemoryReady 场景中使用，配置模式和难度
/// </summary>
public class MemorySettingsMenu : MonoBehaviour
{
    [Header("模式选择 (ToggleGroup)")]
    public ToggleGroup modeToggleGroup;
    public Toggle autoModeToggle;     // 自动模式 (0)
    public Toggle orderModeToggle;    // 固定顺序 (1)
    public Toggle reverseModeToggle;  // 固定逆序 (2)

    [Header("难度选择 (Slider)")]
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyLabel;

    [Header("确认按钮")]
    public Button confirmButton;

    private static readonly string[] DifficultyNames =
        { "随机(2-5位)", "2位", "3位", "4位", "5位" };

    private void Start()
    {
        // 初始化 UI
        if (AllSettingCtr.Instance != null)
        {
            int mode = AllSettingCtr.Instance.memoryMode;
            if (autoModeToggle != null) autoModeToggle.isOn = mode == 0;
            if (orderModeToggle != null) orderModeToggle.isOn = mode == 1;
            if (reverseModeToggle != null) reverseModeToggle.isOn = mode == 2;

            if (difficultySlider != null)
            {
                difficultySlider.minValue = 0;
                difficultySlider.maxValue = 4;
                difficultySlider.wholeNumbers = true;
                difficultySlider.value = AllSettingCtr.Instance.memoryDifficulty;
            }
        }

        UpdateDifficultyLabel();

        if (difficultySlider != null)
            difficultySlider.onValueChanged.AddListener(_ => UpdateDifficultyLabel());

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ApplySettings);
    }

    private void UpdateDifficultyLabel()
    {
        if (difficultyLabel != null && difficultySlider != null)
        {
            int idx = Mathf.Clamp((int)difficultySlider.value, 0, DifficultyNames.Length - 1);
            difficultyLabel.text = DifficultyNames[idx];
        }
    }

    private void ApplySettings()
    {
        if (AllSettingCtr.Instance == null) return;

        // 模式
        if (autoModeToggle != null && autoModeToggle.isOn)
            AllSettingCtr.Instance.memoryMode = 0;
        else if (orderModeToggle != null && orderModeToggle.isOn)
            AllSettingCtr.Instance.memoryMode = 1;
        else if (reverseModeToggle != null && reverseModeToggle.isOn)
            AllSettingCtr.Instance.memoryMode = 2;

        // 难度
        if (difficultySlider != null)
            AllSettingCtr.Instance.memoryDifficulty = (int)difficultySlider.value;

        Debug.Log($"DigitSpan 设置已更新: 模式={AllSettingCtr.Instance.memoryMode}, 难度={AllSettingCtr.Instance.memoryDifficulty}");
    }
}
