using UnityEngine;

/// <summary>
/// Direction 设置页脚本（用于 ExcutiveReady 场景的设置 UI）
/// 通过 AllSettingCtr 单例传递参数到 Direction 场景
/// </summary>
public class DirectionSettings : MonoBehaviour
{
    public void OnTimeSliderChanged(float value)
    {
        if (AllSettingCtr.Instance != null)
            AllSettingCtr.Instance.directionGameDuration = value;
    }

    public void OnRandomToggleChanged(bool value)
    {
        if (AllSettingCtr.Instance != null)
            AllSettingCtr.Instance.directionIsRandomMode = value;
    }

    public void OnGroundSliderChanged(float value)
    {
        if (AllSettingCtr.Instance != null)
            AllSettingCtr.Instance.directionGameRounds = value;
    }
}
