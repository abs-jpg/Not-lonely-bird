using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 截取画板内容并通过 Tripo API 生成3D模型
/// 挂载在 BordController 对象上
/// </summary>
public class DrawingScreenshotter : MonoBehaviour
{
    [Header("UI 引用")]
    public Slider progressSlider;

    [Header("3D 模型容器")]
    [Tooltip("SimpleGameObject — 生成的3D模型放置于此")]
    public GameObject simpleModel;

    [Header("运行时查找")]
    private MonoBehaviour tripoRuntime;
    private MonoBehaviour drawingBoard;

    private bool isGenerating = false;

    private void Start()
    {
        // 运行时查找 TripoRuntimeCore 和 DrawingBoard
        tripoRuntime = FindByTypeName("TripoRuntimeCore");
        drawingBoard = FindByTypeName("DrawingBoard");

        // 禁用 SimpleGameObject 的 BoxCollider，生成完成后启用
        if (simpleModel != null)
        {
            var col = simpleModel.GetComponent<BoxCollider>();
            if (col != null) col.enabled = false;
        }
    }

    /// <summary>
    /// 截取画板内容并保存为图片，然后调用 Tripo ImageToModel
    /// </summary>
    public void CaptureAndSaveToFile()
    {
        if (isGenerating)
        {
            Debug.LogWarning("DrawingScreenshotter: 正在生成中，请等待");
            return;
        }

        if (drawingBoard == null)
        {
            Debug.LogError("DrawingScreenshotter: 未找到 DrawingBoard");
            return;
        }

        // 通过反射获取 drawingTexture
        var texField = drawingBoard.GetType().GetField("drawingTexture",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (texField == null)
        {
            Debug.LogError("DrawingScreenshotter: DrawingBoard 中未找到 drawingTexture 字段");
            return;
        }

        Texture2D tex = texField.GetValue(drawingBoard) as Texture2D;
        if (tex == null)
        {
            Debug.LogError("DrawingScreenshotter: drawingTexture 为空");
            return;
        }

        // 保存为 PNG
        string filePath = Path.Combine(Application.persistentDataPath, "board_capture.png");
        byte[] pngData = tex.EncodeToPNG();
        File.WriteAllBytes(filePath, pngData);
        Debug.Log("DrawingScreenshotter: 截图已保存到 " + filePath);

        // 调用 Tripo ImageToModel
        if (tripoRuntime != null)
        {
            isGenerating = true;

            // 通过反射调用 SetImagePath 和 ImageToModel
            var setPathMethod = tripoRuntime.GetType().GetMethod("SetImagePath");
            if (setPathMethod != null)
                setPathMethod.Invoke(tripoRuntime, new object[] { filePath });

            var generateMethod = tripoRuntime.GetType().GetMethod("ImageToModel");
            if (generateMethod != null)
                generateMethod.Invoke(tripoRuntime, null);
        }
    }

    /// <summary>
    /// 模型生成完成回调 — 由 TripoRuntimeCore.OnModelGenerateComplete 调用
    /// </summary>
    public void ChangeBool()
    {
        isGenerating = false;

        // 启用 SimpleGameObject 的 BoxCollider，允许用户抓取
        if (simpleModel != null)
        {
            var col = simpleModel.GetComponent<BoxCollider>();
            if (col != null) col.enabled = true;
        }

        Debug.Log("DrawingScreenshotter: 3D模型生成完成");
    }

    /// <summary>
    /// 通过类型名查找场景中的 MonoBehaviour
    /// </summary>
    private MonoBehaviour FindByTypeName(string typeName)
    {
        foreach (var mb in FindObjectsOfType<MonoBehaviour>())
        {
            if (mb.GetType().Name == typeName)
                return mb;
        }
        return null;
    }
}
