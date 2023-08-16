using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainNonPerishableFood : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainNonPerishableFoodEvent(1);
    }
}
