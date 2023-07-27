using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainHide : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("GainHide effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainHideEvent(1);
        hasBeenApplied = true;
    }
}
