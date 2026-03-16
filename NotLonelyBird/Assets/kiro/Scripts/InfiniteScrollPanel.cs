using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 无限水平滚动面板：当内容滚动到边界时，将首/尾元素移到另一端实现循环
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class InfiniteScrollPanel : MonoBehaviour
{
    private ScrollRect _scrollRect;
    private RectTransform _content;
    private RectTransform[] _items;
    private float _itemWidth;
    private float _spacing;
    private int _itemCount;
    private bool _initialized;

    void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _content = _scrollRect.content;

        var layout = _content.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
            _spacing = layout.spacing;

        _itemCount = _content.childCount;
        if (_itemCount == 0) return;

        _items = new RectTransform[_itemCount];
        for (int i = 0; i < _itemCount; i++)
            _items[i] = _content.GetChild(i) as RectTransform;

        _itemWidth = _items[0].rect.width;
        _initialized = true;
    }

    void LateUpdate()
    {
        if (!_initialized) return;

        float step = _itemWidth + _spacing;

        // 向左滚动时，第一个元素超出左边界 → 移到末尾
        var first = _content.GetChild(0) as RectTransform;
        if (first.anchoredPosition.x + _content.anchoredPosition.x < -_itemWidth)
        {
            first.SetAsLastSibling();
            _content.anchoredPosition += new Vector2(step, 0);
        }

        // 向右滚动时，最后一个元素超出右边界 → 移到开头
        var last = _content.GetChild(_itemCount - 1) as RectTransform;
        if (last.anchoredPosition.x + _content.anchoredPosition.x > _scrollRect.GetComponent<RectTransform>().rect.width)
        {
            last.SetAsFirstSibling();
            _content.anchoredPosition -= new Vector2(step, 0);
        }
    }
}
