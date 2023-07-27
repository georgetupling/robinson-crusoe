using UnityEngine;

public class LoseDetermination : CardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("LoseDetermination effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("LoseDetermination target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseDeterminationEvent(targetId, 1);
        hasBeenApplied = true;
    }
}
