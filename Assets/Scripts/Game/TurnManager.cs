using System.Collections;
using UnityEngine;

public enum TurnPhase { ChooseAction, Done }

/// <summary>
/// 回合状态机：每回合只能摸牌或打牌，二选一
/// 打出餐具牌 = 直接上菜结算
/// </summary>
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] private float aiDelay = 1.0f;

    public int CurrentPlayerIndex { get; private set; } = 0;
    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.ChooseAction;
    public bool IsLocalPlayerTurn => CurrentPlayerIndex == 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartGame()
    {
        CurrentPlayerIndex = 0;
        StartTurn();
    }

    // ── 回合开始 ────────────────────────────────────────────────────────────────

    private void StartTurn()
    {
        var player = GameManager.Instance.Players[CurrentPlayerIndex];

        if (player.SkipNextTurn)
        {
            player.SkipNextTurn = false;
            UIManager.Instance.ShowMessage($"{player.PlayerName} skipped their turn");
            EndTurn();
            return;
        }

        CurrentPhase = TurnPhase.ChooseAction;

        UIManager.Instance.RefreshRoundInfo(
            GameManager.Instance.CurrentRound,
            GameManager.Instance.TotalRounds,
            player.PlayerName);

        // 手牌超限罚款（>12张）
        if (player.Hand.Count > 12)
        {
            int penalty = (player.Hand.Count - 12) * 3;
            player.Revenue -= penalty;
            UIManager.Instance.ShowMessage($"{player.PlayerName} over hand limit! -{penalty}", 2f);
        }

        if (IsLocalPlayerTurn)
        {
            UIManager.Instance.ShowMessage("Draw a card or play a card");
            GameUI.Instance?.SetActionButtons(true);
        }
        else
        {
            GameUI.Instance?.SetActionButtons(false);
            StartCoroutine(AITurnRoutine());
        }
    }

    // ── 玩家行动 ────────────────────────────────────────────────────────────────

    /// <summary>玩家点"Draw Card"按钮</summary>
    public void PlayerDrawCard()
    {
        if (!IsLocalPlayerTurn || CurrentPhase != TurnPhase.ChooseAction) return;

        var player = GameManager.Instance.LocalPlayer;
        var card = DeckManager.Instance.DrawCard();
        if (card != null)
        {
            player.Hand.AddCard(card);
            UIManager.Instance.ShowMessage($"Drew: {card.Data.cardName}");
        }
        else
        {
            UIManager.Instance.ShowMessage("Deck is empty!");
        }

        EndTurn();
    }

    /// <summary>玩家点"Play Card"按钮</summary>
    public void PlayerPlayCard()
    {
        if (!IsLocalPlayerTurn || CurrentPhase != TurnPhase.ChooseAction) return;

        var card = HandView.Instance.ConsumeSelected();
        if (card == null)
        {
            UIManager.Instance.ShowMessage("Select a card first");
            return;
        }

        PlayCard(GameManager.Instance.LocalPlayer, card);
    }

    private void PlayCard(PlayerAgent player, CardInstance card)
    {
        player.Hand.RemoveCard(card);

        // 打出餐具牌 = 上菜
        if (card.Data.cardType == CardType.Utensil)
        {
            if (!TableManager.Instance.HasIngredient)
            {
                // 没有食材，餐具牌无效，放回手里
                player.Hand.AddCard(card);
                UIManager.Instance.ShowMessage("No ingredients on table to serve!");
                return;
            }

            TableManager.Instance.PlayCard(card, player.PlayerId);
            UIManager.Instance.ShowMessage($"{player.PlayerName} served the dish!");
            RevenueCalculator.Calculate(player.PlayerId);
            TableManager.Instance.ClearTable();
            EndTurn();
            return;
        }

        // 功能卡或食材卡 → 放到桌面
        TableManager.Instance.PlayCard(card, player.PlayerId);

        // 功能卡立即触发效果
        if (card.Data.cardType == CardType.Function)
            EffectResolver.Resolve(card, player.PlayerId);

        UIManager.Instance.ShowMessage($"Played: {card.Data.cardName}");
        EndTurn();
    }

    // ── AI回合 ──────────────────────────────────────────────────────────────────

    private IEnumerator AITurnRoutine()
    {
        yield return new WaitForSeconds(aiDelay);

        var player = GameManager.Instance.Players[CurrentPlayerIndex];
        var hand   = player.Hand;

        var utensils    = hand.GetCardsByType(CardType.Utensil);
        var ingredients = hand.GetCardsByType(CardType.Ingredient);
        var functions   = hand.GetCardsByType(CardType.Function);

        // 桌面有食材且有餐具 → 上菜
        if (utensils.Count > 0 && TableManager.Instance.HasIngredient)
        {
            PlayCard(player, utensils[0]);
            yield break;
        }

        // 有食材 → 放食材到桌面
        if (ingredients.Count > 0)
        {
            PlayCard(player, ingredients[Random.Range(0, ingredients.Count)]);
            yield break;
        }

        // 有功能卡 → 打功能卡
        if (functions.Count > 0)
        {
            PlayCard(player, functions[0]);
            yield break;
        }

        // 什么都没有 → 摸牌
        var card = DeckManager.Instance.DrawCard();
        if (card != null)
        {
            player.Hand.AddCard(card);
            UIManager.Instance.ShowMessage($"{player.PlayerName} drew a card");
        }

        yield return new WaitForSeconds(aiDelay * 0.5f);
        EndTurn();
    }

    // ── 回合结束 ────────────────────────────────────────────────────────────────

    private void EndTurn()
    {
        GameUI.Instance?.SetActionButtons(false);
        CurrentPhase = TurnPhase.Done;

        // 推进腐烂计时
        TableManager.Instance.AdvanceRot();

        // 强制结算检查
        if (TableManager.Instance.ShouldForceSettle && TableManager.Instance.HasIngredient)
        {
            UIManager.Instance.ShowMessage("Dish spoiled! Force settling...", 2f);
            RevenueCalculator.Calculate(CurrentPlayerIndex);
            TableManager.Instance.ClearTable();
        }

        // 更新腐烂提示
        string rotMsg = TableManager.Instance.GetRotStatus();
        if (!string.IsNullOrEmpty(rotMsg))
            UIManager.Instance.ShowMessage(rotMsg, 2f);

        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % 4;

        if (CurrentPlayerIndex == 0)
            GameManager.Instance.OnRoundEnd();

        if (!GameManager.Instance.IsGameOver)
            StartTurn();
    }
}
