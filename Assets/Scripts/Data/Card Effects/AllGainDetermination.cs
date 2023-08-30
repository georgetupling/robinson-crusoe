using UnityEngine;

public class AllGainDetermination : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainDeterminationEvent(DeterminationEvent.AllPlayers, 1);
    }
}
