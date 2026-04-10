using UnityEngine;

public enum CardType { Ingredient, Utensil, Function }

public enum IngredientCategory { Meat, Seafood, Vegetable, Egg }

public enum FunctionType { Trash, Clean, Event }

public enum EffectType
{
    None,
    ReduceRevenue,        // 垃圾卡：提交时收益-15%
    RemoveTrash,          // 突击检查：所有玩家丢弃手中垃圾牌
    SpoilRandomCard,      // 粗心大意：目标玩家随机丢1张食材
    SkipTurn,             // 停水停电：目标玩家跳过下一回合
    StealCards,           // 食材求助：从牌堆抽2张食材到手中
    ReduceSeafoodValue,   // 冷链断裂：目标玩家本回合海鲜价值归零
    AllDrawIngredient,    // 紧急补货：所有玩家各抽2张食材
    BonusIfNoTrash        // 五星认证：桌上无垃圾则收益+25%
}

[CreateAssetMenu(fileName = "NewCard", menuName = "KitchenClash/Card")]
public class CardData : ScriptableObject
{
    [Header("通用")]
    public string cardId;
    public string cardName;
    [TextArea] public string description;
    public Sprite artwork;
    public CardType cardType;
    public int cardCount = 1;

    [Header("食材卡")]
    public IngredientCategory ingredientCategory;
    [Range(1, 3)] public int qualityLevel = 1;
    public int baseValue;

    [Header("功能卡")]
    public FunctionType functionType;
    public EffectType effectType;
    public float effectValue;
}
