using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 玩家手牌，纯数据层，不含任何Unity组件
/// </summary>
public class PlayerHand
{
    private List<CardInstance> _cards = new List<CardInstance>();

    public IReadOnlyList<CardInstance> Cards => _cards;
    public int Count => _cards.Count;

    public void AddCard(CardInstance card)
    {
        _cards.Add(card);
        OnHandChanged?.Invoke();
    }

    public bool RemoveCard(CardInstance card)
    {
        bool removed = _cards.Remove(card);
        if (removed) OnHandChanged?.Invoke();
        return removed;
    }

    public List<CardInstance> GetCardsByType(CardType type)
    {
        return _cards.Where(c => c.Data.cardType == type).ToList();
    }

    public void Clear()
    {
        _cards.Clear();
        OnHandChanged?.Invoke();
    }

    public System.Action OnHandChanged;
}
