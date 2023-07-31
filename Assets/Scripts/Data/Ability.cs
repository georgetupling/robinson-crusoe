using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Ability
{
    public string abilityName { get; private set; }
    public int determinationCost { get; private set; }
    public int foodCost { get; private set; }
    public bool isActivatedAbility { get; private set; }
    public List<string> options { get; private set; }
    public CardEffect abilityEffect { get; private set; }
    public Sprite abilitySprite { get; private set; }

    public Ability(string abilityName, int determinationCost, int foodCost, bool isActivatedAbility, CardEffect abilityEffect) {
        this.abilityName = abilityName;
        this.determinationCost = determinationCost;
        this.foodCost = foodCost;
        this.isActivatedAbility = isActivatedAbility;
        options = new List<string>();
        this.abilityEffect = abilityEffect;
        string spriteName = abilityName.ToString().Replace(" ", "") + "Sprite";
        abilitySprite = Resources.Load<Sprite>(Path.Combine("Sprites/Abilities", spriteName));
        if (abilitySprite == null) {
            Debug.LogError($"Failed to load {spriteName}.");
        }
    }

    public Ability(string abilityName, int determinationCost, int foodCost, bool isActivatedAbility, List<string> options, CardEffect abilityEffect) {
        this.abilityName = abilityName;
        this.determinationCost = determinationCost;
        this.foodCost = foodCost;
        this.isActivatedAbility = isActivatedAbility;
        this.options = options;
        options = new List<string>();
        this.abilityEffect = abilityEffect;
        string spriteName = abilityName.ToString().Replace(" ", "") + "Sprite";
        abilitySprite = Resources.Load<Sprite>(Path.Combine("Sprites/Abilities", spriteName));
        if (abilitySprite == null) {
            Debug.LogError($"Failed to load {spriteName}.");
        }
    }
}
