using UnityEngine;

public class AllLoseDetermination : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("AllLoseDetermination effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseDeterminationEvent(DeterminationEvent.AllPlayers, 1);
        hasBeenApplied = true;
    }
}
