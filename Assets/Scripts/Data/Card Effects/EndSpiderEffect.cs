using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSpiderEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndSpiderEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(SpiderEffect));
        hasBeenApplied = true;
    }
}
