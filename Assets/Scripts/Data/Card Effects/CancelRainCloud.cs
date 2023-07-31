using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelRainCloud : CardEffect
{   
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("CancelRainCloud effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseCancelRainCloudEvent();
        hasBeenApplied = true;
    }
}
