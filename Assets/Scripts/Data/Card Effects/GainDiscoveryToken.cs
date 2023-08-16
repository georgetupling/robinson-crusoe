using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainDiscoveryToken : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseDrawDiscoveryTokenEvent(1);
    }
}
