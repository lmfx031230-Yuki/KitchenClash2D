using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 本地玩家手牌UI，管理CardView的生成与点击选择
/// </summary>
public class HandView : MonoBehaviour
{
    public static HandView Instance { get; private set; }

    [SerializeField] private GameObject cardViewPrefab;
    [SerializeField] private Transform cardContainer;   // HorizontalLayoutGroup

    private PlayerHand _hand;
    private List<CardView> _cardViews = new List<CardView>();
    private CardView _selectedCard;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Init(PlayerHand hand)
    {
        _hand = hand;
        _hand.OnHandChanged += RefreshView;
        RefreshView();
    }

    private void RefreshView()
    {
        // 清除旧的
        foreach (var cv in _cardViews)
            Destroy(cv.gameObject);
        _cardViews.Clear();
        _selectedCard = null;

        // 重新生成
        foreach (var card in _hand.Cards)
        {
            var go = Instantiate(cardViewPrefab, cardContainer);
            var cv = go.GetComponent<CardView>();
            cv.Init(card);
            _cardViews.Add(cv);
        }
    }

    public void OnCardClicked(CardView clicked)
    {
        // 只有本地玩家回合才能选牌
        if (!TurnManager.Instance.IsLocalPlayerTurn) return;

        if (_selectedCard == clicked)
        {
            // 再次点击取消选中
            _selectedCard.SetSelected(false);
            _selectedCard = null;
        }
        else
        {
            if (_selectedCard != null) _selectedCard.SetSelected(false);
            _selectedCard = clicked;
            _selectedCard.SetSelected(true);
        }
    }

    /// <summary>获取当前选中的卡，并清除选中状态</summary>
    public CardInstance ConsumeSelected()
    {
        if (_selectedCard == null) return null;
        var card = _selectedCard.CardInstance;
        _selectedCard.SetSelected(false);
        _selectedCard = null;
        return card;
    }

    public CardView SelectedCardView => _selectedCard;
}
