using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainFood : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainFoodEvent(1);
    }
}
