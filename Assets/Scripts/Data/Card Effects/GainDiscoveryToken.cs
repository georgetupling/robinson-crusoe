using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainDiscoveryToken : CardEffect
{   
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("GainDiscoveryToken effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseDrawDiscoveryTokenEvent(1);
        hasBeenApplied = true;
    }
}
