using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridayDeterminationSpawner : MonoBehaviour
{
    // Positions for determination tokens and miscellaneous tokens
    [SerializeField] Transform position0;
    [SerializeField] Transform position1;
    [SerializeField] Transform position2;
    [SerializeField] Transform position3;
    [SerializeField] Transform position4;
    [SerializeField] Transform position5;
    [SerializeField] Transform position6;
    List<Transform> positions;

    TokenController determinationPrefab;
    List<TokenController> tokens = new List<TokenController>();

    const int FridaysPlayerId = 4;

    void Awake()
    {
        positions = new List<Transform> { position0, position1, position2, position3, position4, position5, position6 };
        EventGenerator.Singleton.AddListenerToSetDeterminationTokensEvent(OnSetDeterminationTokensEvent);
    }

    void Start()
    {
        determinationPrefab = PrefabLoader.Singleton.GetPrefab(TokenType.Determination);
    }

    void OnSetDeterminationTokensEvent(int playerId, int determination)
    {
        if (playerId != FridaysPlayerId)
        {
            return;
        }
        int currentDetermination = 0;
        foreach (Transform position in positions)
        {
            if (position.childCount > 0 && position.GetChild(0) != null)
            {
                TokenController token = position.GetChild(0).GetComponent<TokenController>();
                if (token != null && token.tokenType == TokenType.Determination)
                {
                    currentDetermination++;
                }
            }
        }
        int determinationChange = determination - currentDetermination;
        if (determinationChange > 0)
        {
            SpawnTokens(determinationChange);
        }
        else if (determinationChange < 0)
        {
            DestroyTokens(-determinationChange);
        }
    }

    // Code for spawning tokens

    void SpawnTokens(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnToken();
        }
    }

    void SpawnToken()
    {
        Transform parentTransform = AssignParentTransform();
        if (parentTransform == null)
        {
            Debug.LogError($"No position available to spawn Determination token on Friday's character sheet.");
            return;
        }
        TokenController spawnedToken = Instantiate(determinationPrefab, parentTransform, false);
        tokens.Add(spawnedToken);
    }

    // Code for destroying tokens

    void DestroyTokens(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DestroyToken();
        }
    }

    void DestroyToken()
    {
        foreach (TokenController token in tokens)
        {
            if (token.tokenType == TokenType.Determination)
            {
                tokens.Remove(token);
                Destroy(token.gameObject);
                return;
            }
        }
        Debug.LogError($"Friday's character sheet does not have a Determination token spawned.");
    }

    // Helper methods

    Transform AssignParentTransform()
    {
        List<Transform> unoccupiedPositions = new List<Transform>();
        foreach (Transform position in positions)
        {
            if (!PositionIsOccupied(position))
            {
                unoccupiedPositions.Add(position);
            }
        }
        if (unoccupiedPositions.Count == 0)
        {
            return null;
        }
        int randomIndex = Random.Range(0, unoccupiedPositions.Count);
        return unoccupiedPositions[randomIndex];
    }

    bool PositionIsOccupied(Transform position)
    {
        foreach (TokenController token in tokens)
        {
            if (token.transform.parent == position)
            {
                return true;
            }
        }
        return false;
    }
}
