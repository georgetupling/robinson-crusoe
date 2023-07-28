using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckTokenSpawner : MonoBehaviour
{
    [SerializeField] Deck deck;

    Transform deckTokenArea;
    
    Transform largeTokenPosition;
    Transform smallTokenPosition0;
    Transform smallTokenPosition1;

    List<TokenController> tokens = new List<TokenController>();

    const float CardThickness = 0.005f;
    
    void Awake() {
        deckTokenArea = GetComponent<Transform>();
        largeTokenPosition = transform.Find("LargeTokenPosition");
        smallTokenPosition0 = transform.Find("SmallTokenPosition0");
        smallTokenPosition1 = transform.Find("SmallTokenPosition1");
        EventGenerator.Singleton.AddListenerToCardSpawnedEvent(OnCardSpawnedEvent);
        EventGenerator.Singleton.AddListenerToCardDrawnEvent(OnCardDrawnEvent);
        EventGenerator.Singleton.AddListenerToSpawnTokenOnDeckEvent(OnSpawnTokenOnDeckEvent);
        EventGenerator.Singleton.AddListenerToDestroyTokenOnDeckEvent(OnDestroyTokenOnDeckEvent);
    }

    // Updates the deck token area's position as cards are spawned/drawn

    void OnCardSpawnedEvent(Deck deck) {
        if (deck == this.deck) {
            MoveDeckTokenArea(-CardThickness);
        }
    }

    void OnCardDrawnEvent(Deck deck, int componentId) {
        if (deck == this.deck) {
            MoveDeckTokenArea(CardThickness);
        }
    }

    void MoveDeckTokenArea(float distance) {
        Vector3 originalPosition = deckTokenArea.localPosition;
        deckTokenArea.localPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + distance);
    }

    // Spawns tokens in response to SpawnTokenOnDeckEvent

    void OnSpawnTokenOnDeckEvent(Deck deck, TokenType tokenType) {
        if (deck == this.deck) {
            SpawnToken(tokenType);
        }
    }

    void SpawnToken(TokenType tokenType) {
        tokens.RemoveAll(x => x == null);
        bool tokenIsLarge = false;
        if (tokenType == TokenType.BuildAdventure || tokenType == TokenType.GatherAdventure || tokenType == TokenType.ExploreAdventure) {
            tokenIsLarge = true;
        }
        Transform parentTransform = AssignParentTransform(tokenIsLarge);
        if (parentTransform == null) {
            Debug.LogError($"Failed to find available position to spawn {tokenType} on {deck} deck.");
            return;
        }
        TokenController prefab = PrefabLoader.Singleton.GetPrefab(tokenType);
        if (prefab == null) {
            Debug.LogError($"Failed to load {tokenType} token prefab.");
            return;
        }
        TokenController newToken = Instantiate(prefab, parentTransform, false);
        tokens.Add(newToken);
    }

    // Destroys tokens in response to DestroyTokenOnDeckEvent

    void OnDestroyTokenOnDeckEvent(Deck deck, TokenType tokenType) {
        if (deck == this.deck) {
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
        Debug.LogError($"{deck} deck does not have a {tokenType} token spawned.");
    }

    // Helper methods

    Transform AssignParentTransform(bool tokenIsLarge) {
        if (tokenIsLarge) {
            if (!PositionIsOccupied(largeTokenPosition)) {
                return largeTokenPosition;
            } else {
                return null;
            }
        } else {
            if (!PositionIsOccupied(smallTokenPosition0)) {
                return smallTokenPosition0;
            } else if (!PositionIsOccupied(smallTokenPosition1)) {
                return smallTokenPosition1;
            } else if (!PositionIsOccupied(largeTokenPosition)) {
                return largeTokenPosition;
            } else {
                return null;
            }
        }
    }
    
    bool PositionIsOccupied(Transform positionTransform) {
        foreach(TokenController token in tokens) {
            if (token != null && token.transform.parent == positionTransform) {
                return true;
            }
        }
        return false;
    }
}
