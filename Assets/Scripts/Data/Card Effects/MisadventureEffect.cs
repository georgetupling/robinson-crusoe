using UnityEngine;

public class MisadventureEffect : OngoingCardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
        endTrigger = Trigger.None;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("MisadventureEffect effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("MisadventureEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(targetId, WoundType.Leg, TokenType.ExploreWound);
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        hasBeenApplied = true;
    }

    public override void EndEffect() {
        if (hasEnded) {
            Debug.LogError("MisadventureEffect effect has already ended.");
            return;
        }
        EventGenerator.Singleton.RaisePlayerCanOnlyRestBuildOrMakeCampThisTurnEvent(targetId);
        EventGenerator.Singleton.RaiseDestroyWoundTokenEvent(targetId, WoundType.Leg, TokenType.ExploreWound);
        hasEnded = true;
    }
}
