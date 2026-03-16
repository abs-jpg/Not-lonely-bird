using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// NBack 设置界面 UI 事件绑定
/// 在设置场景中使用，修改 AllSettingCtr 的参数
/// </summary>
public class NBackSetting : MonoBehaviour
{
    [Header("UI 控件")]
    public TMP_InputField nValueInput;
    public TMP_InputField stimulusDurationInput;
    public TMP_InputField interStimulusIntervalInput;
    public TMP_InputField totalTrialsInput;
    public TMP_InputField matchProbabilityInput;
    public Toggle oneBlockModeToggle;
    public Button confirmButton;

    private void Start()
    {
        // 初始化 UI 显示
        if (AllSettingCtr.Instance != null)
        {
            var s = AllSettingCtr.Instance;
            if (nValueInput != null) nValueInput.text = s.nValue.ToString();
            if (stimulusDurationInput != null) stimulusDurationInput.text = s.stimulusDuration.ToString("F1");
            if (interStimulusIntervalInput != null) interStimulusIntervalInput.text = s.interStimulusInterval.ToString("F1");
            if (totalTrialsInput != null) totalTrialsInput.text = s.totalTrials.ToString();
            if (matchProbabilityInput != null) matchProbabilityInput.text = s.matchProbability.ToString("F2");
            if (oneBlockModeToggle != null) oneBlockModeToggle.isOn = s.isOneBlockMode;
        }

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ApplySettings);
    }

    private void ApplySettings()
    {
        if (AllSettingCtr.Instance == null) return;
        var s = AllSettingCtr.Instance;

        if (nValueInput != null && int.TryParse(nValueInput.text, out int n))
            s.nValue = n;
        if (stimulusDurationInput != null && float.TryParse(stimulusDurationInput.text, out float sd))
            s.stimulusDuration = sd;
        if (interStimulusIntervalInput != null && float.TryParse(interStimulusIntervalInput.text, out float isi))
            s.interStimulusInterval = isi;
        if (totalTrialsInput != null && int.TryParse(totalTrialsInput.text, out int tt))
            s.totalTrials = tt;
        if (matchProbabilityInput != null && float.TryParse(matchProbabilityInput.text, out float mp))
            s.matchProbability = mp;
        if (oneBlockModeToggle != null)
            s.isOneBlockMode = oneBlockModeToggle.isOn;

        Debug.Log("NBack 设置已更新");
    }
}
