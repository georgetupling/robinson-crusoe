using UnityEngine;

public class OngoingCardEffect : CardEffect, IEffectEndable
{
    public Trigger EndTrigger { get; private set; }

    public OngoingCardEffect() : base() {
        EndTrigger = Trigger.EndTurn; // Default value
    }

    public new void ApplyEffect() {
        if (!hasBeenApplied) {
            EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
            hasBeenApplied = true;
            Debug.LogError("OngoingCardEffect started with no effect. Consider using a child class instead.");
        } else {
            Debug.LogError("Card effects can only be applied once.");
        }
    }

    public void EndEffect() {
        Debug.LogError("OngoingCardEffect ended with no effect. Consider using a child class instead.");
    }
}
