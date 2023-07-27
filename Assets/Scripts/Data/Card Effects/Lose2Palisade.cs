using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lose2Palisade : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Lose2Palisade effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseLosePalisadeEvent(2);
        hasBeenApplied = true;
    }
}
