using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndNastyWoundEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndNastyWoundEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(NastyWoundEffect));
        hasBeenApplied = true;
    }
}
