using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainWoodInProductionPhase : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainResourceInProductionPhaseEvent(ResourceType.Wood, 1);
    }
}
