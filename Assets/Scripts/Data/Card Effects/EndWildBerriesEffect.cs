using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndWildBerriesEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndWildBerriesEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(WildBerriesEffect));
        hasBeenApplied = true;
    }
}
