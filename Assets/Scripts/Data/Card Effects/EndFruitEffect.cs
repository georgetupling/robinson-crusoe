using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndFruitEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndFruitEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(FruitEffect));
        hasBeenApplied = true;
    }
}
