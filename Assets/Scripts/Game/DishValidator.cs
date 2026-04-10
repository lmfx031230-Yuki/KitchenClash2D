using System.Collections.Generic;

/// <summary>
/// 判断桌面上的牌是否构成合法菜品
/// 合法条件：至少1张餐具 + 至少1张食材
/// </summary>
public static class DishValidator
{
    public static bool CanSubmit(IReadOnlyList<CardInstance> tableCards)
    {
        bool hasUtensil    = false;
        bool hasIngredient = false;

        foreach (var card in tableCards)
        {
            if (card.Data.cardType == CardType.Utensil)    hasUtensil    = true;
            if (card.Data.cardType == CardType.Ingredient) hasIngredient = true;
        }

        return hasUtensil && hasIngredient;
    }
}
