using UnityEngine;

public class FirstPlayerLoses2Determination : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseDeterminationEvent(DeterminationEvent.FirstPlayer, 2);
    }
}
