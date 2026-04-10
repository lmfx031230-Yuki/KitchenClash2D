using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnPhase { Draw, Play, Submit, End }

/// <summary>
/// 回合状态机：驱动4名玩家轮流行动
/// </summary>
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [Header("AI出牌延迟（秒）")]
    [SerializeField] private float aiDelay = 1.0f;

    public int CurrentPlayerIndex { get; private set; } = 0;
    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Draw;
    public bool IsLocalPlayerTurn => CurrentPlayerIndex == 0;

    // 当前回合桌面上的牌
    private List<CardInstance> _tableCards = new List<CardInstance>();
    public IReadOnlyList<CardInstance> TableCards => _tableCards;

    // 当前玩家是否已摸牌
    private bool _hasDrawnThisPhase = false;

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

    // ── 回合开始 ──────────────────────────────────────────────────────────────

    private void StartTurn()
    {
        var player = GameManager.Instance.Players[CurrentPlayerIndex];

        // 跳过回合处理
        if (player.SkipNextTurn)
        {
            player.SkipNextTurn = false;
            UIManager.Instance.ShowMessage($"{player.PlayerName} 跳过本回合");
            EndTurn();
            return;
        }

        _tableCards.Clear();
        TableView.Instance?.RefreshTable(_tableCards);

        UIManager.Instance.RefreshRoundInfo(
            GameManager.Instance.CurrentRound,
            GameManager.Instance.TotalRounds,
            player.PlayerName);

        EnterDrawPhase();
    }

    // ── 摸牌阶段 ──────────────────────────────────────────────────────────────

    private void EnterDrawPhase()
    {
        CurrentPhase = TurnPhase.Draw;
        _hasDrawnThisPhase = false;

        var player = GameManager.Instance.Players[CurrentPlayerIndex];

        // 摸一张牌
        var card = DeckManager.Instance.DrawCard();
        if (card != null)
        {
            player.Hand.AddCard(card);
            UIManager.Instance.ShowMessage($"{player.PlayerName} 摸了一张牌");
        }

        EnterPlayPhase();
    }

    // ── 出牌阶段 ──────────────────────────────────────────────────────────────

    private void EnterPlayPhase()
    {
        CurrentPhase = TurnPhase.Play;

        if (IsLocalPlayerTurn)
        {
            UIManager.Instance.ShowMessage("请出牌，或点击[结束出牌]");
            GameUI.Instance?.SetPlayButtons(true);
        }
        else
        {
            GameUI.Instance?.SetPlayButtons(false);
            StartCoroutine(AIPlayRoutine());
        }
    }

    /// <summary>玩家点击"打出选中卡"按钮</summary>
    public void PlayerPlayCard()
    {
        if (!IsLocalPlayerTurn || CurrentPhase != TurnPhase.Play) return;

        var card = HandView.Instance.ConsumeSelected();
        if (card == null)
        {
            UIManager.Instance.ShowMessage("请先选择一张牌");
            return;
        }

        PlayCardToTable(GameManager.Instance.LocalPlayer, card);
    }

    /// <summary>玩家点击"结束出牌"按钮</summary>
    public void PlayerEndPlay()
    {
        if (!IsLocalPlayerTurn || CurrentPhase != TurnPhase.Play) return;
        EnterSubmitPhase();
    }

    private void PlayCardToTable(PlayerAgent player, CardInstance card)
    {
        player.Hand.RemoveCard(card);
        _tableCards.Add(card);
        TableView.Instance?.RefreshTable(_tableCards);
        UIManager.Instance.ShowMessage($"打出：{card.Data.cardName}");
    }

    // ── AI出牌 ────────────────────────────────────────────────────────────────

    private IEnumerator AIPlayRoutine()
    {
        yield return new WaitForSeconds(aiDelay);

        var player = GameManager.Instance.Players[CurrentPlayerIndex];
        var hand = player.Hand;

        var utensils    = hand.GetCardsByType(CardType.Utensil);
        var ingredients = hand.GetCardsByType(CardType.Ingredient);
        var trashCards  = hand.GetCardsByType(CardType.Function);

        bool submitted = false;

        // 有餐具且有食材 → 提交菜品
        if (utensils.Count > 0 && ingredients.Count >= 1)
        {
            // 打出餐具
            PlayCardToTable(player, utensils[0]);
            yield return new WaitForSeconds(aiDelay * 0.5f);

            // 打出所有食材（最多4张）
            int count = Mathf.Min(ingredients.Count, 4);
            for (int i = 0; i < count; i++)
            {
                // 重新获取，因为手牌已变化
                var ing = player.Hand.GetCardsByType(CardType.Ingredient);
                if (ing.Count == 0) break;
                PlayCardToTable(player, ing[0]);
                yield return new WaitForSeconds(aiDelay * 0.3f);
            }
            submitted = true;
        }
        else if (trashCards.Count > 0)
        {
            // 打出垃圾/功能卡
            PlayCardToTable(player, trashCards[0]);
        }
        else if (ingredients.Count > 0)
        {
            // 随机打出一张食材
            PlayCardToTable(player, ingredients[Random.Range(0, ingredients.Count)]);
        }

        yield return new WaitForSeconds(aiDelay * 0.5f);

        if (submitted)
            SubmitDish();
        else
            EndTurn();
    }

    // ── 提交阶段 ──────────────────────────────────────────────────────────────

    private void EnterSubmitPhase()
    {
        CurrentPhase = TurnPhase.Submit;

        if (DishValidator.CanSubmit(_tableCards))
        {
            GameUI.Instance?.SetSubmitButton(true);
            UIManager.Instance.ShowMessage("可以提交菜品！");
        }
        else
        {
            GameUI.Instance?.SetSubmitButton(false);
            UIManager.Instance.ShowMessage("桌上没有合法菜品，结束回合");
            EndTurn();
        }
    }

    /// <summary>玩家点击"提交菜品"按钮</summary>
    public void PlayerSubmitDish()
    {
        if (CurrentPhase != TurnPhase.Submit) return;
        SubmitDish();
    }

    private void SubmitDish()
    {
        var player = GameManager.Instance.Players[CurrentPlayerIndex];
        int revenue = RevenueCalculator.Calculate(_tableCards, player);
        player.Revenue += revenue;

        UIManager.Instance.ShowMessage($"{player.PlayerName} 提交菜品！获得 ¥{revenue}", 2.5f);
        UIManager.Instance.RefreshRevenue(GameManager.Instance.Players);

        _tableCards.Clear();
        TableView.Instance?.RefreshTable(_tableCards);

        EndTurn();
    }

    // ── 回合结束 ──────────────────────────────────────────────────────────────

    private void EndTurn()
    {
        GameUI.Instance?.SetPlayButtons(false);
        GameUI.Instance?.SetSubmitButton(false);

        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % 4;

        // 每4人一圈算一轮
        if (CurrentPlayerIndex == 0)
            GameManager.Instance.OnRoundEnd();

        if (GameManager.Instance.CurrentRound <= GameManager.Instance.TotalRounds)
            StartTurn();
    }
}
