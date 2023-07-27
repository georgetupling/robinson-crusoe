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
        { "GainHealth", typeof(GainHealth) },
        { "Gain2Health", typeof(Gain2Health) },
        {"GainNonPerishableFood", typeof(GainNonPerishableFood)},
        { "GainWood", typeof(GainWood) },
        { "GainWeapon", typeof(GainWeapon) },
        { "Gain2Weapon", typeof(Gain2Weapon) },
        { "Gain3Weapon", typeof(Gain3Weapon) },
        { "GainPalisade", typeof(GainPalisade) },
        {"Gain2Wood", typeof(Gain2Wood)},
        { "LoseWeapon", typeof(LoseWeapon) },
        { "LosePalisade", typeof(LosePalisade) },
        { "LoseFood", typeof(LoseFood) },
        { "Lose2Weapon", typeof(Lose2Weapon) },
        { "Lose3Weapon", typeof(Lose3Weapon) },
        { "LoseMorale", typeof(LoseMorale) },
        { "Lose2Morale", typeof(Lose2Morale) },
        { "GainHide", typeof(GainHide) },
        { "Gain2Hide", typeof(Gain2Hide) },
        { "LoseDetermination", typeof(LoseDetermination) },
        { "AllLoseDetermination", typeof(AllLoseDetermination) },
        { "AllLoseHealth", typeof(AllLoseHealth) },
        { "DecideDiscardOrGet2FoodAnd2Hide", typeof(DecideDiscardOrGet2FoodAnd2Hide) },
        { "RainInWeatherSpace", typeof(RainInWeatherSpace) },
        { "SnowInWeatherSpace", typeof(SnowInWeatherSpace) },
        { "LoseRoofOrPalisade", typeof(LoseRoofOrPalisade) },
        { "WindInWeatherSpace", typeof(WindInWeatherSpace) },
        { "Gain3Determination", typeof(Gain3Determination) },
        { "GainDetermination", typeof(GainDetermination) },
        { "Gain2Determination", typeof(Gain2Determination) },
        { "GainRoofOrPalisade", typeof(GainRoofOrPalisade) },
        { "LoseHalfRoofOrPalisade", typeof(LoseHalfRoofOrPalisade) },
        { "RedDieInWeatherSpace", typeof(RedDieInWeatherSpace) },
        { "DecideDiscardOrGain2Wood", typeof(DecideDiscardOrGain2Wood) },
        { "RerollOnBuildAction", typeof(RerollOnBuildAction) },
        { "RerollOnGatherAction", typeof(RerollOnGatherAction) },
        { "RerollOnExploreAction", typeof(RerollOnExploreAction) },
        { "DecideDiscardOrGainFoodPerPlayer", typeof(DecideDiscardOrGainFoodPerPlayer) },
        { "Gain2Palisade", typeof(Gain2Palisade) },
        { "Lose2Palisade", typeof(Lose2Palisade) },
        { "LoseHealth", typeof(LoseHealth) },
        { "GainDiscoveryToken", typeof(GainDiscoveryToken) },
        { "LoseDeterminationOrHealth", typeof(LoseDeterminationOrHealth) }
    };
    
    public static List<CardEffect> CreateCardEffectList(List<string> namesOfCardEffects) {
        List<CardEffect> cardEffects = new List<CardEffect>();

        foreach (string effectName in namesOfCardEffects) {
            if (typeMap.ContainsKey(effectName)) {
                CardEffect cardEffect = CardEffect.CreateCardEffectInstance(effectName) as CardEffect;
                cardEffects.Add(cardEffect);
            } else {
                Debug.Log($"CardEffectFactory: {effectName} not found in typeMap.");
            }
        }
        return cardEffects;
    }
}
