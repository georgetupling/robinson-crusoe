using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gain2Wood : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainWoodEvent(2);
        hasBeenApplied = true;
    }
}
