using UnityEngine;

public class EndThornyBushEffect : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("EndThornyBushEffect effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(ThornyBushEffect));
        hasBeenApplied = true;
    }
}
