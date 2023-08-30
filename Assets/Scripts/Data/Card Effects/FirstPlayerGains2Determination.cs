using UnityEngine;

public class FirstPlayerGains2Determination : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainDeterminationEvent(DeterminationEvent.FirstPlayer, 2);
    }
}
