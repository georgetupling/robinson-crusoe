using UnityEngine;

public class HandymanEffect : CardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
    }
    
    public override void ApplyEffect() {
        if (targetId == -1) {
            Debug.LogError("HandymanEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnSingleUsePawnEvent(targetId, PawnType.Build);
    }
}
