using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustAllFoodOnTileGatheredFrom : CardEffect
{
    protected override void Initialize()
    {
        base.Initialize();
        targetType = TargetType.IslandTile;
    }

    public override void ApplyEffect()
    {
        if (targetId == -1)
        {
            Debug.LogError("ExhaustAllFoodOnTileGatheredFrom target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseExhaustSourceByIslandTileIdEvent(targetId, Source.Fish, true);
        EventGenerator.Singleton.RaiseExhaustSourceByIslandTileIdEvent(targetId, Source.Parrot, true);
    }
}
