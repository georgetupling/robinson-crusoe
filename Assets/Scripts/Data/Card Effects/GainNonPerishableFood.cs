using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainNonPerishableFood : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("GainNonPerishableFood effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainNonPerishableFoodEvent(1);
        hasBeenApplied = true;
    }
}
