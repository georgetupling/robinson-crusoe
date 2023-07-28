using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMisadventureEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndMisadventureEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(MisadventureEffect));
        hasBeenApplied = true;
    }
}
