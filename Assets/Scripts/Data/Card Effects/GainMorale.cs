using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainMorale : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainMoraleEvent(1);
    }
}
