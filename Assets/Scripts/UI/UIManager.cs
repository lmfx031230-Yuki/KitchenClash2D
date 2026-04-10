using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI[] revenueTexts;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI currentPlayerText;
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
        for (int i = 0; i < players.Length && i < revenueTexts.Length; i++)
            revenueTexts[i].text = $"{players[i].PlayerName}\n${players[i].Revenue}";
    }

    public void ShowMessage(string msg, float duration = 2f)
    {
        if (messageText == null) return;
        messageText.text = msg;
        CancelInvoke(nameof(ClearMessage));
        Invoke(nameof(ClearMessage), duration);
    }

    private void ClearMessage() => messageText.text = "";

    public void RefreshRoundInfo(int current, int total, string playerName)
    {
        if (roundText != null) roundText.text = $"Round {current}/{total}";
        if (currentPlayerText != null) currentPlayerText.text = $"Turn: {playerName}";
    }

    public void ShowGameOver(PlayerAgent[] players)
    {
        if (gameOverPanel == null) return;
        gameOverPanel.SetActive(true);

        var sorted = new PlayerAgent[players.Length];
        players.CopyTo(sorted, 0);
        System.Array.Sort(sorted, (a, b) => b.Revenue.CompareTo(a.Revenue));

        string result = "=== GAME OVER ===\n\n";
        string[] medals = { "1st", "2nd", "3rd", "4th" };
        for (int i = 0; i < sorted.Length; i++)
            result += $"{medals[i]}  {sorted[i].PlayerName}  ${sorted[i].Revenue}\n";

        gameOverText.text = result;
    }
}
