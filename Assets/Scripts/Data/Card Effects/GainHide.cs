using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainHide : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainHideEvent(1);
    }
}
