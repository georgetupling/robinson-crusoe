using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCutHeadEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndCutHeadEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(CutHeadEffect));
        hasBeenApplied = true;
    }
}
