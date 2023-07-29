using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public CharacterType characterType { get; private set; }
    public string characterName { get; private set; }
    public int maximumHealth { get; private set; }
    public List<int> moraleThresholds { get; private set; }
    public Invention personalInvention { get; private set; }
    public List<Ability> abilities { get; private set; }

    // TODO: character special abilities
    public Character(CharacterType characterType, string characterName, int maximumHealth, List<int> moraleThresholds, Invention personalInvention, List<Ability> abilities) {
        this.characterType = characterType;
        this.characterName = characterName;
        this.maximumHealth = maximumHealth;
        this.moraleThresholds = moraleThresholds;
        this.personalInvention = personalInvention;
        this.abilities = abilities;
    }
}
