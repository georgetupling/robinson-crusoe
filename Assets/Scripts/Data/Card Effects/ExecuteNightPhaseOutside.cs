using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteNightPhaseOutside : CardEffect
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
            Debug.LogError("ExecuteNightPhaseOutside target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSleepingOutsideEvent(targetId);
    }
}
