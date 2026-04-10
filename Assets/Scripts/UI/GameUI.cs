using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理游戏中的按钮：打出选中牌、结束出牌、提交菜品
/// </summary>
public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    [Header("按钮")]
    [SerializeField] private Button playCardButton;    // 打出选中的牌
    [SerializeField] private Button endPlayButton;     // 结束出牌
    [SerializeField] private Button submitDishButton;  // 提交菜品

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        playCardButton.onClick.AddListener(TurnManager.Instance.PlayerPlayCard);
        endPlayButton.onClick.AddListener(TurnManager.Instance.PlayerEndPlay);
        submitDishButton.onClick.AddListener(TurnManager.Instance.PlayerSubmitDish);

        SetPlayButtons(false);
        SetSubmitButton(false);
    }

    public void SetPlayButtons(bool active)
    {
        playCardButton.interactable = active;
        endPlayButton.interactable  = active;
    }

    public void SetSubmitButton(bool active)
    {
        submitDishButton.interactable = active;
    }
}
