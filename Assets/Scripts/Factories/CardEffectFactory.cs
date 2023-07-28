using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardEffectFactory
{
    public static List<CardEffect> CreateCardEffectList(List<string> namesOfCardEffects) {
        List<CardEffect> cardEffects = new List<CardEffect>();

        foreach (string effectName in namesOfCardEffects) {
            System.Type type = System.Type.GetType(effectName);
            if (type != null && typeof(CardEffect).IsAssignableFrom(type)) {
                CardEffect cardEffect = CardEffect.CreateCardEffectInstance(effectName) as CardEffect;
                cardEffects.Add(cardEffect);
            } else {
                Debug.Log($"{effectName} is not a valid CardEffect type.");
            }
        }
        return cardEffects;
    }
}
