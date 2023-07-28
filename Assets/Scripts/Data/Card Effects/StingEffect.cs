using UnityEngine;

public class StingEffect : OngoingCardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
        endTrigger = Trigger.None;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("StingEffect effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("StingEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(targetId, WoundType.Belly, TokenType.BuildWound);
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        hasBeenApplied = true;
    }

    public override void EndEffect() {
        if (hasEnded) {
            Debug.LogError("StingEffect effect has already ended.");
            return;
        }
        if (!medicineBuilt) {
            EventGenerator.Singleton.RaisePlayerHasOnly1ActionThisTurnEvent(targetId);
        }
        EventGenerator.Singleton.RaiseDestroyWoundTokenEvent(targetId, WoundType.Belly, TokenType.BuildWound);
        hasEnded = true;
    }
}
