using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 计算提交菜品的收益
/// </summary>
public static class RevenueCalculator
{
    public static int Calculate(IReadOnlyList<CardInstance> tableCards, PlayerAgent player)
    {
        int baseRevenue = 0;
        int trashCount  = 0;
        bool hasBonus   = false;
        bool seafoodDisabled = player.SeafoodDisabled;

        foreach (var card in tableCards)
        {
            switch (card.Data.cardType)
            {
                case CardType.Ingredient:
                    if (seafoodDisabled && card.Data.ingredientCategory == IngredientCategory.Seafood)
                        break; // 冷链断裂效果：海鲜不计入
                    baseRevenue += card.GetCurrentValue();
                    break;

                case CardType.Function:
                    if (card.Data.effectType == EffectType.ReduceRevenue)
                        trashCount++;
                    if (card.Data.effectType == EffectType.BonusIfNoTrash)
                        hasBonus = true;
                    break;
            }
        }

        // 垃圾卡扣减（每张-15%）
        float penalty = 1f - (trashCount * 0.15f);
        penalty = Mathf.Max(penalty, 0.1f); // 最多扣到10%

        float total = baseRevenue * penalty;

        // 五星认证加成（无垃圾时+25%）
        if (hasBonus && trashCount == 0)
            total *= 1.25f;

        // 提交后重置海鲜禁用
        player.SeafoodDisabled = false;

        return Mathf.Max(0, Mathf.RoundToInt(total));
    }
}
