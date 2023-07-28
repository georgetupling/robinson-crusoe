using UnityEngine;

public class NastyWoundEffect : OngoingCardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
        endTrigger = Trigger.None;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("NastyWoundEffect effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("NastyWoundEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(targetId, WoundType.Arm, TokenType.BuildWound);
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        hasBeenApplied = true;
    }

    public override void EndEffect() {
        if (hasEnded) {
            Debug.LogError("NastyWoundEffect effect has already ended.");
            return;
        }
        if (!medicineBuilt) {
            EventGenerator.Singleton.RaiseLoseHealthEvent(targetId, 2);
        }
        EventGenerator.Singleton.RaiseDestroyWoundTokenEvent(targetId, WoundType.Arm, TokenType.BuildWound);
        hasEnded = true;
    }
}
