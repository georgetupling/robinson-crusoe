using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gain2Food : CardEffect
{   
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Gain2Food effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainFoodEvent(2);
        hasBeenApplied = true;
    }
}
