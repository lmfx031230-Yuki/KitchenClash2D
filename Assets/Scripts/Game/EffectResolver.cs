using UnityEngine;

/// <summary>
/// 功能卡效果执行器
/// </summary>
public static class EffectResolver
{
    public static void Resolve(CardInstance card, int playerIndex)
    {
        var players = GameManager.Instance.Players;
        var player  = players[playerIndex];
        int target  = GetOpponentIndex(playerIndex);

        switch (card.Data.effectType)
        {
            case EffectType.ReduceRevenue:
                // 垃圾卡：留在桌面，结算时自动扣分（RevenueCalculator处理）
                break;

            case EffectType.RemoveTrash:
                // 突击检查：所有玩家丢弃手中垃圾牌
                foreach (var p in players)
                {
                    var trash = p.Hand.GetCardsByType(CardType.Function);
                    foreach (var t in trash)
                        if (t.Data.functionType == FunctionType.Trash)
                            p.Hand.RemoveCard(t);
                }
                UIManager.Instance.ShowMessage("Inspection! All trash cards discarded!");
                break;

            case EffectType.SpoilRandomCard:
                // 粗心大意：目标玩家丢1张随机食材
                var targetIngredients = players[target].Hand.GetCardsByType(CardType.Ingredient);
                if (targetIngredients.Count > 0)
                {
                    var toRemove = targetIngredients[Random.Range(0, targetIngredients.Count)];
                    players[target].Hand.RemoveCard(toRemove);
                    UIManager.Instance.ShowMessage($"{players[target].PlayerName} lost an ingredient!");
                }
                break;

            case EffectType.SkipTurn:
                // 停水停电：目标玩家跳过下一回合
                players[target].SkipNextTurn = true;
                UIManager.Instance.ShowMessage($"{players[target].PlayerName} will skip next turn!");
                break;

            case EffectType.StealCards:
                // 食材求助：从牌堆抽2张食材
                for (int i = 0; i < 2; i++)
                {
                    var drawn = DeckManager.Instance.DrawCardOfType(CardType.Ingredient);
                    if (drawn != null) player.Hand.AddCard(drawn);
                }
                UIManager.Instance.ShowMessage($"{player.PlayerName} drew 2 ingredients!");
                break;

            case EffectType.ReduceSeafoodValue:
                // 冷链断裂：目标玩家海鲜本轮归零
                players[target].SeafoodDisabled = true;
                UIManager.Instance.ShowMessage($"{players[target].PlayerName}'s seafood is worth $0 this turn!");
                break;

            case EffectType.AllDrawIngredient:
                // 紧急补货：所有玩家各抽2张食材
                foreach (var p in players)
                    for (int i = 0; i < 2; i++)
                    {
                        var drawn = DeckManager.Instance.DrawCardOfType(CardType.Ingredient);
                        if (drawn != null) p.Hand.AddCard(drawn);
                    }
                UIManager.Instance.ShowMessage("Emergency restock! All players drew 2 ingredients!");
                break;

            case EffectType.BonusIfNoTrash:
                // 五星认证：留在桌面，结算时处理
                break;
        }
    }

    /// <summary>获取当前玩家的对手（对方队伍中随机一个）</summary>
    private static int GetOpponentIndex(int playerIndex)
    {
        int myTeam = GameManager.Instance.Players[playerIndex].TeamId;
        var opponents = new System.Collections.Generic.List<int>();
        var players = GameManager.Instance.Players;
        for (int i = 0; i < players.Length; i++)
            if (players[i].TeamId != myTeam)
                opponents.Add(i);
        return opponents[Random.Range(0, opponents.Count)];
    }
}
