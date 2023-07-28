using UnityEngine;

public class AccidentEffect : OngoingCardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
        endTrigger = Trigger.None;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("AccidentEffect effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("AccidentEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(targetId, WoundType.Leg, TokenType.BuildWound);
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        hasBeenApplied = true;
    }

    public override void EndEffect() {
        if (hasEnded) {
            Debug.LogError("AccidentEffect effect has already ended.");
            return;
        }
        if (!medicineBuilt) {
            EventGenerator.Singleton.RaiseLoseHealthEvent(targetId, 3);
        } else {
            EventGenerator.Singleton.RaiseLoseHealthEvent(targetId, 1);
        }
        EventGenerator.Singleton.RaiseDestroyWoundTokenEvent(targetId, WoundType.Leg, TokenType.BuildWound);
        hasEnded = true;
    }
}
