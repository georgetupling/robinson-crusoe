using UnityEngine;

public class LoseMorale : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("LoseMorale effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseMoraleEvent(1);
        hasBeenApplied = true;
    }
}
