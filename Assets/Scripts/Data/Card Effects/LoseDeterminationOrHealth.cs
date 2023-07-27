using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseDeterminationOrHealth : CardEffect
{   
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("LoseDeterminationOrHealth effect has already been applied.");
            return;
        }
        if (optionChosen == -1) {
            Debug.LogError("LoseDeterminationOrHealth optionChosen not set.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("LoseDeterminationOrHealth target not set.");
            return;
        }
        // 0 = determination, 1 = health
        if (optionChosen == 0) {
            EventGenerator.Singleton.RaiseLoseDeterminationEvent(targetId, 1);
        } else if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseLoseHealthEvent(targetId, 1);
        }
        hasBeenApplied = true;
    }
}
