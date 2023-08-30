using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustSourceGatheredFrom : CardEffect
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
            Debug.LogError("ExhaustSourceGatheredFrom target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseExhaustSourceByIslandTileIdEvent(targetId, sourceGatheredFrom, true);
    }
}
