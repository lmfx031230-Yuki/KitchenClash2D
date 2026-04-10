using UnityEngine;
using TMPro;

/// <summary>
/// 管理全局UI：玩家收益显示、提示文本、游戏结束界面
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("收益显示（4个文本，顺序对应玩家0-3）")]
    [SerializeField] private TextMeshProUGUI[] revenueTexts;

    [Header("提示文本")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("回合信息")]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI currentPlayerText;

    [Header("游戏结束界面")]
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
        {
            revenueTexts[i].text = $"{players[i].PlayerName}\n¥{players[i].Revenue}";
        }
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
        if (roundText != null) roundText.text = $"第 {current}/{total} 轮";
        if (currentPlayerText != null) currentPlayerText.text = $"当前：{playerName}";
    }

    public void ShowGameOver(PlayerAgent[] players)
    {
        if (gameOverPanel == null) return;
        gameOverPanel.SetActive(true);

        // 按收益排序
        var sorted = new PlayerAgent[players.Length];
        players.CopyTo(sorted, 0);
        System.Array.Sort(sorted, (a, b) => b.Revenue.CompareTo(a.Revenue));

        string result = "== 游戏结束 ==\n\n";
        for (int i = 0; i < sorted.Length; i++)
        {
            string medal = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : "  ";
            result += $"{medal} {sorted[i].PlayerName}  ¥{sorted[i].Revenue}\n";
        }
        gameOverText.text = result;
    }
}
