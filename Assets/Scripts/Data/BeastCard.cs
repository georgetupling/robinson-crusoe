using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class BeastCard : Card
{
    public string beastName { get; private set; }
    public int strength { get; private set; }
    public int spearsLostAfterFight { get; private set; }
    public int foodGainedAfterFight { get; private set; }
    public int hidesGainedAfterFight { get; private set; }
    public List<CardEffect> effectsAfterFight { get; private set; }

    public BeastCard(BeastCardUnprocessedData data) {
        beastName = data.beastName;
        strength = data.strength;
        spearsLostAfterFight = data.spearsLostAfterFight;
        foodGainedAfterFight = data.foodGainedAfterFight;
        hidesGainedAfterFight = data.hidesGainedAfterFight;
        effectsAfterFight = CardEffectFactory.CreateCardEffectList(data.effectsAfterFight);
        string materialName = data.beastName.Replace(" ", "") + "Material";
        cardMaterial = Resources.Load<Material>(Path.Combine("Materials/Beast Cards", materialName));
        if (cardMaterial == null) {
            Debug.LogError($"{materialName} not found.");
        }
        string spriteName = beastName.Replace(" ", "") + "Sprite";
        cardSprite = Resources.Load<Sprite>(Path.Combine("Sprites/Beast Cards", spriteName));
        if (cardSprite == null) {
            Debug.LogError($"{spriteName} not found.");
        }
    }
}
