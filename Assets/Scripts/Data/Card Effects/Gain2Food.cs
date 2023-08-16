using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gain2Food : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainFoodEvent(2);
        hasBeenApplied = true;
    }
}
