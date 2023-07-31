using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSoupEffect : CardEffect
{ 
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseGainFoodEvent(1);
    }
}
