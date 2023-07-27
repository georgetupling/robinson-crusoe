using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gain2Wood : CardEffect
{    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Gain2Wood effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainWoodEvent(2);
        hasBeenApplied = true;
    }
}
