using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 单张卡牌的UI显示与点击交互
/// </summary>
public class CardView : MonoBehaviour, IPointerClickHandler
{
    [Header("UI组件")]
    [SerializeField] private Image cardBackground;
    [SerializeField] private Image artworkImage;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image selectionHighlight;

    [Header("卡牌颜色")]
    [SerializeField] private Color ingredientColor = new Color(0.6f, 0.9f, 0.6f);
    [SerializeField] private Color utensilColor    = new Color(0.6f, 0.7f, 0.95f);
    [SerializeField] private Color trashColor      = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color cleanColor      = new Color(0.9f, 0.95f, 0.6f);
    [SerializeField] private Color eventColor      = new Color(0.95f, 0.75f, 0.6f);

    public CardInstance CardInstance { get; private set; }
    private bool _isSelected;

    public void Init(CardInstance instance)
    {
        CardInstance = instance;
        Refresh();
    }

    public void Refresh()
    {
        var data = CardInstance.Data;

        cardNameText.text = data.cardName;
        descriptionText.text = data.description;

        if (data.artwork != null)
            artworkImage.sprite = data.artwork;

        // 背景颜色
        cardBackground.color = GetCardColor(data);

        // 数值显示
        if (data.cardType == CardType.Ingredient)
            valueText.text = $"¥{CardInstance.GetCurrentValue()}";
        else if (data.cardType == CardType.Utensil)
            valueText.text = "餐具";
        else
            valueText.text = "";

        SetSelected(false);
    }

    private Color GetCardColor(CardData data)
    {
        return data.cardType switch
        {
            CardType.Ingredient => ingredientColor,
            CardType.Utensil    => utensilColor,
            CardType.Function   => data.functionType switch
            {
                FunctionType.Trash => trashColor,
                FunctionType.Clean => cleanColor,
                _                  => eventColor
            },
            _ => Color.white
        };
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        if (selectionHighlight != null)
            selectionHighlight.gameObject.SetActive(selected);

        // 选中时上移
        var pos = GetComponent<RectTransform>().anchoredPosition;
        pos.y = selected ? 30f : 0f;
        GetComponent<RectTransform>().anchoredPosition = pos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HandView.Instance?.OnCardClicked(this);
    }
}
