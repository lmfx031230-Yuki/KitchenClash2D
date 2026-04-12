using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnPhase { Draw, Play, Submit, End }

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] private float aiDelay = 1.0f;

    public int CurrentPlayerIndex { get; private set; } = 0;
    public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Draw;
    public bool IsLocalPlayerTurn => CurrentPlayerIndex == 0;

    private List<CardInstance> _tableCards = new List<CardInstance>();
    public IReadOnlyList<CardInstance> TableCards => _tableCards;

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

    private void StartTurn()
    {
        var player = GameManager.Instance.Players[CurrentPlayerIndex];

        if (player.SkipNextTurn)
        {
            player.SkipNextTurn = false;
            UIManager.Instance.ShowMessage($"{player.PlayerName} skipped turn");
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

    private void EnterDrawPhase()
    {
        CurrentPhase = TurnPhase.Draw;

        var player = GameManager.Instance.Players[CurrentPlayerIndex];

        var card = DeckManager.Instance.DrawCard();
        if (card != null)
        {
            player.Hand.AddCard(card);
            UIManager.Instance.ShowMessage($"{player.PlayerName} drew a card");
        }

        EnterPlayPhase();
    }

    private void EnterPlayPhase()
    {
        CurrentPhase = TurnPhase.Play;

        if (IsLocalPlayerTurn)
        {
            UIManager.Instance.ShowMessage("Play a card or click End Play");
            GameUI.Instance?.SetPlayButtons(true);
        }
        else
        {
            GameUI.Instance?.SetPlayButtons(false);
            StartCoroutine(AIPlayRoutine());
        }
    }

    public void PlayerPlayCard()
    {
        if (!IsLocalPlayerTurn || CurrentPhase != TurnPhase.Play) return;

        var card = HandView.Instance.ConsumeSelected();
        if (card == null)
        {
            UIManager.Instance.ShowMessage("Select a card first");
            return;
        }

        PlayCardToTable(GameManager.Instance.LocalPlayer, card);
    }

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
        UIManager.Instance.ShowMessage($"Played: {card.Data.cardName}");
    }

    private IEnumerator AIPlayRoutine()
    {
        yield return new WaitForSeconds(aiDelay);

        var player = GameManager.Instance.Players[CurrentPlayerIndex];
        var hand = player.Hand;

        var utensils    = hand.GetCardsByType(CardType.Utensil);
        var ingredients = hand.GetCardsByType(CardType.Ingredient);
        var trashCards  = hand.GetCardsByType(CardType.Function);

        bool submitted = false;

        if (utensils.Count > 0 && ingredients.Count >= 1)
        {
            PlayCardToTable(player, utensils[0]);
            yield return new WaitForSeconds(aiDelay * 0.5f);

            int count = Mathf.Min(ingredients.Count, 4);
            for (int i = 0; i < count; i++)
            {
                var ing = player.Hand.GetCardsByType(CardType.Ingredient);
                if (ing.Count == 0) break;
                PlayCardToTable(player, ing[0]);
                yield return new WaitForSeconds(aiDelay * 0.3f);
            }
            submitted = true;
        }
        else if (trashCards.Count > 0)
        {
            PlayCardToTable(player, trashCards[0]);
        }
        else if (ingredients.Count > 0)
        {
            PlayCardToTable(player, ingredients[Random.Range(0, ingredients.Count)]);
        }

        yield return new WaitForSeconds(aiDelay * 0.5f);

        if (submitted)
            SubmitDish();
        else
            EndTurn();
    }

    private void EnterSubmitPhase()
    {
        CurrentPhase = TurnPhase.Submit;

        if (DishValidator.CanSubmit(_tableCards))
        {
            GameUI.Instance?.SetSubmitButton(true);
            UIManager.Instance.ShowMessage("Ready to submit dish!");
        }
        else
        {
            GameUI.Instance?.SetSubmitButton(false);
            UIManager.Instance.ShowMessage("No valid dish, ending turn");
            EndTurn();
        }
    }

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

        UIManager.Instance.ShowMessage($"{player.PlayerName} submitted! +{revenue}", 2.5f);
        UIManager.Instance.RefreshRevenue(GameManager.Instance.Players);

        _tableCards.Clear();
        TableView.Instance?.RefreshTable(_tableCards);

        EndTurn();
    }

    private void EndTurn()
    {
        GameUI.Instance?.SetPlayButtons(false);
        GameUI.Instance?.SetSubmitButton(false);

        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % 4;

        if (CurrentPlayerIndex == 0)
            GameManager.Instance.OnRoundEnd();

        if (GameManager.Instance.CurrentRound <= GameManager.Instance.TotalRounds)
            StartTurn();
    }
}
