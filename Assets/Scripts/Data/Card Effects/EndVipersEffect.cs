using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndVipersEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndVipersEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(VipersEffect));
        hasBeenApplied = true;
    }
}
