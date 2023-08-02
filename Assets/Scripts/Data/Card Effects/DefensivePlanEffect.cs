using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensivePlanEffect : CardEffect
{   
    public override void ApplyEffect() {
        if (optionChosen == -1) {
            Debug.LogError("DefensivePlanEffect optionChosen not set.");
            return;
        }
        // 0 = cancel, 1 = increase palisade, 2 = increase weapon
        if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseGainPalisadeEvent(1);
        } else if (optionChosen == 2) {
            EventGenerator.Singleton.RaiseGainWeaponEvent(1);
        }
    }
}
