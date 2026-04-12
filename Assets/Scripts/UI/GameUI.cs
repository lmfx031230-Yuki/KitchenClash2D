using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏中的两个按钮：Draw Card / Play Card
/// </summary>
public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    [SerializeField] private Button drawCardButton;
    [SerializeField] private Button playCardButton;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        drawCardButton.onClick.AddListener(TurnManager.Instance.PlayerDrawCard);
        playCardButton.onClick.AddListener(TurnManager.Instance.PlayerPlayCard);
        SetActionButtons(false);
    }

    public void SetActionButtons(bool active)
    {
        drawCardButton.interactable = active;
        playCardButton.interactable = active;
    }

    // 兼容旧调用
    public void SetPlayButtons(bool active) => SetActionButtons(active);
    public void SetSubmitButton(bool active) { }
}
