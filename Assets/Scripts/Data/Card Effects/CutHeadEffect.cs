using UnityEngine;

public class CutHeadEffect : OngoingCardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
        endTrigger = Trigger.None;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("CutHeadEffect effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("CutHeadEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(targetId, WoundType.Head, TokenType.BuildWound);
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        hasBeenApplied = true;
    }

    public override void EndEffect() {
        if (hasEnded) {
            Debug.LogError("CutHeadEffect effect has already ended.");
            return;
        }
        if (!medicineBuilt) {
            EventGenerator.Singleton.RaiseLoseHealthEvent(targetId, 2);
        }
        EventGenerator.Singleton.RaiseDestroyWoundTokenEvent(targetId, WoundType.Head, TokenType.BuildWound);
        hasEnded = true;
    }
}
