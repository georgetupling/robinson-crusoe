using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideDiscardOrGet2FoodAnd2Hide : CardEffect
{   
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("DecideDiscardOrGet2FoodAnd2Hide effect has already been applied.");
            return;
        }
        if (optionChosen == -1) {
            Debug.LogError("DecideDiscardOrGet2FoodAnd2Hide optionChosen not set.");
            return;
        }
        // 0 = discard card, 1 = gain 2 food and 2 hide
        if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseGainFoodEvent(2);
            EventGenerator.Singleton.RaiseGainHideEvent(2);
        }
        hasBeenApplied = true;
    }
}
