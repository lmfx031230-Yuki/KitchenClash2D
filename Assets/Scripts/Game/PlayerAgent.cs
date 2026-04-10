/// <summary>
/// 玩家数据模型（本地玩家和AI共用）
/// </summary>
public class PlayerAgent
{
    public int PlayerId { get; private set; }
    public string PlayerName { get; private set; }
    public bool IsAI { get; private set; }

    public PlayerHand Hand { get; private set; }
    public int Revenue { get; set; }

    public bool SkipNextTurn { get; set; }
    public bool SeafoodDisabled { get; set; }

    public PlayerAgent(int id, string name, int startRevenue, bool isAI)
    {
        PlayerId = id;
        PlayerName = name;
        Revenue = startRevenue;
        IsAI = isAI;
        Hand = new PlayerHand();
    }
}
