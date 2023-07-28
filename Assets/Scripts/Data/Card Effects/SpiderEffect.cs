using UnityEngine;

public class SpiderEffect : OngoingCardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
        endTrigger = Trigger.None;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("SpiderEffect effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("SpiderEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(targetId, WoundType.Head, TokenType.GatherWound);
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        hasBeenApplied = true;
    }

    public override void EndEffect() {
        if (hasEnded) {
            Debug.LogError("SpiderEffect effect has already ended.");
            return;
        }
        EventGenerator.Singleton.RaisePlayerHasOnly1ActionThisTurnEvent(targetId);
        EventGenerator.Singleton.RaiseDestroyWoundTokenEvent(targetId, WoundType.Head, TokenType.GatherWound);
        hasEnded = true;
    }
}
