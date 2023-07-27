using UnityEngine;

public class Lose2Morale : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Lose2Morale effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseMoraleEvent(2);
        hasBeenApplied = true;
    }
}
