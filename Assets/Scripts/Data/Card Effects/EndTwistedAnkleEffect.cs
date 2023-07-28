using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTwistedAnkleEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndTwistedAnkleEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(TwistedAnkleEffect));
        hasBeenApplied = true;
    }
}
