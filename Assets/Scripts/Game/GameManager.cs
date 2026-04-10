using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private HandView handView;

    [SerializeField] private int initialHandSize = 8;
    [SerializeField] private int initialRevenue = 20;
    [SerializeField] private int totalRounds = 10;

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
        handView.Init(LocalPlayer.Hand);
        UIManager.Instance.RefreshRevenue(Players);
        TurnManager.Instance.StartGame();
    }

    private void InitPlayers()
    {
        string[] names = { "Player", "AI Chef 1", "AI Chef 2", "AI Chef 3" };
        Players = new PlayerAgent[4];
        for (int i = 0; i < 4; i++)
            Players[i] = new PlayerAgent(i, names[i], initialRevenue, i > 0);
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
        if (CurrentRound > totalRounds || DeckManager.Instance.RemainingCount == 0)
            UIManager.Instance.ShowGameOver(Players);
    }
}
