using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LosePalisade : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("LosePalisade effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseLosePalisadeEvent(1);
        hasBeenApplied = true;
    }
}
