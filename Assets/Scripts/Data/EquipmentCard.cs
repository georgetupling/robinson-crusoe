using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class EquipmentCard : Card
{
    public Equipment equipment { get; private set; }
    public List<CardEffect> effectsOnActivation { get; private set; }
    public EquipmentCard(EquipmentCardUnprocessedData data) {
        equipment = EnumParser.ParseEquipment(data.equipment);
        effectsOnActivation = CardEffectFactory.CreateCardEffectList(data.effectsOnActivation);
        string materialName = data.equipment.ToString() + "Material";
        cardMaterial = Resources.Load<Material>(Path.Combine("Materials/Equipment Cards", materialName));
        if (cardMaterial == null) {
            Debug.LogError($"Failed to load {materialName}.");
        }
        string spriteName = data.equipment.ToString() + "Sprite";
        cardSprite = Resources.Load<Sprite>(Path.Combine("Sprites/Equipment Cards", spriteName));
        if (cardSprite == null) {
            Debug.LogError($"Failed to load {spriteName}.");
        }
    }
}
