using UnityEngine;

public class ThornyBushEffect : OngoingCardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
        endTrigger = Trigger.None;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("ThornyBushEffect effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("ThornyBushEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(targetId, WoundType.Arm, TokenType.ExploreWound);
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        hasBeenApplied = true;
    }

    public override void EndEffect() {
        if (hasEnded) {
            Debug.LogError("ThornyBushEffect effect has already ended.");
            return;
        }
        if (!medicineBuilt) {
            EventGenerator.Singleton.RaisePlayerHasOnly1ActionThisTurnEvent(targetId);
        }
        EventGenerator.Singleton.RaiseDestroyWoundTokenEvent(targetId, WoundType.Arm, TokenType.ExploreWound);
        hasEnded = true;
    }
}
