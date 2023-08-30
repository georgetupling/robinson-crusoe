using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lose2Determination : CardEffect
{
    protected override void Initialize()
    {
        base.Initialize();
        targetType = TargetType.Player;
    }

    public override void ApplyEffect()
    {
        if (targetId == -1)
        {
            Debug.LogError("Lose2Determination target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseDeterminationEvent(targetId, 2);
    }
}
