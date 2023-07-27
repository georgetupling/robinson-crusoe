using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AdventureCard : Card
{
    public string adventureName { get; private set; }
    public AdventureType adventureType { get; private set; }
    public string adventureDescription { get; private set; }
    public List<CardEffect> adventureEffects { get; private set; }
    public bool adventureHasDecision { get; private set; }
    public List<string> adventureOptions { get; private set; }

    public bool hasEvent { get; private set; }
    public string eventName { get; private set; }
    public string eventDescription { get; private set; }
    public List<CardEffect> eventEffects { get; private set; }
    public bool eventHasDecision { get; private set; }
    public List<string> eventOptions { get; private set; }

    public AdventureCard(AdventureCardUnprocessedData data) {
        adventureName = data.adventureName;
        adventureType = EnumParser.ParseAdventureType(data.adventureType);
        adventureDescription = data.adventureDescription;
        adventureEffects = CardEffectFactory.CreateCardEffectList(data.adventureEffects);
        adventureHasDecision = data.adventureHasDecision;
        adventureOptions = data.adventureOptions;
        hasEvent = data.hasEvent;
        eventName = data.eventName;
        eventDescription = data.eventDescription;
        eventEffects = CardEffectFactory.CreateCardEffectList(data.eventEffects);
        eventHasDecision = data.eventHasDecision;
        eventOptions = data.eventOptions;

        // Selects the correct folder to load the Material from
        cardMaterial = Resources.Load<Material>(Path.Combine("Materials/Adventure Cards", data.materialName));
        if (cardMaterial == null) {
            Debug.LogError($"Failed to load {data.materialName}.");
        }
        string spriteName = ResourceName.GetSpriteName(data.adventureName);
        cardSprite = Resources.Load<Sprite>(Path.Combine("Sprites/Adventure Cards", spriteName));
        if (cardSprite == null) {
            Debug.LogError($"Failed to load {spriteName}.");
        }
    }
}
