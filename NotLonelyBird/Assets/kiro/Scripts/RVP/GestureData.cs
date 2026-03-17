using UnityEngine;

/// <summary>
/// 手势数据 ScriptableObject
/// 定义手势类型枚举和单个手势的配置数据
/// </summary>
[CreateAssetMenu(fileName = "NewGesture", menuName = "RVP/GestureData")]
public class GestureData : ScriptableObject
{
    public enum Hand { Left = 0, Right = 1 }

    public enum CustomGestureType
    {
        None = 0,
        Grip = 1,      // 握拳
        Pinch = 2,     // 捏合
        PalmForward = 3, // 掌朝前
        PalmUp = 4     // 掌朝上
    }

    [Header("手势信息")]
    public string gestureName;
    public Sprite gestureImage;
    public Hand hand;
    public CustomGestureType gestureType;
}
