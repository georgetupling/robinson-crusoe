using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndUnbelievableEffortEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndUnbelievableEffortEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(UnbelievableEffortEffect));
        hasBeenApplied = true;
    }
}
