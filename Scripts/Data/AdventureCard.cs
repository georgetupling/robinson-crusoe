using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AdventureCard : Card
{
    public enum AdventureType {Build, Gather, Explore};

    public string adventureName { get; private set; }
    public AdventureType adventureType { get; private set; }
    public string adventureDescription { get; private set; }
    public List<CardEffect> adventureEffects { get; private set; }

    public bool hasEvent { get; private set; }
    public string eventName { get; private set; }
    public string eventDescription { get; private set; }
    public List<CardEffect> eventEffects { get; private set; }

    public AdventureCard(AdventureCardUnprocessedData data) {
        adventureName = data.adventureName;
        adventureType = GetAdventureType(data.adventureType);
        adventureDescription = data.adventureDescription;
        adventureEffects = CardEffectFactory.CreateCardEffectList(data.adventureEffects);
        hasEvent = data.hasEvent;
        eventName = data.eventName;
        eventDescription = data.eventDescription;
        eventEffects = CardEffectFactory.CreateCardEffectList(data.eventEffects);

        // Selects the correct folder to load the Material from
        string folder = adventureType == AdventureType.Build ? "Materials/Build Adventure Cards" : adventureType == AdventureType.Gather ? "Materials/Gather Adventure Cards" : "Materials/Explore Adventure Cards";
        cardMaterial = Resources.Load<Material>(Path.Combine(folder, data.materialName));
        string spriteName = adventureName.Replace(" ", "") + "Sprite";
        cardSprite = Resources.Load(Path.Combine("Sprites/Adventure Cards", spriteName)) as Sprite;
    }

    private AdventureType GetAdventureType(string str) {
        AdventureType adventureType;
        if (AdventureType.TryParse(str, out adventureType)) {
            return adventureType;
        } else if (str == null || str == "") {
            Debug.Log($"AdventureCard: no adventure type provided.");
        } else {
            Debug.Log($"AdventureCard: {str} is not a valid adventure type.");
        }
        return AdventureType.Build; // Default value
    }
}
