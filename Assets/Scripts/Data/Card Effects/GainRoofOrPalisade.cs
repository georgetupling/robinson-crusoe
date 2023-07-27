using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainRoofOrPalisade : CardEffect
{   
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("GainRoofOrPalisade effect has already been applied.");
            return;
        }
        if (optionChosen == -1) {
            Debug.LogError("GainRoofOrPalisade optionChosen not set.");
            return;
        }
        // 0 = gain roof, 1 = gain palisade
        if (optionChosen == 0) {
            EventGenerator.Singleton.RaiseGainRoofEvent(1);
        } else if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseGainPalisadeEvent(1);
        }
        hasBeenApplied = true;
    }
}
