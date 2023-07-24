using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainWood : CardEffect
{    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("GainWood effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainWoodEvent(1);
        hasBeenApplied = true;
    }
}
