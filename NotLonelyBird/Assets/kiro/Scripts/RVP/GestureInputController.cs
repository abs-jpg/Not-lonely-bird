using System;
using UnityEngine;

/// <summary>
/// 手势输入控制器
/// 运行时尝试调用 Rokid SDK 手势 API，失败则降级为键盘模拟
/// </summary>
public class GestureInputController : MonoBehaviour
{
    /// <summary>
    /// 手势识别事件：(手, 手势类型)
    /// </summary>
    public static event Action<GestureData.Hand, GestureData.CustomGestureType> OnGesturePerformed;

    private bool _enabled;
    private bool _rokidAvailable;
    private System.Reflection.MethodInfo _getLeftHandInfo;
    private System.Reflection.MethodInfo _getRightHandInfo;
    private System.Reflection.FieldInfo _gestureTypeField;

    // 防止同一手势连续触发的冷却
    private GestureData.CustomGestureType _lastLeftGesture;
    private GestureData.CustomGestureType _lastRightGesture;

    private void Awake()
    {
        TryInitRokidSDK();
    }

    /// <summary>
    /// 运行时通过反射检测 Rokid SDK 是否可用
    /// </summary>
    private void TryInitRokidSDK()
    {
        try
        {
            var gesApiType = Type.GetType("Rokid.UXR.Interaction.GesApi, Assembly-CSharp");
            if (gesApiType == null)
                gesApiType = Type.GetType("Rokid.UXR.Interaction.GesApi, Rokid.UXR.Interaction");

            if (gesApiType != null)
            {
                _getLeftHandInfo = gesApiType.GetMethod("GetLeftHandInfo",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                _getRightHandInfo = gesApiType.GetMethod("GetRightHandInfo",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

                if (_getLeftHandInfo != null && _getRightHandInfo != null)
                {
                    // 获取 HandInfo 的 gesture_type 字段
                    var returnType = _getLeftHandInfo.ReturnType;
                    _gestureTypeField = returnType.GetField("gesture_type");

                    if (_gestureTypeField != null)
                    {
                        _rokidAvailable = true;
                        Debug.Log("[GestureInput] Rokid SDK 手势 API 已找到，使用设备手势检测");
                        return;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[GestureInput] Rokid SDK 初始化失败: {e.Message}");
        }

        _rokidAvailable = false;
        Debug.Log("[GestureInput] Rokid SDK 不可用，使用键盘模拟");
    }

    public void SetDetectionEnabled(bool enabled)
    {
        _enabled = enabled;
        if (!enabled)
        {
            _lastLeftGesture = GestureData.CustomGestureType.None;
            _lastRightGesture = GestureData.CustomGestureType.None;
        }
    }

    private void Update()
    {
        if (!_enabled) return;

        if (_rokidAvailable)
            PollRokidGestures();
        else
            PollKeyboardGestures();
    }

    private void PollRokidGestures()
    {
        try
        {
            // 左手
            var leftHand = _getLeftHandInfo.Invoke(null, null);
            if (leftHand != null)
            {
                int rawType = (int)_gestureTypeField.GetValue(leftHand);
                var type = MapRokidGesture(rawType);
                if (type != GestureData.CustomGestureType.None && type != _lastLeftGesture)
                {
                    _lastLeftGesture = type;
                    OnGesturePerformed?.Invoke(GestureData.Hand.Left, type);
                }
                else if (type == GestureData.CustomGestureType.None)
                {
                    _lastLeftGesture = GestureData.CustomGestureType.None;
                }
            }

            // 右手
            var rightHand = _getRightHandInfo.Invoke(null, null);
            if (rightHand != null)
            {
                int rawType = (int)_gestureTypeField.GetValue(rightHand);
                var type = MapRokidGesture(rawType);
                if (type != GestureData.CustomGestureType.None && type != _lastRightGesture)
                {
                    _lastRightGesture = type;
                    OnGesturePerformed?.Invoke(GestureData.Hand.Right, type);
                }
                else if (type == GestureData.CustomGestureType.None)
                {
                    _lastRightGesture = GestureData.CustomGestureType.None;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[GestureInput] 手势检测异常: {e.Message}");
        }
    }

    private GestureData.CustomGestureType MapRokidGesture(int rokidType)
    {
        switch (rokidType)
        {
            case 1: return GestureData.CustomGestureType.Grip;
            case 2: return GestureData.CustomGestureType.Pinch;
            case 3: return GestureData.CustomGestureType.PalmForward;
            case 4: return GestureData.CustomGestureType.PalmUp;
            default: return GestureData.CustomGestureType.None;
        }
    }

    /// <summary>
    /// 键盘模拟手势（编辑器调试用）
    /// Q/W/E/R = 左手 握拳/捏合/掌朝前/掌朝上
    /// U/I/O/P = 右手 握拳/捏合/掌朝前/掌朝上
    /// </summary>
    private void PollKeyboardGestures()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            OnGesturePerformed?.Invoke(GestureData.Hand.Left, GestureData.CustomGestureType.Grip);
        if (Input.GetKeyDown(KeyCode.W))
            OnGesturePerformed?.Invoke(GestureData.Hand.Left, GestureData.CustomGestureType.Pinch);
        if (Input.GetKeyDown(KeyCode.E))
            OnGesturePerformed?.Invoke(GestureData.Hand.Left, GestureData.CustomGestureType.PalmForward);
        if (Input.GetKeyDown(KeyCode.R))
            OnGesturePerformed?.Invoke(GestureData.Hand.Left, GestureData.CustomGestureType.PalmUp);

        if (Input.GetKeyDown(KeyCode.U))
            OnGesturePerformed?.Invoke(GestureData.Hand.Right, GestureData.CustomGestureType.Grip);
        if (Input.GetKeyDown(KeyCode.I))
            OnGesturePerformed?.Invoke(GestureData.Hand.Right, GestureData.CustomGestureType.Pinch);
        if (Input.GetKeyDown(KeyCode.O))
            OnGesturePerformed?.Invoke(GestureData.Hand.Right, GestureData.CustomGestureType.PalmForward);
        if (Input.GetKeyDown(KeyCode.P))
            OnGesturePerformed?.Invoke(GestureData.Hand.Right, GestureData.CustomGestureType.PalmUp);
    }
}
