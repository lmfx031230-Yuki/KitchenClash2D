using System;

/// <summary>
/// 运行时卡牌实例，每张实物卡对应一个CardInstance
/// </summary>
public class CardInstance
{
    public CardData Data { get; private set; }
    public string InstanceId { get; private set; }

    // 运行时可变状态
    public int CurrentQuality { get; set; }
    public bool IsDisabled { get; set; }

    public CardInstance(CardData data)
    {
        Data = data;
        InstanceId = Guid.NewGuid().ToString();
        CurrentQuality = data.qualityLevel;
        IsDisabled = false;
    }

    public int GetCurrentValue()
    {
        if (Data.cardType != CardType.Ingredient) return 0;
        float multiplier = CurrentQuality switch
        {
            1 => 1.0f,
            2 => 1.5f,
            3 => 2.0f,
            _ => 1.0f
        };
        return Mathf.RoundToInt(Data.baseValue * multiplier);
    }
}
