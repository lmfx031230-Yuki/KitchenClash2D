using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    public static HandView Instance { get; private set; }

    [SerializeField] private GameObject cardViewPrefab;
    [SerializeField] private Transform  cardContainer;

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

        ArrangeCards();
    }

    private void ArrangeCards()
    {
        int count = _cardViews.Count;
        if (count == 0) return;

        float cardW   = 75f;
        float overlap = 20f;   // 牌之间重叠多少
        float spacing = cardW - overlap;
        float totalW  = spacing * (count - 1);
        float startX  = -totalW / 2f;

        for (int i = 0; i < count; i++)
        {
            var rt = _cardViews[i].GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(cardW, 105f);
            rt.anchoredPosition = new Vector2(startX + spacing * i,
                _cardViews[i] == _selectedCard ? 20f : 0f);
            rt.localRotation = Quaternion.identity;
            rt.SetSiblingIndex(i);
        }

        // 选中牌放最上层
        if (_selectedCard != null)
            _selectedCard.transform.SetAsLastSibling();
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

        ArrangeCards();
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
