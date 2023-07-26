using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DiscoveryToken
{
    public Discovery discovery { get; private set; }
    public string activationMessage { get; private set; }
    public List<CardEffect> effectsOnDraw { get; private set; }
    public bool canBeActivated { get; private set; }
    public bool activationRequiresWeapon { get; private set; }
    public bool activationRequiresPot { get; private set; }
    public List<CardEffect> effectsOnActivation { get; private set; }
    public Material tokenMaterial { get; private set; }
    public Sprite tokenSprite { get; private set; }
    public Sprite tokenBackSprite { get; private set; }

    public DiscoveryToken(DiscoveryTokenUnprocessedData data) {
        discovery = EnumParser.ParseDiscovery(data.discovery);
        activationMessage = data.activationMessage;
        effectsOnDraw = CardEffectFactory.CreateCardEffectList(data.effectsOnDraw);
        canBeActivated = data.canBeActivated;
        activationRequiresWeapon = data.activationRequiresWeapon;
        activationRequiresPot = data.activationRequiresPot;
        effectsOnActivation = CardEffectFactory.CreateCardEffectList(data.effectsOnActivation);
        string materialName = data.discovery.ToString() + "Material";
        tokenMaterial = Resources.Load<Material>(Path.Combine("Materials/Discovery Tokens", materialName));
        if (tokenMaterial == null) {
            Debug.LogError($"Failed to find {discovery} token material.");
        }
        string spriteName = data.discovery.ToString() + "Sprite";
        tokenSprite = Resources.Load<Sprite>(Path.Combine("Sprites/Discovery Tokens", spriteName));
        if (tokenSprite == null) {
            Debug.LogError($"Failed to find {discovery} token sprite.");
        }
        tokenBackSprite = Resources.Load<Sprite>(Path.Combine("Sprites/Discovery Tokens", "DiscoveryTokenBackSprite"));
    }
}
