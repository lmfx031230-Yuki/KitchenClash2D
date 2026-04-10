using UnityEngine;

/// <summary>
/// 回合状态机 — Day 4 实现，此处为占位让代码先编译
/// </summary>
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public bool IsLocalPlayerTurn { get; private set; } = true;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartGame()
    {
        Debug.Log("[TurnManager] 游戏开始（占位）");
    }
}
