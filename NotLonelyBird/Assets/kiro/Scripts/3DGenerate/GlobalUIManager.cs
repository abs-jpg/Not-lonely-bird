using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI总管单例，集中管理所有UI面板和3D物体的显隐
/// 挂载在 GlobalUIManager 游戏对象上
/// </summary>
public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager Instance { get; private set; }

    [Header("Tripo 引用")]
    [Tooltip("Tripo_Manager 上的 TripoRuntimeCore 组件（通过 MonoBehaviour 引用）")]
    public MonoBehaviour TripoRuntimeCore;

    [Header("进度条")]
    public Slider tripoSlider;

    [Header("受管理的 UI CanvasGroup")]
    [Tooltip("索引0=null(占位), 索引1=画板面板CanvasGroup")]
    public CanvasGroup[] managedUICanvasGroups;

    [Header("受管理的 3D 物体")]
    [Tooltip("索引0=画笔模型, 索引1=Board(画板), 索引2=Gear(齿轮)")]
    public GameObject[] managed3DObjects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 隐藏所有受管理的UI面板和3D物体
    /// </summary>
    public void HideAllManagedItems()
    {
        foreach (var cg in managedUICanvasGroups)
        {
            if (cg != null)
            {
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }

        foreach (var obj in managed3DObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    /// <summary>
    /// 显示所有受管理的UI面板和3D物体
    /// </summary>
    public void ShowAllManagedItems()
    {
        foreach (var cg in managedUICanvasGroups)
        {
            if (cg != null)
            {
                cg.alpha = 1;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
        }

        foreach (var obj in managed3DObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    /// <summary>
    /// 更新进度条值 (0~1)
    /// </summary>
    public void UpdateSlider(float value)
    {
        if (tripoSlider != null)
            tripoSlider.value = value;
    }

    /// <summary>
    /// 设置进度条显隐
    /// </summary>
    public void SetSliderActive(bool active)
    {
        if (tripoSlider != null)
            tripoSlider.gameObject.SetActive(active);
    }
}
