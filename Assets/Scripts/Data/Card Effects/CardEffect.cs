using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : ScriptableObject, IEffectApplicable
{
    public int CardEffectId { get; private set; }
    private static int NextCardEffectId = 0;

    protected bool hasBeenApplied = false;

    public TargetType targetType { get; protected set; }
    public int targetId { get; protected set; }

    public static CardEffect CreateCardEffectInstance(string effectName) {
        CardEffect cardEffect = ScriptableObject.CreateInstance(effectName) as CardEffect;
        cardEffect.Initialize();
        return cardEffect;
    }

    protected virtual void Initialize() {
        CardEffectId = NextCardEffectId++;
        targetType = TargetType.Global;
        targetId = -1;
    }

    public virtual void ApplyEffect() {
        if (!hasBeenApplied) {
            Debug.LogError("CardEffect class has no effect. Consider using a child class instead.");
            hasBeenApplied = true;
        } else {
            Debug.LogError("Card effects can only be applied once.");
        }
    }

    public void SetTarget(int targetId) {
        if (targetType == TargetType.Global) {
            Debug.LogError("SetTarget() has no effect on global card effects.");
            return;
        }
        this.targetId = targetId;
    }
}
