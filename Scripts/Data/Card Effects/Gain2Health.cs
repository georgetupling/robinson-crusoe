using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gain2Health : CardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Gain2Health effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("Gain2Health target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseGainHealthEvent(targetId, 2);
        hasBeenApplied = true;
    }
}
