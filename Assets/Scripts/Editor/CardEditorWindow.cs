using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 批量生成CardData ScriptableObject的编辑器工具
/// 菜单：KitchenClash > Card Editor
/// </summary>
public class CardEditorWindow : EditorWindow
{
    [MenuItem("KitchenClash/Card Editor")]
    public static void Open() => GetWindow<CardEditorWindow>("Card Editor");

    private Vector2 _scroll;

    private void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        GUILayout.Label("批量生成卡牌", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("生成所有卡牌（会跳过已存在的）", GUILayout.Height(35)))
            GenerateAllCards();

        EditorGUILayout.EndScrollView();
    }

    private void GenerateAllCards()
    {
        GenerateIngredients();
        GenerateUtensils();
        GenerateFunctionCards();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[CardEditor] 所有卡牌生成完毕！");
    }

    // ─── 食材卡 ────────────────────────────────────────────────────────────────

    private void GenerateIngredients()
    {
        // 肉类
        CreateIngredient("ING-001", "猪五花", "肥瘦相间，香气浓郁", IngredientCategory.Meat, 1, 10, 3);
        CreateIngredient("ING-002", "鸡胸肉", "低脂高蛋白", IngredientCategory.Meat, 1, 8, 3);
        CreateIngredient("ING-003", "牛里脊", "嫩滑高档", IngredientCategory.Meat, 2, 18, 2);
        CreateIngredient("ING-004", "羊排", "香气独特", IngredientCategory.Meat, 2, 16, 2);
        CreateIngredient("ING-005", "鸭腿", "皮脆肉嫩", IngredientCategory.Meat, 1, 12, 2);
        CreateIngredient("ING-006", "和牛", "顶级食材，大理石纹", IngredientCategory.Meat, 3, 30, 1);

        // 海鲜
        CreateIngredient("ING-011", "基围虾", "鲜甜弹牙", IngredientCategory.Seafood, 1, 12, 3);
        CreateIngredient("ING-012", "草鱼", "家常美味", IngredientCategory.Seafood, 1, 8, 3);
        CreateIngredient("ING-013", "扇贝", "鲜味十足", IngredientCategory.Seafood, 2, 16, 2);
        CreateIngredient("ING-014", "三文鱼", "富含omega-3", IngredientCategory.Seafood, 2, 20, 2);
        CreateIngredient("ING-015", "帝王蟹", "海鲜之王", IngredientCategory.Seafood, 3, 35, 1);
        CreateIngredient("ING-016", "鱿鱼", "口感Q弹", IngredientCategory.Seafood, 1, 10, 2);

        // 蔬菜
        CreateIngredient("ING-021", "白菜", "清脆爽口", IngredientCategory.Vegetable, 1, 4, 4);
        CreateIngredient("ING-022", "土豆", "百搭食材", IngredientCategory.Vegetable, 1, 5, 4);
        CreateIngredient("ING-023", "香菇", "鲜味浓郁", IngredientCategory.Vegetable, 1, 7, 3);
        CreateIngredient("ING-024", "西兰花", "营养丰富", IngredientCategory.Vegetable, 2, 10, 2);
        CreateIngredient("ING-025", "松露", "菌中之王", IngredientCategory.Vegetable, 3, 28, 1);
        CreateIngredient("ING-026", "番茄", "酸甜可口", IngredientCategory.Vegetable, 1, 5, 3);

        // 蛋类
        CreateIngredient("ING-031", "土鸡蛋", "营养丰富", IngredientCategory.Egg, 1, 5, 4);
        CreateIngredient("ING-032", "鹌鹑蛋", "精致小巧", IngredientCategory.Egg, 1, 6, 3);
        CreateIngredient("ING-033", "鸭蛋", "咸香浓郁", IngredientCategory.Egg, 1, 6, 3);
        CreateIngredient("ING-034", "鱼子酱蛋", "顶级配料", IngredientCategory.Egg, 3, 25, 1);
    }

    // ─── 餐具卡 ────────────────────────────────────────────────────────────────

    private void GenerateUtensils()
    {
        CreateUtensil("UTN-001", "炒锅", "万能炒锅，适合各类食材", 3);
        CreateUtensil("UTN-002", "蒸锅", "保留食材原汁原味", 3);
        CreateUtensil("UTN-003", "烤箱", "烤制香脆外皮", 2);
        CreateUtensil("UTN-004", "砂锅", "慢炖出浓郁汤汁", 2);
        CreateUtensil("UTN-005", "平底锅", "煎炸两用", 2);
        CreateUtensil("UTN-006", "高压锅", "快速烹饪", 2);
    }

    // ─── 功能卡 ────────────────────────────────────────────────────────────────

    private void GenerateFunctionCards()
    {
        // 垃圾卡
        CreateFunction("TRS-001", "蛋壳", "遗留桌面，提交时收益-15%", FunctionType.Trash, EffectType.ReduceRevenue, 15, 3);
        CreateFunction("TRS-002", "指甲", "遗留桌面，提交时收益-15%", FunctionType.Trash, EffectType.ReduceRevenue, 15, 3);
        CreateFunction("TRS-003", "厨余垃圾", "遗留桌面，提交时收益-15%", FunctionType.Trash, EffectType.ReduceRevenue, 15, 2);

        // 清洁卡
        CreateFunction("CLN-001", "突击检查", "所有玩家丢弃手中全部垃圾牌", FunctionType.Clean, EffectType.RemoveTrash, 0, 2);
        CreateFunction("CLN-002", "卫生大扫除", "所有玩家丢弃手中全部垃圾牌", FunctionType.Clean, EffectType.RemoveTrash, 0, 2);

        // 事件卡
        CreateFunction("EVT-001", "粗心大意", "目标玩家随机丢弃1张食材牌", FunctionType.Event, EffectType.SpoilRandomCard, 0, 2);
        CreateFunction("EVT-002", "停水停电", "目标玩家跳过下一回合", FunctionType.Event, EffectType.SkipTurn, 0, 2);
        CreateFunction("EVT-003", "食材求助", "从牌堆抽2张食材牌到手中", FunctionType.Event, EffectType.StealCards, 2, 2);
        CreateFunction("EVT-004", "冷链断裂", "目标玩家本回合海鲜食材价值归零", FunctionType.Event, EffectType.ReduceSeafoodValue, 0, 2);
        CreateFunction("EVT-005", "紧急补货", "所有玩家各抽2张食材牌", FunctionType.Event, EffectType.AllDrawIngredient, 2, 1);
        CreateFunction("EVT-006", "五星认证", "本回合提交时桌上无垃圾则收益+25%", FunctionType.Event, EffectType.BonusIfNoTrash, 25, 2);
    }

    // ─── 工具方法 ──────────────────────────────────────────────────────────────

    private void CreateIngredient(string id, string name, string desc,
        IngredientCategory category, int quality, int baseValue, int count)
    {
        var card = CreateOrLoad<CardData>($"Assets/Data/Cards/{id}.asset");
        card.cardId = id;
        card.cardName = name;
        card.description = desc;
        card.cardType = CardType.Ingredient;
        card.ingredientCategory = category;
        card.qualityLevel = quality;
        card.baseValue = baseValue;
        card.cardCount = count;
        EditorUtility.SetDirty(card);
    }

    private void CreateUtensil(string id, string name, string desc, int count)
    {
        var card = CreateOrLoad<CardData>($"Assets/Data/Cards/{id}.asset");
        card.cardId = id;
        card.cardName = name;
        card.description = desc;
        card.cardType = CardType.Utensil;
        card.cardCount = count;
        EditorUtility.SetDirty(card);
    }

    private void CreateFunction(string id, string name, string desc,
        FunctionType funcType, EffectType effect, float effectVal, int count)
    {
        var card = CreateOrLoad<CardData>($"Assets/Data/Cards/{id}.asset");
        card.cardId = id;
        card.cardName = name;
        card.description = desc;
        card.cardType = CardType.Function;
        card.functionType = funcType;
        card.effectType = effect;
        card.effectValue = effectVal;
        card.cardCount = count;
        EditorUtility.SetDirty(card);
    }

    private T CreateOrLoad<T>(string path) where T : ScriptableObject
    {
        var existing = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existing != null) return existing;

        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        var asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }
}
