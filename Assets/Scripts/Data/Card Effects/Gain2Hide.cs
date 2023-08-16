using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gain2Hide : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainHideEvent(2);
        hasBeenApplied = true;
    }
}
