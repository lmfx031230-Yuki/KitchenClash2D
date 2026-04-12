using System.Collections.Generic;

/// <summary>
/// 玩家数据模型（本地玩家和AI共用）
/// Team 0: Player (index 0) + AI Chef 1 (index 1)
/// Team 1: AI Chef 2 (index 2) + AI Chef 3 (index 3)
/// </summary>
public class PlayerAgent
{
    public int PlayerId   { get; private set; }
    public string PlayerName { get; private set; }
    public bool IsAI      { get; private set; }
    public int TeamId     { get; private set; }   // 0 or 1

    public PlayerHand Hand     { get; private set; }
    public int Revenue         { get; set; }

    // Status effects
    public bool SkipNextTurn   { get; set; }
    public bool SeafoodDisabled { get; set; }

    // Cards this player contributed to the current shared dish
    public List<CardInstance> ContributedCards { get; private set; } = new List<CardInstance>();

    public PlayerAgent(int id, string name, int startRevenue, bool isAI, int teamId)
    {
        PlayerId   = id;
        PlayerName = name;
        Revenue    = startRevenue;
        IsAI       = isAI;
        TeamId     = teamId;
        Hand       = new PlayerHand();
    }

    public void ClearContributions() => ContributedCards.Clear();
    public void AddContribution(CardInstance card) => ContributedCards.Add(card);
}
