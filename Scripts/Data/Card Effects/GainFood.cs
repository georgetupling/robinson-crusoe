using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainFood : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("GainFood effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainFoodEvent(1);
        hasBeenApplied = true;
    }
}
