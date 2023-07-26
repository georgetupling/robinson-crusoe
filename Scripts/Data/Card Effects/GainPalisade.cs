using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainPalisade : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("GainPalisade effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainPalisadeEvent(1);
        hasBeenApplied = true;
    }
}
