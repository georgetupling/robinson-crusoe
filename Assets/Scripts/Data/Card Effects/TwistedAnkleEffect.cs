using UnityEngine;

public class TwistedAnkleEffect : OngoingCardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
        endTrigger = Trigger.None;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("TwistedAnkleEffect effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("TwistedAnkleEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(targetId, WoundType.Leg, TokenType.GatherWound);
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        hasBeenApplied = true;
    }

    public override void EndEffect() {
        if (hasEnded) {
            Debug.LogError("TwistedAnkleEffect effect has already ended.");
            return;
        }
        EventGenerator.Singleton.RaisePlayerCanOnlyRestBuildOrMakeCampThisTurnEvent(targetId);
        EventGenerator.Singleton.RaiseDestroyWoundTokenEvent(targetId, WoundType.Leg, TokenType.GatherWound);
        hasEnded = true;
    }
}
