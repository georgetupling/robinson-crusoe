using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class EventCard : Card
{
    public string eventName { get; private set; }
    public string eventDescription { get; private set; }
    public CardSymbol cardSymbol { get; private set; }
    public List<CardEffect> eventEffects { get; private set; }
    public bool eventHasDecision { get; private set; }
    public List<string> eventOptions { get; private set; }
    public string threatName { get; private set; }
    public List<Invention> threatItemRequirements { get; private set; }
    public int threatWeaponRequirement { get; private set; }
    public List<ResourceCost> threatResourceCosts { get; private set; }
    public bool has1ActionThreat { get; private set; }
    public bool has2ActionThreat { get; private set; }
    public List<CardEffect> successEffects1Action { get; private set; }
    public List<CardEffect> successEffects2Action { get; private set; }
    public List<CardEffect> failureEffects { get; private set; }
    public bool isStartingEvent { get; private set; }

    public EventCard(EventCardUnprocessedData data) {
        eventName = data.eventName;
        eventDescription = data.eventDescription;
        cardSymbol = EnumParser.ParseCardSymbol(data.cardSymbol);
        eventEffects = CardEffectFactory.CreateCardEffectList(data.eventEffects);
        eventHasDecision = data.eventHasDecision;
        eventOptions = data.eventOptions;
        threatName = data.threatName;
        threatItemRequirements = EnumParser.ParseInventionList(data.threatItemRequirements);
        threatWeaponRequirement = data.threatWeaponRequirement;
        threatResourceCosts = EnumParser.ParseResourceCostList(data.threatResourceCosts);
        has1ActionThreat = data.has1ActionThreat;
        has2ActionThreat = data.has2ActionThreat;
        successEffects1Action = CardEffectFactory.CreateCardEffectList(data.successEffects1Action);
        successEffects2Action = CardEffectFactory.CreateCardEffectList(data.successEffects2Action);
        failureEffects = CardEffectFactory.CreateCardEffectList(data.failureEffects);
        cardMaterial = Resources.Load<Material>(Path.Combine("Materials/Event Cards", data.materialName));
        if (cardMaterial == null) {
            Debug.LogError($"Failed to load {data.materialName}.");
        }
        isStartingEvent = data.isStartingEvent;
        string spriteName = ResourceName.GetSpriteName(eventName);
        cardSprite = Resources.Load<Sprite>(Path.Combine("Sprites/Event Cards", spriteName));
        if (cardSprite == null) {
            Debug.LogError($"Failed to load {spriteName}.");
        }
    }
}
