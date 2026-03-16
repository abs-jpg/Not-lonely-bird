using UnityEngine;

/// <summary>
/// 方块生成配置 — 在 Inspector 中直接调整参数
/// </summary>
public class SettlementScreen : MonoBehaviour
{
    [Header("网格布局")]
    [Tooltip("行数")]
    public int rows = 3;
    [Tooltip("列数")]
    public int columns = 3;
    [Tooltip("格子间距")]
    public float spacing = 0.05f;

    [Header("方块外观")]
    [Tooltip("方块缩放")]
    public Vector3 cubeScale = new Vector3(0.03f, 0.03f, 0.03f);
    [Tooltip("方块材质（留空则用默认）")]
    public Material cubeMaterial;
    [Tooltip("使用自定义 Prefab 替代内置 Cube")]
    public GameObject cubePrefab;

    [Header("底板")]
    [Tooltip("底板 Prefab（留空则不生成底板）")]
    public GameObject tilePrefab;
    [Tooltip("底板父节点缩放")]
    public Vector3 tileParentScale = new Vector3(0.05f, 0f, 0.05f);

    /// <summary>
    /// 生成九宫格方块，返回所有格子的 GameObject 数组
    /// </summary>
    public GameObject[] GenerateGrid(Transform parent)
    {
        int total = rows * columns;
        GameObject[] cells = new GameObject[total];

        float startX = -(columns - 1) * spacing * 0.5f;
        float startZ = (rows - 1) * spacing * 0.5f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int index = r * columns + c;
                Vector3 pos = new Vector3(
                    startX + c * spacing,
                    0.015f,
                    startZ - r * spacing
                );

                GameObject cell;
                if (cubePrefab != null)
                    cell = Instantiate(cubePrefab, parent);
                else
                    cell = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cell.name = (index + 1).ToString();
                cell.transform.SetParent(parent, false);
                cell.transform.localPosition = pos;
                cell.transform.localScale = cubeScale;

                if (cubeMaterial != null)
                {
                    var renderer = cell.GetComponent<Renderer>();
                    if (renderer != null) renderer.material = cubeMaterial;
                }

                cell.SetActive(false);
                cells[index] = cell;
            }
        }

        return cells;
    }

    /// <summary>
    /// 生成底板网格
    /// </summary>
    public void GenerateTiles(Transform parent)
    {
        if (tilePrefab == null) return;

        parent.localScale = tileParentScale;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                float x = c - (columns - 1) * 0.5f;
                float z = (rows - 1) * 0.5f - r;

                GameObject tile = Instantiate(tilePrefab, parent);
                tile.transform.localPosition = new Vector3(x, 0f, z);
                tile.transform.localScale = Vector3.one;
            }
        }
    }
}
