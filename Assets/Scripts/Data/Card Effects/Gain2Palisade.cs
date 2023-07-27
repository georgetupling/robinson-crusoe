using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gain2Palisade : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Gain2Palisade effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainPalisadeEvent(2);
        hasBeenApplied = true;
    }
}
