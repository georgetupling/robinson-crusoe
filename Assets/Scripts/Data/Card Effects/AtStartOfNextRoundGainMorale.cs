using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtStartOfNextRoundGainMorale : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseAtStartOfNextRoundGainMoraleEvent();
    }
}
