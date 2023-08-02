using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[System.Serializable]
public class InventionCard : Card
{
    public Invention invention { get; private set; }
    public List<Invention> itemRequirements { get; private set; }
    public List<ResourceCost> resourceCosts { get; private set; }
    public Terrain terrainTypeRequirement { get; private set; }
    public List<CardEffect> effectsOnBuild { get; private set; }
    public List<CardEffect> effectsOnLoss { get; private set; }
    public List<CardEffect> effectsOnActivation { get; private set; }
    public bool isDefaultInvention { get; private set; }
    public bool isPersonalInvention { get; private set; }

    public InventionCard(InventionCardUnprocessedData data) {
        invention = EnumParser.ParseInvention(data.inventionName);
        itemRequirements = EnumParser.ParseInventionList(data.itemRequirements);
        resourceCosts = EnumParser.ParseResourceCostList(data.resourceCosts);
        terrainTypeRequirement = EnumParser.ParseTerrain(data.terrainTypeRequirement);
        effectsOnBuild = CardEffectFactory.CreateCardEffectList(data.effectsOnBuild);
        effectsOnLoss = CardEffectFactory.CreateCardEffectList(data.effectsOnLoss);
        effectsOnActivation = CardEffectFactory.CreateCardEffectList(data.effectsOnActivation);
        isDefaultInvention = data.isDefaultInvention;
        isPersonalInvention = data.isPersonalInvention;
        string materialName = data.inventionName + "Material";
        cardMaterial = Resources.Load<Material>(Path.Combine("Materials/Invention Cards", materialName));
        if (cardMaterial == null) {
            Debug.LogError($"Failed to load {materialName}.");
        }
        string spriteName = data.inventionName.Replace(" ", "") + "FrontSprite";
        cardSprite = Resources.Load<Sprite>(Path.Combine("Sprites/Invention Cards", spriteName));
        if (cardSprite == null) {
            Debug.LogError($"Failed to load {spriteName}.");
        }
        string spriteBackName = data.inventionName.Replace(" ", "") + "BackSprite";
        cardBackSprite = Resources.Load<Sprite>(Path.Combine("Sprites/Invention Cards", spriteBackName));
        if (cardBackSprite == null) {
            Debug.LogError($"Failed to load {spriteBackName}.");
        }
    }
}
