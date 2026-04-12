using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private HandView handView;
    [SerializeField] private int initialHandSize = 10;
    [SerializeField] private int initialRevenue  = 20;
    [SerializeField] private int totalRounds     = 20;

    public PlayerAgent[] Players { get; private set; }
    public PlayerAgent LocalPlayer => Players[0];

    public int CurrentRound { get; private set; } = 1;
    public int TotalRounds  => totalRounds;
    public bool IsGameOver  { get; private set; } = false;

    // Team 0: Players[0] + Players[1]
    // Team 1: Players[2] + Players[3]
    public int GetTeamRevenue(int teamId)
    {
        int sum = 0;
        foreach (var p in Players)
            if (p.TeamId == teamId) sum += p.Revenue;
        return sum;
    }

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
        handView.Init(LocalPlayer.Hand);
        UIManager.Instance.RefreshRevenue(Players);
        TurnManager.Instance.StartGame();
    }

    private void InitPlayers()
    {
        // teamId: 0=Player+AI1, 1=AI2+AI3
        Players = new PlayerAgent[]
        {
            new PlayerAgent(0, "Player",    initialRevenue, false, 0),
            new PlayerAgent(1, "AI Chef 1", initialRevenue, true,  0),
            new PlayerAgent(2, "AI Chef 2", initialRevenue, true,  1),
            new PlayerAgent(3, "AI Chef 3", initialRevenue, true,  1),
        };
    }

    private void DealInitialHands()
    {
        for (int round = 0; round < initialHandSize; round++)
            foreach (var player in Players)
            {
                var card = DeckManager.Instance.DrawCard();
                if (card != null) player.Hand.AddCard(card);
            }

        Debug.Log($"Dealt hands. Deck remaining: {DeckManager.Instance.RemainingCount}");
    }

    public void OnRoundEnd()
    {
        CurrentRound++;
        CheckEndConditions();
    }

    /// <summary>每次有人出牌/摸牌后检查结束条件</summary>
    public void CheckEndConditions()
    {
        // 任意玩家手牌清空
        foreach (var p in Players)
        {
            if (p.Hand.Count == 0)
            {
                TriggerGameOver($"{p.PlayerName}'s hand is empty!");
                return;
            }
        }

        // 牌堆中餐具牌用完
        if (!DeckManager.Instance.HasCardOfType(CardType.Utensil) &&
            !AnyPlayerHasUtensil())
        {
            TriggerGameOver("All utensil cards used up!");
            return;
        }

        // 回合上限保底
        if (CurrentRound > totalRounds)
        {
            TriggerGameOver("Round limit reached!");
        }
    }

    private bool AnyPlayerHasUtensil()
    {
        foreach (var p in Players)
            if (p.Hand.GetCardsByType(CardType.Utensil).Count > 0)
                return true;
        return false;
    }

    private void TriggerGameOver(string reason)
    {
        IsGameOver = true;
        Debug.Log($"Game Over: {reason}");
        UIManager.Instance.ShowGameOver(Players);
    }
}
