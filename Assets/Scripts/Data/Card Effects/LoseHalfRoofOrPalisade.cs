using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseHalfRoofOrPalisade : CardEffect
{   
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("LoseHalfRoofOrPalisade effect has already been applied.");
            return;
        }
        if (optionChosen == -1) {
            Debug.LogError("LoseHalfRoofOrPalisade optionChosen not set.");
            return;
        }
        // 0 = lose half roof, 1 = lose half palisade
        if (optionChosen == 0) {
            EventGenerator.Singleton.RaiseLoseHalfRoofEvent();
        } else if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseLoseHalfPalisadeEvent();
        }
        hasBeenApplied = true;
    }
}
