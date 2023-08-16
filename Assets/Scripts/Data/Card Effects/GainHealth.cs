using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainHealth : CardEffect
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
            Debug.LogError("GainHealth target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseGainHealthEvent(targetId, 1);
    }
}
