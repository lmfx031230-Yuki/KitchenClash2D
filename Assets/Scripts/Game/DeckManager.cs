using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理共用牌堆：建堆、洗牌、摸牌
/// </summary>
public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }

    [Header("所有卡牌数据（拖入全部CardData）")]
    [SerializeField] private CardData[] allCards;

    private List<CardInstance> _deck = new List<CardInstance>();

    public int RemainingCount => _deck.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void BuildAndShuffle()
    {
        _deck.Clear();

        foreach (var data in allCards)
        {
            for (int i = 0; i < data.cardCount; i++)
                _deck.Add(new CardInstance(data));
        }

        Shuffle();
        Debug.Log($"[DeckManager] 牌堆建立完成，共 {_deck.Count} 张");
    }

    private void Shuffle()
    {
        for (int i = _deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (_deck[i], _deck[j]) = (_deck[j], _deck[i]);
        }
    }

    /// <summary>摸一张牌，牌堆空时返回null</summary>
    public CardInstance DrawCard()
    {
        if (_deck.Count == 0) return null;
        var card = _deck[0];
        _deck.RemoveAt(0);
        return card;
    }

    /// <summary>摸指定类型的牌，找不到返回null</summary>
    public CardInstance DrawCardOfType(CardType type)
    {
        var index = _deck.FindIndex(c => c.Data.cardType == type);
        if (index < 0) return null;
        var card = _deck[index];
        _deck.RemoveAt(index);
        return card;
    }
}
