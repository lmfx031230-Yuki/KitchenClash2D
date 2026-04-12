using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Team Revenue")]
    [SerializeField] private TextMeshProUGUI teamARevenueText;  // Player + AI Chef 1
    [SerializeField] private TextMeshProUGUI teamBRevenueText;  // AI Chef 2 + AI Chef 3

    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI playerHandCountText;
    [SerializeField] private TextMeshProUGUI teammateInfoText;
    [SerializeField] private TextMeshProUGUI opponentLeftInfoText;
    [SerializeField] private TextMeshProUGUI opponentRightInfoText;

    [Header("Round Info")]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI currentPlayerText;

    [Header("Table")]
    [SerializeField] private TextMeshProUGUI tableStatusText;  // rot warning etc.

    [Header("Message")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void RefreshRevenue(PlayerAgent[] players)
    {
        var gm = GameManager.Instance;
        int teamA = gm.GetTeamRevenue(0);
        int teamB = gm.GetTeamRevenue(1);

        if (teamARevenueText != null)
            teamARevenueText.text = $"Team A: ${teamA}";
        if (teamBRevenueText != null)
            teamBRevenueText.text = $"Team B: ${teamB}";

        // Individual info
        if (playerHandCountText != null)
            playerHandCountText.text = $"Hand: {players[0].Hand.Count}  ${players[0].Revenue}";
        if (teammateInfoText != null)
            teammateInfoText.text = $"{players[1].PlayerName}\nHand: {players[1].Hand.Count}  ${players[1].Revenue}";
        if (opponentLeftInfoText != null)
            opponentLeftInfoText.text = $"{players[2].PlayerName}\nHand: {players[2].Hand.Count}  ${players[2].Revenue}";
        if (opponentRightInfoText != null)
            opponentRightInfoText.text = $"{players[3].PlayerName}\nHand: {players[3].Hand.Count}  ${players[3].Revenue}";
    }

    public void ShowMessage(string msg, float duration = 2f)
    {
        if (messageText == null) return;
        messageText.text = msg;
        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), duration);
    }

    private void ClearMessage() { if (messageText != null) messageText.text = ""; }

    public void RefreshRoundInfo(int current, int total, string playerName)
    {
        if (roundText != null) roundText.text = $"Round {current}/{total}";
        if (currentPlayerText != null) currentPlayerText.text = $"Turn: {playerName}";
    }

    public void ShowTableStatus(string status)
    {
        if (tableStatusText != null) tableStatusText.text = status;
    }

    public void ShowGameOver(PlayerAgent[] players)
    {
        if (gameOverPanel == null) return;
        gameOverPanel.SetActive(true);

        var gm = GameManager.Instance;
        int teamA = gm.GetTeamRevenue(0);
        int teamB = gm.GetTeamRevenue(1);
        string winner = teamA >= teamB ? "Team A Wins!" : "Team B Wins!";

        string result = $"=== GAME OVER ===\n{winner}\n\n";
        result += $"Team A (Player + AI Chef 1): ${teamA}\n";
        result += $"  Player: ${players[0].Revenue}  |  AI Chef 1: ${players[1].Revenue}\n\n";
        result += $"Team B (AI Chef 2 + AI Chef 3): ${teamB}\n";
        result += $"  AI Chef 2: ${players[2].Revenue}  |  AI Chef 3: ${players[3].Revenue}";

        if (gameOverText != null) gameOverText.text = result;
    }
}
