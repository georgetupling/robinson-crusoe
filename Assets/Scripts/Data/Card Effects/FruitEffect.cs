using UnityEngine;

public class FruitEffect : OngoingCardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
        endTrigger = Trigger.None;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("FruitEffect effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("FruitEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(targetId, WoundType.Belly, TokenType.GatherWound);
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        hasBeenApplied = true;
    }

    public override void EndEffect() {
        if (hasEnded) {
            Debug.LogError("FruitEffect effect has already ended.");
            return;
        }
        if (!medicineBuilt) {
            EventGenerator.Singleton.RaisePlayerCanOnlyRestThisTurnEvent(targetId);
        }
        EventGenerator.Singleton.RaiseDestroyWoundTokenEvent(targetId, WoundType.Head, TokenType.BuildWound);
        hasEnded = true;
    }
}
