using UnityEngine;

/// <summary>
/// 游戏总控：初始化玩家、发牌、驱动回合
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("规则")]
    [SerializeField] private int initialHandSize = 8;
    [SerializeField] private int initialRevenue = 20;
    [SerializeField] private int totalRounds = 10;

    // 4名玩家（index 0 = 本地玩家，1-3 = AI）
    public PlayerAgent[] Players { get; private set; }
    public PlayerAgent LocalPlayer => Players[0];

    public int CurrentRound { get; private set; } = 1;
    public int TotalRounds => totalRounds;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        InitPlayers();
        DeckManager.Instance.BuildAndShuffle();
        DealInitialHands();
        TurnManager.Instance.StartGame();
    }

    private void InitPlayers()
    {
        string[] names = { "玩家", "AI厨师1", "AI厨师2", "AI厨师3" };
        Players = new PlayerAgent[4];
        for (int i = 0; i < 4; i++)
        {
            Players[i] = new PlayerAgent(i, names[i], initialRevenue, i > 0);
        }
    }

    private void DealInitialHands()
    {
        for (int round = 0; round < initialHandSize; round++)
        {
            foreach (var player in Players)
            {
                var card = DeckManager.Instance.DrawCard();
                if (card != null) player.Hand.AddCard(card);
            }
        }
        Debug.Log($"[GameManager] 发牌完成，牌堆剩余 {DeckManager.Instance.RemainingCount} 张");
    }

    public void OnRoundEnd()
    {
        CurrentRound++;
        if (CurrentRound > totalRounds || DeckManager.Instance.RemainingCount == 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("[GameManager] 游戏结束！");
        UIManager.Instance.ShowGameOver(Players);
    }
}
