using UnityEngine;

/// <summary>
/// 把手牌排成扇形（类似UNO桌面效果）
/// 挂在HandView同一个物体上，替代HorizontalLayoutGroup
/// </summary>
public class HandFanLayout : MonoBehaviour
{
    [Header("扇形参数")]
    [SerializeField] private float cardWidth    = 80f;
    [SerializeField] private float cardHeight   = 110f;
    [SerializeField] private float fanSpread    = 35f;   // 每张牌之间的角度
    [SerializeField] private float maxAngle     = 30f;   // 最大总张角（度）
    [SerializeField] private float arcRadius    = 800f;  // 圆弧半径（越大越平）
    [SerializeField] private float hoverOffset  = 30f;   // 选中时上移

    public void ArrangeCards(System.Collections.Generic.List<CardView> cards, CardView selected)
    {
        if (cards.Count == 0) return;

        int count = cards.Count;

        // 计算每张牌的角度间隔（牌多时压缩）
        float totalAngle = Mathf.Min(fanSpread * (count - 1), maxAngle * 2);
        float step = count > 1 ? totalAngle / (count - 1) : 0f;
        float startAngle = totalAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle - step * i;  // 正值=左倾，负值=右倾
            float rad = angle * Mathf.Deg2Rad;

            // 在圆弧上的位置
            float x = arcRadius * Mathf.Sin(rad);
            float y = arcRadius * (Mathf.Cos(rad) - 1f); // 让底部对齐

            var rt = cards[i].GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(cardWidth, cardHeight);
            rt.anchoredPosition = new Vector2(x, y + (cards[i] == selected ? hoverOffset : 0f));
            rt.localRotation = Quaternion.Euler(0, 0, angle);

            // 选中的牌渲染在最上层
            cards[i].transform.SetSiblingIndex(cards[i] == selected ? count - 1 : i);
        }
    }
}
