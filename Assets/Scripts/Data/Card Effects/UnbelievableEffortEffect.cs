using UnityEngine;

public class UnbelievableEffortEffect : OngoingCardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
        endTrigger = Trigger.None;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("UnbelievableEffortEffect effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("UnbelievableEffortEffect target not set.");
            return;
        }
        if (optionChosen == -1) {
            Debug.LogError("UnbelievableEffortEffect optionChosen not set.");
            return;
        }
        // 0 = discard, 1 = gain 2 wood and gain a wound
        if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseGainWoodEvent(2);
            EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(targetId, WoundType.Arm, TokenType.GatherWound);
            EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
        }
        hasBeenApplied = true;
    }

    public override void EndEffect() {
        if (hasEnded) {
            Debug.LogError("AccidentEffect effect has already ended.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseHealthEvent(targetId, 1);
        EventGenerator.Singleton.RaiseDestroyWoundTokenEvent(targetId, WoundType.Arm, TokenType.GatherWound);
        hasEnded = true;
    }
}
