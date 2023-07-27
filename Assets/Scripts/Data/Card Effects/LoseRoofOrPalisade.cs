using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseRoofOrPalisade : CardEffect
{   
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("LoseRoofOrPalisade effect has already been applied.");
            return;
        }
        if (optionChosen == -1) {
            Debug.LogError("LoseRoofOrPalisade optionChosen not set.");
            return;
        }
        // 0 = lose roof, 1 = lose palisade
        if (optionChosen == 0) {
            EventGenerator.Singleton.RaiseLoseRoofEvent(1);
        } else if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseLosePalisadeEvent(1);
        }
        hasBeenApplied = true;
    }
}
