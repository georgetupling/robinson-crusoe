using UnityEngine;

public class AllLose2Determination : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseDeterminationEvent(DeterminationEvent.AllPlayers, 2);
    }
}
