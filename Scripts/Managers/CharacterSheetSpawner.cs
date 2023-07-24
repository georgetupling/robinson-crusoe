using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheetSpawner : MonoBehaviour
{
    Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();

    [SerializeField] Transform characterSheetArea;

    // Game Settings
    int playerCount;
    Dictionary<int, CharacterType> playerCharacters = new Dictionary<int, CharacterType>();

    void Awake() {
        playerCount = GameSettings.PlayerCount;
        playerCharacters = GameSettings.PlayerCharacters;
        InitializePositions();
    }

    void Start() {
        SpawnCharacterSheets();
    }

    void InitializePositions() {
        for (int positionNumber = 0; positionNumber < playerCount; positionNumber++) {
            string transformName = "Position" + positionNumber;
            Transform positionTransform = transform.Find(transformName);
            if (positionTransform == null) {
                Debug.LogError($"{transformName} not found as child of parent transform.");
                return;
            }
            positions.Add(positionNumber, positionTransform.localPosition);
        }
    }

    void SpawnCharacterSheets() {
        for (int playerId = 0; playerId < playerCount; playerId++) {
            SpawnCharacterSheet(playerId, playerCharacters[playerId], positions[playerId]);
        }
    }

    void SpawnCharacterSheet(int playerId, CharacterType characterType, Vector3 position) {
        CharacterSheetController prefab = PrefabLoader.Singleton.GetPrefab(characterType);
        if (prefab == null) {
            return;
        }
        CharacterSheetController newCharacterSheet = Instantiate(prefab, characterSheetArea, false);
        EventGenerator.Singleton.RaiseMoveComponentEvent(
            newCharacterSheet.ComponentId,
            characterSheetArea,
            position,
            MoveStyle.Instant
        );
        Character character = CharacterFactory.CreateCharacter(characterType);
        EventGenerator.Singleton.RaiseInitializeCharacterSheetEvent(newCharacterSheet.ComponentId, playerId, character);
    }
}
