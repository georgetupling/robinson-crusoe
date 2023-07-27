using UnityEngine;

public class AllLoseHealth : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("AllLoseHealth effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseHealthEvent(HealthEvent.AllPlayers, 1);
        hasBeenApplied = true;
    }
}
