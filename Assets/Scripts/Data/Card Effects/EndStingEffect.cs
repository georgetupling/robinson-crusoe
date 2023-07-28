using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndStingEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndStingEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(StingEffect));
        hasBeenApplied = true;
    }
}
