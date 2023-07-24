using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheetController : ComponentController
{
    List<TokenController> tokens = new List<TokenController>();

    // Pre-spawned tokens
    [SerializeField] TrackerTokenController healthTrackerToken;
    [SerializeField] ActionPawnController actionPawn0;
    [SerializeField] ActionPawnController actionPawn1;

    // Positions for determination tokens and miscellaneous tokens
    [SerializeField] Transform position0;
    [SerializeField] Transform position1;
    [SerializeField] Transform position2;
    [SerializeField] Transform position3;
    [SerializeField] Transform position4;
    [SerializeField] Transform position5;
    [SerializeField] Transform position6;
    [SerializeField] Transform position7;
    [SerializeField] Transform position8;
    [SerializeField] Transform position9;
    [SerializeField] Transform position10;
    [SerializeField] Transform position11;

    List<Transform> positions;

    // Positions for wound tokens
    [SerializeField] Transform headWoundPosition;
    [SerializeField] Transform armWoundPosition;
    [SerializeField] Transform bellyWoundPosition;
    [SerializeField] Transform legWoundPosition;
    
    int playerId;
    Character character;
    bool isInitialized;

    int firstPlayer;
    bool waitingOnFirstPlayerQuery;

    protected override void Awake() {
        base.Awake();
        positions = new List<Transform> { position0, position1, position2, position3, position4, position5, position6, position7, position8, position9, position10, position11 };
        EventGenerator.Singleton.AddListenerToInitializeCharacterSheetEvent(OnInitializeCharacterSheetEvent);
        EventGenerator.Singleton.AddListenerToDeterminationEvent(OnDeterminationEvent);
        EventGenerator.Singleton.AddListenerToGetFirstPlayerEvent(OnGetFirstPlayerEvent);
    }

    protected override void Start() {
        base.Start();
    }
    
    // Listeners

    void OnInitializeCharacterSheetEvent(int componentId, int playerId, Character character) {
        if (componentId == this.ComponentId) {
            if (isInitialized) {
                Debug.LogError($"Player {playerId + 1}'s character sheet is already initialized.");
                return;
            }
            this.playerId = playerId;
            this.character = character;
            if (GameSettings.PlayerGenders[playerId] == Gender.Female) {
                // TODO
            }
            EventGenerator.Singleton.RaiseInitializeHealthTrackerTokenEvent(healthTrackerToken.ComponentId, playerId, character.maximumHealth);
            EventGenerator.Singleton.RaiseInitializeActionPawnEvent(actionPawn0.ComponentId, playerId);
            EventGenerator.Singleton.RaiseInitializeActionPawnEvent(actionPawn1.ComponentId, playerId);
            isInitialized = true;
        }
    }

    void OnDeterminationEvent(string eventType, int playerId, int amount) {
        if (playerId == DeterminationEvent.FirstPlayer) {
            StartCoroutine(HandleFirstPlayerDeterminationEvent(eventType, amount));
            return;
        }
        if (playerId != this.playerId && playerId != DeterminationEvent.AllPlayers) {
            return;
        }
        if (eventType == DeterminationEvent.GainDetermination) {
            SpawnTokens(TokenType.Determination, amount);
        } else if (eventType == DeterminationEvent.LoseDetermination) {
            DestroyTokens(TokenType.Determination, amount);
        }
    }

    void OnGetFirstPlayerEvent(string eventType, int playerId) {
        if (waitingOnFirstPlayerQuery) {
            firstPlayer = playerId;
            waitingOnFirstPlayerQuery = false;
        }
    }

    // Co-routine that queries for the first player, pauses until it recieves an answer, and then spawns determination if appropriate

    IEnumerator HandleFirstPlayerDeterminationEvent(string eventType, int amount) {
        waitingOnFirstPlayerQuery = true;
        EventGenerator.Singleton.RaiseGetFirstPlayerEvent();
        while (waitingOnFirstPlayerQuery) {
            yield return null;
        }
        if (firstPlayer != this.playerId) {
            yield break;
        }
        if (eventType == DeterminationEvent.GainDetermination) {
            SpawnTokens(TokenType.Determination, amount);
        } else if (eventType == DeterminationEvent.LoseDetermination) {
            DestroyTokens(TokenType.Determination, amount);
        }
    }

    // Methods for spawning and destroying tokens

    void SpawnTokens(TokenType tokenType, int amount) {
        for (int i = 0; i < amount; i++) {
            SpawnToken(tokenType);
        }
    }

    void SpawnToken(TokenType tokenType) {
        Transform parentTransform = AssignParentTransform(tokenType);
        if (parentTransform == null) {
            Debug.LogError($"No position available to spawn {tokenType} token on player {playerId + 1}'s character sheet.");
            return;
        }
        TokenController prefab = PrefabLoader.Singleton.GetPrefab(tokenType);
        if (prefab == null) {
            Debug.LogError($"{tokenType} token prefab does not exist.");
            return;
        }
        TokenController spawnedToken = Instantiate(prefab, parentTransform, false);
        tokens.Add(spawnedToken);
    }

    void DestroyTokens(TokenType tokenType, int amount) {
        for (int i = 0; i < amount; i++) {
            DestroyToken(tokenType);
        }
    }

    void DestroyToken(TokenType tokenType) {
        foreach(TokenController token in tokens) {
            if (token.tokenType == tokenType) {
                tokens.Remove(token);
                Destroy(token.gameObject);
                return;
            }
        }
        Debug.LogError($"Player {playerId + 1}'s character sheet does not have a {tokenType} token spawned.");
    }

    // Helper methods

    Transform AssignParentTransform(TokenType tokenType) {
        List<Transform> unoccupiedPositions = new List<Transform>();
        foreach(Transform position in positions) {
            if (!PositionIsOccupied(position)) {
                unoccupiedPositions.Add(position);
            }
        }
        if (unoccupiedPositions.Count == 0) {
            return null;
        }
        int randomIndex = Random.Range(0, unoccupiedPositions.Count);
        return unoccupiedPositions[randomIndex];
    }

    bool PositionIsOccupied(Transform position) {
        foreach(TokenController token in tokens) {
            if (token.transform.parent == position) {
                return true;
            }
        }
        return false;
    }
}
