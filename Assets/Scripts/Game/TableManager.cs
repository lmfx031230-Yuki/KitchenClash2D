using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 共用桌面管理：所有玩家共享一张桌面
/// 追踪食材、腐烂计数、贡献者
/// </summary>
public class TableManager : MonoBehaviour
{
    public static TableManager Instance { get; private set; }

    // 桌面上的所有牌
    private List<CardInstance> _tableCards = new List<CardInstance>();
    public IReadOnlyList<CardInstance> TableCards => _tableCards;

    // 腐烂计数：第一张食材上桌后开始计回合数
    public int TurnsOnTable { get; private set; } = 0;
    private bool _rottingStarted = false;

    // 贡献者记录：cardInstanceId → playerIndex
    private Dictionary<string, int> _contributions = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool HasIngredient => _tableCards.Exists(c => c.Data.cardType == CardType.Ingredient);
    public bool HasUtensil    => _tableCards.Exists(c => c.Data.cardType == CardType.Utensil);
    public bool IsEmpty       => _tableCards.Count == 0;

    /// <summary>玩家打出一张牌到共用桌面</summary>
    public void PlayCard(CardInstance card, int playerIndex)
    {
        _tableCards.Add(card);
        _contributions[card.InstanceId] = playerIndex;

        // 记录贡献（只记食材）
        if (card.Data.cardType == CardType.Ingredient)
        {
            GameManager.Instance.Players[playerIndex].AddContribution(card);

            // 第一张食材上桌，开始腐烂计时
            if (!_rottingStarted)
            {
                _rottingStarted = true;
                TurnsOnTable = 0;
            }
        }

        TableView.Instance?.RefreshTable(_tableCards);
    }

    /// <summary>每回合结束时推进腐烂计时</summary>
    public void AdvanceRot()
    {
        if (_rottingStarted)
            TurnsOnTable++;
    }

    /// <summary>是否超时强制结算（第4回合）</summary>
    public bool ShouldForceSettle => _rottingStarted && TurnsOnTable >= 4;

    /// <summary>腐烂扣分倍率</summary>
    public float RotMultiplier
    {
        get
        {
            if (!_rottingStarted) return 1f;
            if (TurnsOnTable >= 4) return 0f; // 强制结算时已罚款，菜品价值归零
            if (TurnsOnTable >= 3) return 0.8f; // 第3回合 -20%
            return 1f;
        }
    }

    /// <summary>结算并清空桌面</summary>
    public void ClearTable()
    {
        _tableCards.Clear();
        _contributions.Clear();
        _rottingStarted = false;
        TurnsOnTable = 0;

        foreach (var player in GameManager.Instance.Players)
            player.ClearContributions();

        TableView.Instance?.RefreshTable(_tableCards);
    }

    public string GetRotStatus()
    {
        if (!_rottingStarted) return "";
        if (TurnsOnTable >= 3) return "ROTTING! -20%";
        if (TurnsOnTable == 2) return "Warning: 1 turn before rot!";
        return "";
    }
}
