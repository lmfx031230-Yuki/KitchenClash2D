using UnityEngine;
using UnityEditor;
using System.IO;

public class CardEditorWindow : EditorWindow
{
    [MenuItem("KitchenClash/Card Editor")]
    public static void Open() => GetWindow<CardEditorWindow>("Card Editor");

    private Vector2 _scroll;

    private void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        GUILayout.Label("Generate All Cards", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        if (GUILayout.Button("Generate All Cards (skip existing)", GUILayout.Height(35)))
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
        Debug.Log("[CardEditor] All cards generated!");
    }

    private void GenerateIngredients()
    {
        // Meat
        CreateIngredient("ING-001", "Pork Belly",    "Rich and juicy, perfect for braising",     IngredientCategory.Meat, 1, 10, 3);
        CreateIngredient("ING-002", "Chicken Breast","Lean and protein-rich",                     IngredientCategory.Meat, 1,  8, 3);
        CreateIngredient("ING-003", "Beef Tenderloin","Tender premium cut",                       IngredientCategory.Meat, 2, 18, 2);
        CreateIngredient("ING-004", "Lamb Chop",     "Distinctive aromatic flavor",               IngredientCategory.Meat, 2, 16, 2);
        CreateIngredient("ING-005", "Duck Leg",      "Crispy skin, tender meat",                  IngredientCategory.Meat, 1, 12, 2);
        CreateIngredient("ING-006", "Wagyu Beef",    "Top-grade marbled beef",                    IngredientCategory.Meat, 3, 30, 1);

        // Seafood
        CreateIngredient("ING-011", "Tiger Shrimp",  "Sweet and springy texture",                 IngredientCategory.Seafood, 1, 12, 3);
        CreateIngredient("ING-012", "Grass Carp",    "Classic home-style fish",                   IngredientCategory.Seafood, 1,  8, 3);
        CreateIngredient("ING-013", "Scallop",       "Rich umami flavor",                         IngredientCategory.Seafood, 2, 16, 2);
        CreateIngredient("ING-014", "Salmon",        "Rich in omega-3",                           IngredientCategory.Seafood, 2, 20, 2);
        CreateIngredient("ING-015", "King Crab",     "King of seafood",                           IngredientCategory.Seafood, 3, 35, 1);
        CreateIngredient("ING-016", "Squid",         "Chewy and flavorful",                       IngredientCategory.Seafood, 1, 10, 2);

        // Vegetable
        CreateIngredient("ING-021", "Napa Cabbage",  "Crisp and refreshing",                      IngredientCategory.Vegetable, 1,  4, 4);
        CreateIngredient("ING-022", "Potato",        "Versatile ingredient",                      IngredientCategory.Vegetable, 1,  5, 4);
        CreateIngredient("ING-023", "Shiitake",      "Deep savory aroma",                         IngredientCategory.Vegetable, 1,  7, 3);
        CreateIngredient("ING-024", "Broccoli",      "Nutritious and fresh",                      IngredientCategory.Vegetable, 2, 10, 2);
        CreateIngredient("ING-025", "Black Truffle", "King of mushrooms",                         IngredientCategory.Vegetable, 3, 28, 1);
        CreateIngredient("ING-026", "Tomato",        "Sweet and tangy",                           IngredientCategory.Vegetable, 1,  5, 3);

        // Egg
        CreateIngredient("ING-031", "Farm Egg",      "Nutritious free-range egg",                 IngredientCategory.Egg, 1,  5, 4);
        CreateIngredient("ING-032", "Quail Egg",     "Delicate and refined",                      IngredientCategory.Egg, 1,  6, 3);
        CreateIngredient("ING-033", "Duck Egg",      "Rich and savory",                           IngredientCategory.Egg, 1,  6, 3);
        CreateIngredient("ING-034", "Caviar Egg",    "Premium topping ingredient",                IngredientCategory.Egg, 3, 25, 1);
    }

    private void GenerateUtensils()
    {
        CreateUtensil("UTN-001", "Wok",           "All-purpose stir-fry pan", 3);
        CreateUtensil("UTN-002", "Steamer",       "Preserves natural flavors", 3);
        CreateUtensil("UTN-003", "Oven",          "Perfect for roasting", 2);
        CreateUtensil("UTN-004", "Clay Pot",      "Slow-cook for rich broth", 2);
        CreateUtensil("UTN-005", "Frying Pan",    "Pan-fry or deep-fry", 2);
        CreateUtensil("UTN-006", "Pressure Cooker","Fast cooking under pressure", 2);
    }

    private void GenerateFunctionCards()
    {
        // Trash
        CreateFunction("TRS-001", "Eggshell",     "Left on table: dish revenue -15%",             FunctionType.Trash, EffectType.ReduceRevenue, 15, 3);
        CreateFunction("TRS-002", "Fingernail",   "Left on table: dish revenue -15%",             FunctionType.Trash, EffectType.ReduceRevenue, 15, 3);
        CreateFunction("TRS-003", "Kitchen Waste","Left on table: dish revenue -15%",             FunctionType.Trash, EffectType.ReduceRevenue, 15, 2);

        // Clean
        CreateFunction("CLN-001", "Inspection",   "All players discard all trash cards",          FunctionType.Clean, EffectType.RemoveTrash, 0, 2);
        CreateFunction("CLN-002", "Deep Clean",   "All players discard all trash cards",          FunctionType.Clean, EffectType.RemoveTrash, 0, 2);

        // Event
        CreateFunction("EVT-001", "Carelessness", "Target player discards 1 random ingredient",  FunctionType.Event, EffectType.SpoilRandomCard,    0, 2);
        CreateFunction("EVT-002", "Power Outage", "Target player skips next turn",               FunctionType.Event, EffectType.SkipTurn,            0, 2);
        CreateFunction("EVT-003", "Supply Run",   "Draw 2 ingredient cards from deck",           FunctionType.Event, EffectType.StealCards,          2, 2);
        CreateFunction("EVT-004", "Cold Chain Fail","Target player's seafood worth $0 this turn",FunctionType.Event, EffectType.ReduceSeafoodValue,  0, 2);
        CreateFunction("EVT-005", "Emergency Restock","All players draw 2 ingredient cards",     FunctionType.Event, EffectType.AllDrawIngredient,   2, 1);
        CreateFunction("EVT-006", "Five Stars",   "No trash on table: revenue +25%",             FunctionType.Event, EffectType.BonusIfNoTrash,     25, 2);
    }

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
