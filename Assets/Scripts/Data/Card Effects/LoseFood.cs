using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseFood : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("LoseFood effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseFoodEvent(1);
        hasBeenApplied = true;
    }
}
