using UnityEngine;

/// <summary>
/// 跟随头部（主摄像机）的脚本，用于 Score 等 UI 元素
/// </summary>
public class FollowHead : MonoBehaviour
{
    public Vector3 offsetPosition = new Vector3(16f, 9.5f, 60f);

    private Transform _cameraTransform;

    private void Start()
    {
        if (Camera.main != null)
            _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (_cameraTransform == null) return;

        transform.position = _cameraTransform.position + _cameraTransform.TransformDirection(offsetPosition);
        transform.rotation = _cameraTransform.rotation;
    }
}
