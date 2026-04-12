using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 本地玩家手牌UI，扇形排列
/// </summary>
public class HandView : MonoBehaviour
{
    public static HandView Instance { get; private set; }

    [SerializeField] private GameObject cardViewPrefab;
    [SerializeField] private Transform  cardContainer;
    [SerializeField] private HandFanLayout fanLayout;

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
        foreach (var cv in _cardViews) Destroy(cv.gameObject);
        _cardViews.Clear();
        _selectedCard = null;

        foreach (var card in _hand.Cards)
        {
            var go = Instantiate(cardViewPrefab, cardContainer);
            var cv = go.GetComponent<CardView>();
            cv.Init(card);
            _cardViews.Add(cv);
        }

        ApplyFanLayout();
    }

    private void ApplyFanLayout()
    {
        if (fanLayout != null)
            fanLayout.ArrangeCards(_cardViews, _selectedCard);
    }

    public void OnCardClicked(CardView clicked)
    {
        if (!TurnManager.Instance.IsLocalPlayerTurn) return;

        if (_selectedCard == clicked)
        {
            _selectedCard.SetSelected(false);
            _selectedCard = null;
        }
        else
        {
            if (_selectedCard != null) _selectedCard.SetSelected(false);
            _selectedCard = clicked;
            _selectedCard.SetSelected(true);
        }

        ApplyFanLayout();
    }

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
