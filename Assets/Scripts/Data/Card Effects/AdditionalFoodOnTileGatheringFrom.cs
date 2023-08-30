using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalFoodOnTileGatheringFrom : CardEffect
{
    protected override void Initialize()
    {
        base.Initialize();
        targetType = TargetType.Location;
    }

    public override void ApplyEffect()
    {
        if (targetId == -1)
        {
            Debug.LogError("AdditionalFoodOnTileGatheringFrom target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnIslandTileTokenEvent(TokenType.AdditionalFood, targetId);
    }
}
