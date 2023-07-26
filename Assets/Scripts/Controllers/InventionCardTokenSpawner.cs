using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventionCardTokenSpawner : MonoBehaviour
{
    InventionCard inventionCard;
    int numberOfRequirements;
    
    // Positions for cards with only 1 requirement
    Transform centrePosition;

    // Positions for cards with 2 requirements
    Transform leftPosition;
    Transform rightPosition;

    // Positions for cards with 3 requirements (the bottom is position is omitted because tokens aren't spawned there)
    Transform topLeftPosition;
    Transform topRightPosition;

    List<TokenController> tokens = new List<TokenController>();

    List<(Terrain, bool)> queuedTerrainEvents = new List<(Terrain, bool)>();
    List<(Invention, bool)> queuedInventionEvents = new List<(Invention, bool)>();
    
    void Awake() {
        centrePosition = transform.Find("CentrePosition");
        leftPosition = transform.Find("LeftPosition");
        rightPosition = transform.Find("RightPosition");
        topLeftPosition = transform.Find("TopLeftPosition");
        topRightPosition = transform.Find("TopRightPosition");
        EventGenerator.Singleton.AddListenerToTerrainRequirementMetEvent(OnTerrainRequirementMetEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
    }

    // Method for initializing the token spawner

    public void Initialize(InventionCard inventionCard) {
        this.inventionCard = inventionCard;
        numberOfRequirements = 0;
        if (inventionCard.terrainTypeRequirement != Terrain.None) {
            numberOfRequirements++;
        }
        numberOfRequirements += inventionCard.resourceCosts.Count;
        numberOfRequirements += inventionCard.itemRequirements.Count;
        // Go through queued events in order
        foreach ((Terrain, bool) queuedTerrainEvent in queuedTerrainEvents) {
            OnTerrainRequirementMetEvent(queuedTerrainEvent.Item1, queuedTerrainEvent.Item2);
            queuedTerrainEvents.Clear();
        }
        foreach ((Invention, bool) queuedInventionEvent in queuedInventionEvents) {
            OnUpdateBuiltInventionsEvent(queuedInventionEvent.Item1, queuedInventionEvent.Item2);
            queuedInventionEvents.Clear();
        }
    }

    // Spawns/destroys the black marker on the terrain symbol

    void OnTerrainRequirementMetEvent(Terrain terrainType, bool requirementMet) {
        if (inventionCard == null) {
            // if the token spawner hasn't been initialized, queue the requirement met event
            queuedTerrainEvents.Add((terrainType, requirementMet));
            return;
        }
        if (inventionCard.terrainTypeRequirement != terrainType) {
            return;
        }
        Transform terrainPosition = FindTerrainPosition();
        if (requirementMet) {
            SpawnToken(TokenType.BlackMarker, terrainPosition);
        } else {
            DestroyToken(TokenType.BlackMarker, terrainPosition);
        }
    }

    // Spawns/destroys the black marker on invention requirements

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt) {
        if (inventionCard == null) {
            queuedInventionEvents.Add((invention, isBuilt));
            return;
        }
        if (!inventionCard.itemRequirements.Contains(invention)) {
            return;
        }
        Transform inventionPosition = FindInventionPosition(invention);
        if (isBuilt) {
            SpawnToken(TokenType.BlackMarker, inventionPosition);
        } else {
            DestroyToken(TokenType.BlackMarker, inventionPosition);
        }
    }

    void SpawnToken(TokenType tokenType, Transform parentTransform) {
        if (parentTransform == null || parentTransform.childCount > 0) {
            Debug.LogError($"Failed to find position to spawn {tokenType} on {inventionCard.invention}.");
            return;
        }
        TokenController prefab = PrefabLoader.Singleton.GetPrefab(tokenType);
        if (prefab == null) {
            Debug.LogError($"Failed to load {tokenType} token prefab.");
            return;
        }
        TokenController newToken = Instantiate(prefab, parentTransform, false);
        Vector3 localPosition = new Vector3(0f, 0f, -0.5f * ComponentDimensions.GetHeight(tokenType));
        EventGenerator.Singleton.RaiseMoveComponentEvent(newToken.ComponentId, parentTransform, localPosition, MoveStyle.Instant);
        tokens.Add(newToken);
    }

    void DestroyToken(TokenType tokenType, Transform parentTransform) {
        foreach(TokenController token in tokens) {
            if (token.tokenType == tokenType && token.transform.parent == parentTransform) {
                tokens.Remove(token);
                Destroy(token.gameObject);
                return;
            }
        }
        Debug.LogError($"Failed to destroy {tokenType} token on {inventionCard.invention}.");
    }

    // Helper methods

    Transform FindTerrainPosition() {
        if (numberOfRequirements == 1) {
            return centrePosition;
        } else if (numberOfRequirements == 2) {
            return leftPosition;
        } else if (numberOfRequirements == 3) {
            return topLeftPosition;
        } else {
            Debug.LogError("Invalid number of requirements.");
            return null;
        }
    }

    Transform FindInventionPosition(Invention invention) {
        if (inventionCard.terrainTypeRequirement == Terrain.None) {
            if (numberOfRequirements == 1) {
                return centrePosition;
            } else if (numberOfRequirements == 2) {
                return inventionCard.itemRequirements.IndexOf(invention) == 0 ? leftPosition : rightPosition;
            } else if (numberOfRequirements == 3) {
                return inventionCard.itemRequirements.IndexOf(invention) == 0 ? topLeftPosition : topRightPosition;
            }
        } else {
            if (numberOfRequirements == 2) {
                return rightPosition;
            } else if (numberOfRequirements == 3) {
                return topRightPosition;
            }
        }
        Debug.LogError("Invalid combination of number of requirements and terrain type.");
        return null;
    }
}
