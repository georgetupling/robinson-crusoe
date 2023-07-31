using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/*
    This class loads the game's prefabs at start-up and makes them globally accessible.
    An older version of the class was static and didn't store the prefabs for use by multiple classes.
*/

public class PrefabLoader : MonoBehaviour
{
    public static PrefabLoader Singleton { get; private set; }

    Dictionary<TokenType, TokenController> tokenPrefabs = new Dictionary<TokenType, TokenController>();
    List<CharacterSheetController> characterSheetPrefabs = new List<CharacterSheetController>();

    private Dictionary<ResourceType, TokenType> resourceToTokenMap = new Dictionary<ResourceType, TokenType> {
            { ResourceType.Wood, TokenType.Wood },
            { ResourceType.Food, TokenType.Food },
            { ResourceType.NonPerishableFood, TokenType.NonPerishableFood },
            { ResourceType.Hide, TokenType.Hide }
        };

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
        } else {
            return;
        }
        LoadTokenPrefabs();
        LoadCharacterSheetPrefabs();
    }

    // Loads prefabs on start-up

    void LoadTokenPrefabs() {
        foreach (TokenType tokenType in Enum.GetValues(typeof(TokenType))) {
            string prefabName = tokenType.ToString() + "TokenPrefab";
            TokenController prefab = Resources.Load<TokenController>(Path.Combine("Prefabs", prefabName));
            tokenPrefabs[tokenType] = prefab;
        }
    }

    void LoadCharacterSheetPrefabs() {
        foreach (CharacterType characterType in Enum.GetValues(typeof(CharacterType))) {
            foreach (Gender gender in Enum.GetValues(typeof(Gender))) {
                string prefabName = characterType.ToString() + gender.ToString() + "Prefab";
                CharacterSheetController prefab = Resources.Load<CharacterSheetController>(Path.Combine("Prefabs", prefabName));
                characterSheetPrefabs.Add(prefab);
            }
        }
    }

    // Provides public methods for retrieving prefabs

    public TokenController GetPrefab(TokenType tokenType) {
        if (!tokenPrefabs.ContainsKey(tokenType) || tokenPrefabs[tokenType] == null) {
            Debug.LogError($"Prefab for {tokenType} token does not exist.");
            return null;
        }
        return tokenPrefabs[tokenType];
    }

    public TokenController GetPrefab(ResourceType resourceType) {
        TokenType tokenType = resourceToTokenMap[resourceType];
        return GetPrefab(tokenType);
    }

    public CharacterSheetController GetPrefab(CharacterType characterType, Gender gender) {
        CharacterSheetController prefab = characterSheetPrefabs.Find(x => x.characterType == characterType && x.gender == gender);
        if (prefab == null) {
            Debug.LogError($"Failed to load {gender} {characterType} prefab.");
        }
        return prefab;
    }
}
