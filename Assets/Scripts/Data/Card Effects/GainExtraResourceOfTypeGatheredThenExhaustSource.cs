using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainExtraResourceOfTypeGatheredThenExhaustSource : CardEffect
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
            Debug.LogError("GainExtraResourceOfTypeGatheredThenExhaustSource target not set.");
            return;
        }
        if (sourceGatheredFrom == Source.Fish || sourceGatheredFrom == Source.Parrot)
        {
            EventGenerator.Singleton.RaiseGainFoodEvent(1);
        }
        else
        {
            EventGenerator.Singleton.RaiseGainWoodEvent(1);
        }
        EventGenerator.Singleton.RaiseExhaustSourceByIslandTileIdEvent(targetId, sourceGatheredFrom, true);
    }
}
