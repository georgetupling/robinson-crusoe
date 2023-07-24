using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardEffectFactory
{
    private static Dictionary<string, System.Type> typeMap = new Dictionary<string, System.Type>(){
        // {"GainEquipment", typeof(GainEquipment)},
        { "GainFood", typeof(GainFood) },
        { "Gain2Food", typeof(Gain2Food) },
        { "GainMorale", typeof(GainMorale) },
        { "Gain2Health", typeof(Gain2Health) },
        //{"GainNonPerishableFood", typeof(GainNonPerishableFood)},
        { "GainWood", typeof(GainWood) },
        { "GainWeapon", typeof(GainWeapon) },
        { "Gain2Weapon", typeof(Gain2Weapon) },
        { "GainPalisade", typeof(GainPalisade) }
        // {"Gain2Wood", typeof(Gain2Wood)},
    };
    
    public static List<CardEffect> CreateCardEffectList(List<string> namesOfCardEffects) {
        List<CardEffect> cardEffects = new List<CardEffect>();

        foreach (string effectName in namesOfCardEffects) {
            if (typeMap.ContainsKey(effectName)) {
                CardEffect cardEffect = CardEffect.CreateCardEffectInstance(effectName) as CardEffect;
                cardEffects.Add(cardEffect);
            } else {
                Debug.LogError($"CardEffectFactory: {effectName} not found in typeMap.");
            }
        }
        return cardEffects;
    }
}
