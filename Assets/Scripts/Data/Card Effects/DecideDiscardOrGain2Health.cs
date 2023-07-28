using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideDiscardOrGain2Health : CardEffect
{   
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("DecideDiscardOrGain2Health effect has already been applied.");
            return;
        }
        if (optionChosen == -1) {
            Debug.LogError("DecideDiscardOrGain2Health optionChosen not set.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("DecideDiscardOrGain2Health target not set.");
            return;
        }
        // 0 = discard, 1 = gain 2 health
        if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseGainHealthEvent(targetId, 2);
        }
        hasBeenApplied = true;
    }
}
