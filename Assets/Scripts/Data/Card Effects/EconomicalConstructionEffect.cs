using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomicalConstructionEffect : CardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
    }
    
    public override void ApplyEffect() {
        if (targetId == -1) {
            Debug.LogError("EconomicalConstructionEffect target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseEconomicalConstructionEvent(targetId);
    }
}
