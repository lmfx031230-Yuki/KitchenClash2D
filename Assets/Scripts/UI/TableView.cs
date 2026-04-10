using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 显示本回合桌面上已打出的牌
/// </summary>
public class TableView : MonoBehaviour
{
    public static TableView Instance { get; private set; }

    [SerializeField] private GameObject cardViewPrefab;
    [SerializeField] private Transform cardContainer;

    private List<CardView> _cardViews = new List<CardView>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RefreshTable(IReadOnlyList<CardInstance> cards)
    {
        foreach (var cv in _cardViews)
            Destroy(cv.gameObject);
        _cardViews.Clear();

        foreach (var card in cards)
        {
            var go = Instantiate(cardViewPrefab, cardContainer);
            var cv = go.GetComponent<CardView>();
            cv.Init(card);
            _cardViews.Add(cv);
        }
    }
}
