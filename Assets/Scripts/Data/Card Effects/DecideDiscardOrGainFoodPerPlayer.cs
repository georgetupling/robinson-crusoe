using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideDiscardOrGainFoodPerPlayer : CardEffect
{   
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("DecideDiscardOrGainFoodPerPlayer effect has already been applied.");
            return;
        }
        if (optionChosen == -1) {
            Debug.LogError("DecideDiscardOrGainFoodPerPlayer optionChosen not set.");
            return;
        }
        // 0 = discard card, 1 = gain 1 food per player
        if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseGainFoodEvent(GameSettings.PlayerCount);
        }
        hasBeenApplied = true;
    }
}
