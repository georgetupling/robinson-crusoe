using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheetSpawner : MonoBehaviour
{
    Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();

    [SerializeField] Transform characterSheetArea;
    [SerializeField] Transform dogPrefab;
    [SerializeField] Transform fridayPrefab;

    // Game Settings
    int playerCount;

    void Awake() {
        playerCount = GameSettings.PlayerCount;
        InitializePositions();
    }

    void Start() {
        SpawnCharacterSheets();
        if (GameSettings.PlayerCount == 1) {
            SpawnFriday();
            SpawnDog();
        } else if (GameSettings.PlayerCount == 2) {
            SpawnFriday();
        }
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
            SpawnCharacterSheet(playerId, GameSettings.PlayerCharacters[playerId], GameSettings.PlayerGenders[playerId], positions[playerId]);
        }
    }

    void SpawnCharacterSheet(int playerId, CharacterType characterType, Gender gender, Vector3 position) {
        CharacterSheetController prefab = PrefabLoader.Singleton.GetPrefab(characterType, gender);
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

    void SpawnFriday() {
        Vector3 lowestPlayerPosition = positions[GameSettings.PlayerCount - 1];
        Vector3 fridayPosition = new Vector3(lowestPlayerPosition.x - 0.5f, lowestPlayerPosition.y - 0.8f, 0f);
        Transform friday = Instantiate(fridayPrefab, characterSheetArea, false);
        friday.localPosition = fridayPosition;
    }
    void SpawnDog() {
        Vector3 lowestPlayerPosition = positions[GameSettings.PlayerCount - 1];
        Vector3 dogPosition = new Vector3(lowestPlayerPosition.x + 0.5f, lowestPlayerPosition.y - 0.8f, 0f);
        Transform dog = Instantiate(dogPrefab, characterSheetArea, false);
        dog.localPosition = dogPosition;
    }
}
