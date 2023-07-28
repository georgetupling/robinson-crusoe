using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAccidentEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndAccidentEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(AccidentEffect));
        hasBeenApplied = true;
    }
}
