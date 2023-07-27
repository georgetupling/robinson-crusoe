using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseHealth : CardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("LoseHealth effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("LoseHealth target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseHealthEvent(targetId, 1);
        hasBeenApplied = true;
    }
}
