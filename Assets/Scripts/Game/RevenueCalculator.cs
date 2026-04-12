using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 结算共用桌面的收益并分配：
/// 提交者获得70%，贡献食材的玩家按比例分30%
/// </summary>
public static class RevenueCalculator
{
    public static void Calculate(int submitterIndex)
    {
        var table   = TableManager.Instance;
        var players = GameManager.Instance.Players;

        int baseRevenue = 0;
        int trashCount  = 0;
        bool hasBonusCard = false;

        foreach (var card in table.TableCards)
        {
            switch (card.Data.cardType)
            {
                case CardType.Ingredient:
                    // 冷链断裂效果
                    if (players[submitterIndex].SeafoodDisabled &&
                        card.Data.ingredientCategory == IngredientCategory.Seafood)
                        break;
                    baseRevenue += card.GetCurrentValue();
                    break;

                case CardType.Function:
                    if (card.Data.effectType == EffectType.ReduceRevenue)
                        trashCount++;
                    if (card.Data.effectType == EffectType.BonusIfNoTrash)
                        hasBonusCard = true;
                    break;
            }
        }

        // 垃圾卡扣减
        float penalty = Mathf.Max(1f - trashCount * 0.15f, 0.1f);
        float total   = baseRevenue * penalty;

        // 五星认证加成
        if (hasBonusCard && trashCount == 0)
            total *= 1.25f;

        // 腐烂扣减
        total *= table.RotMultiplier;

        int totalInt = Mathf.RoundToInt(total);

        // 70% 给提交者
        int submitterShare = Mathf.RoundToInt(totalInt * 0.7f);
        players[submitterIndex].Revenue += submitterShare;

        // 30% 按贡献食材数量比例分给贡献者（不含提交者自己的重复计算）
        int contributionPool = totalInt - submitterShare;
        DistributeContributions(contributionPool, submitterIndex, players);

        players[submitterIndex].SeafoodDisabled = false;

        // 腐烂强制结算罚款
        if (table.ShouldForceSettle)
        {
            foreach (var p in players)
                p.Revenue -= 20;
            UIManager.Instance.ShowMessage("Dish spoiled! All players fined $20!", 3f);
        }
        else
        {
            UIManager.Instance.ShowMessage(
                $"{players[submitterIndex].PlayerName} served the dish! +${submitterShare}", 2.5f);
        }

        UIManager.Instance.RefreshRevenue(players);
    }

    private static void DistributeContributions(int pool, int submitterIndex, PlayerAgent[] players)
    {
        // 统计各玩家贡献食材数
        int[] counts = new int[players.Length];
        int totalCount = 0;

        foreach (var p in players)
        {
            counts[p.PlayerId] = p.ContributedCards.Count;
            totalCount += p.ContributedCards.Count;
        }

        if (totalCount == 0) return;

        foreach (var p in players)
        {
            if (counts[p.PlayerId] == 0) continue;
            int share = Mathf.RoundToInt(pool * (float)counts[p.PlayerId] / totalCount);
            p.Revenue += share;
        }
    }
}
