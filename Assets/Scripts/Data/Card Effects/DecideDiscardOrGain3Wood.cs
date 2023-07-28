using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideDiscardOrGain3Wood : CardEffect
{   
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("DecideDiscardOrGain3Wood effect has already been applied.");
            return;
        }
        if (optionChosen == -1) {
            Debug.LogError("DecideDiscardOrGain3Wood optionChosen not set.");
            return;
        }
        // 0 = discard card, 1 = gain 3 wood
        if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseGainWoodEvent(3);
        }
        hasBeenApplied = true;
    }
}
