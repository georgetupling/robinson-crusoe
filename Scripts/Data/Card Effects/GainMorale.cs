using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainMorale : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("GainMorale effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainMoraleEvent(1);
        hasBeenApplied = true;
    }
}
