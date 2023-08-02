using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireplaceEffect : CardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
    }
    
    public override void ApplyEffect() {
        if (targetId == -1) {
            Debug.LogError("FireplaceEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseFoodEvent(1);
        EventGenerator.Singleton.RaiseGainHealthEvent(targetId, 2);
    }
}
