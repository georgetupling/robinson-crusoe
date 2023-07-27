using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gain2Hide : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Gain2Hide effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainHideEvent(2);
        hasBeenApplied = true;
    }
}
