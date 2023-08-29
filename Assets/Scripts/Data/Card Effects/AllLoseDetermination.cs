using UnityEngine;

public class AllLoseDetermination : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseDeterminationEvent(DeterminationEvent.AllPlayers, 1);
    }
}
