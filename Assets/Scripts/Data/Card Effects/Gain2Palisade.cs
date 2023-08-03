using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gain2Palisade : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainPalisadeEvent(2);
    }
}
