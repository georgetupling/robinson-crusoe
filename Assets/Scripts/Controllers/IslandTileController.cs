using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static IslandTileTokenController;
using static IslandTileTokenController.Position;
using static TokenType;

/*
    This class is repsonsible for controlling the movement of the island tile it is attached to.
    It also spawns and destroys tokens on the island tile.
    This class also contains the code for the production phase.
    I know that's a lot of responsibilities for one class; this is on my watch list for refactoring.
*/

public class IslandTileController : ComponentController
{
    public IslandTile IslandTile { get; private set; }

    private MeshRenderer meshRenderer;
    private Material material;

    private Transform islandTileArea;
    public int tileLocationId { get; private set; }
    public bool IsCampTile { get; private set; }

    private Dictionary<int, Vector3> tileLocations = new Dictionary<int, Vector3>();
    private Dictionary<Position, TokenController> positionTokenMap = new Dictionary<Position, TokenController>();
    const int NumberOfTileLocations = 10;
    const int TileDeckLocationId = -1;

    // Other classes to initialize
    [SerializeField] private GatherActionSpaceController leftGatherActionSpace;
    [SerializeField] private GatherActionSpaceController rightGatherActionSpace;

    // Popup area
    private Transform popupArea;

    // Fields for resolving certain effects
    private bool shelterIsBuilt;
    private bool snareIsBuilt;
    private bool pitIsBuilt;
    private bool sackIsBuilt;
    private bool sackUsedThisRound;
    private bool basketIsBuilt;
    private bool basketUsedThisRound;
    private int additionalResource = 0;
    private bool shortcutIsBuilt;

    // E.g. if you have 2 effects both giving you additional food tokens, the first will be spawned, the second will go in the list
    // Then if you lose one of those effects, DestroyToken will check the list before destroying the token
    private List<TokenType> tokenLimitOverflow = new List<TokenType>();
    List<TokenType> tokensLimitedToOneCopy = new List<TokenType> { TokenType.AdditionalFood, TokenType.AdditionalWood, TokenType.Shortcut };

    // For determining whether this tile is adjacent to camp
    private int campTileLocation;

    protected override void Awake()
    {
        base.Awake();
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer component not found on IslandTileController.");
        }
        material = meshRenderer.material;
        islandTileArea = GameObject.Find("IslandTileArea").transform;
        tileLocationId = TileDeckLocationId;
        IsCampTile = false;
        popupArea = GameObject.Find("Popups").transform;
        InitializeTileLocations();
        TurnFaceDown();
        EventGenerator.Singleton.AddListenerToMoveIslandTileEvent(OnMoveIslandTileEvent);
        EventGenerator.Singleton.AddListenerToSpawnIslandTileTokenEvent(OnSpawnIslandTileTokenEvent);
        EventGenerator.Singleton.AddListenerToDestroyIslandTileTokenEvent(OnDestroyIslandTileTokenEvent);
        EventGenerator.Singleton.AddListenerToCampHasNaturalShelterEvent(OnCampHasNaturalShelterEvent);
        EventGenerator.Singleton.AddListenerToIslandTileDrawnEvent(OnIslandTileDrawn);
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToSpritePopupClosedEvent(OnSpritePopupClosedEvent);
        EventGenerator.Singleton.AddListenerToGetTokensOnIslandTileEvent(OnGetTokensOnIslandTileEvent);
        EventGenerator.Singleton.AddListenerToGatherSuccessEvent(OnGatherSuccessEvent);
        EventGenerator.Singleton.AddListenerToShelterIsBuiltResponseEvent(OnShelterIsBuiltResponseEvent);
        EventGenerator.Singleton.AddListenerToSpawnTokenOnCampEvent(OnSpawnTokenOnCampEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
        EventGenerator.Singleton.AddListenerToAdditionalResourceFromGatherEvent(OnAdditionalResourceFromGatherEvent);
        EventGenerator.Singleton.AddListenerToExhaustSourceByIslandTileIdEvent(OnExhaustSourceByIslandTileIdEvent);
        EventGenerator.Singleton.AddListenerToAdjacentTileChosenEvent(OnAdjacentTileChosenEvent);
    }

    void OnCampHasNaturalShelterEvent()
    {
        if (IsCampTile)
        {
            EventGenerator.Singleton.RaiseCampHasNaturalShelterResponseEvent(IslandTile.HasNaturalShelter);
        }
    }

    void OnShelterIsBuiltResponseEvent(bool shelterIsBuilt)
    {
        this.shelterIsBuilt = shelterIsBuilt;
    }

    void InitializeTileLocations()
    {
        for (int i = 0; i < NumberOfTileLocations; i++)
        {
            string childName = "Location" + i;
            Transform locationTransform = islandTileArea.Find(childName);
            if (locationTransform == null)
            {
                Debug.LogError($"{childName} not found. Skipping island tile location initialization.");
            }
            else
            {
                tileLocations[i] = locationTransform.localPosition;
            }
        }
    }

    public void InitializeIslandTile(IslandTile islandTile)
    {
        if (IslandTile == null)
        {
            IslandTile = islandTile;
            material = IslandTile.Material;
            meshRenderer.material = material;
            leftGatherActionSpace.SetIslandTile(IslandTile);
            rightGatherActionSpace.SetIslandTile(IslandTile);
        }
        else
        {
            Debug.LogError("IslandTile is already initialized.");
        }
    }

    void OnMoveIslandTileEvent(int componentId, int locationId)
    {
        if (componentId != this.ComponentId)
        {
            return;
        }
        if (locationId == tileLocationId)
        {
            Debug.LogError($"Island tile {IslandTile.Id} is already in location {locationId}.");
            return;
        }
        MoveToLocalPosition(tileLocations[locationId], MoveStyle.Instant);
        TurnFaceUp();
        tileLocationId = locationId;
        EventGenerator.Singleton.RaiseEnableExploreActionAreaEvent(tileLocationId, false);
        EventGenerator.Singleton.RaiseEnableGatherActionAreaEvent(IslandTile.Id, true);
        EventGenerator.Singleton.RaiseTerrainRevealedEvent(IslandTile.TerrainType, true);
    }

    void OnIslandTileDrawn(int componentId, int locationId)
    {
        if (componentId == this.ComponentId)
        {
            tileLocationId = locationId;
            RevealTile();
        }
    }

    void OnSpawnIslandTileTokenEvent(TokenType tokenType, int locationId)
    {
        if (tokenType == TokenType.Camp)
        {
            campTileLocation = locationId;
            if (!IsCampTile && locationId == tileLocationId)
            {
                SpawnToken(TokenType.Camp, misc1);
                if (snareIsBuilt)
                {
                    SpawnToken(TokenType.AdditionalFood, misc1);
                }
                if (shortcutIsBuilt)
                {
                    SpawnToken(TokenType.Shortcut, misc1);
                }
                IsCampTile = true;
                EventGenerator.Singleton.RaiseCampMovedEvent();
            }
            else if (IsCampTile && locationId != tileLocationId)
            {
                DestroyToken(TokenType.Camp);
                // Destroys the snare's additional food token if it's built (since it moves with the camp)
                if (snareIsBuilt)
                {
                    DestroyToken(TokenType.AdditionalFood);
                }
                IsCampTile = false;
            }
            else if (shortcutIsBuilt && IsAdjacentToCampTile())
            {
                // If the camp moves and this tile contains the shortcut, thise code deletes the shortcut token
                bool tileHasShortcutToken = false;
                foreach (TokenController token in positionTokenMap.Values)
                {
                    if (token.tokenType == TokenType.Shortcut)
                    {
                        tileHasShortcutToken = true;
                        break;
                    }
                }
                if (tileHasShortcutToken)
                {
                    DestroyToken(TokenType.Shortcut);
                }
            }
        }
        else if (locationId == tileLocationId)
        {
            SpawnToken(tokenType, misc1);
        }
    }

    void OnDestroyIslandTileTokenEvent(int islandTileId, TokenType tokenType)
    {
        DestroyToken(tokenType);
    }

    void OnGetTokensOnIslandTileEvent(string eventType, int islandTileId, List<TokenType> tokenTypes)
    {
        if (eventType == GetTokensOnIslandTileEvent.Query && this.IslandTile.Id == islandTileId)
        {
            HandleGetTokensQuery();
        }
    }

    void OnSpritePopupClosedEvent(int sourceComponentId)
    {
        if (this.ComponentId != sourceComponentId)
        {
            return;
        }
        transform.SetParent(islandTileArea, true);
        MoveToLocalPosition(tileLocations[tileLocationId], MoveStyle.Slow);
        if (IslandTile.HasBeastCard)
        {
            EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Beast);
        }
        if (IslandTile.HasTotem)
        {
            // TODO - raise apply totem effect event
        }
        EventGenerator.Singleton.RaiseDrawDiscoveryTokenEvent(IslandTile.NumberOfDiscoveryTokens);
        EventGenerator.Singleton.RaiseEnableExploreActionAreaEvent(tileLocationId, false);
        EventGenerator.Singleton.RaiseEnableGatherActionAreaEvent(IslandTile.Id, true);
        EventGenerator.Singleton.RaiseTerrainRevealedEvent(IslandTile.TerrainType, true);
    }

    void OnPhaseStartEvent(Phase phase)
    {
        if (phase == Phase.Production)
        {
            if (IsCampTile)
            {
                StartCoroutine(ApplyProductionPhase());
            }
            else if (IsAdjacentToCampTile())
            {
                bool tileHasShortcutToken = false;
                foreach (TokenController token in positionTokenMap.Values)
                {
                    if (token.tokenType == TokenType.Shortcut)
                    {
                        tileHasShortcutToken = true;
                        break;
                    }
                }
                if (tileHasShortcutToken)
                {
                    // If there is only one source on the shortcut tile, you automatically get 1 resource of that type
                    if (IslandTile.Sources.Count == 1)
                    {
                        Source source = IslandTile.Sources[0];
                        if (source == Source.Wood)
                        {
                            EventGenerator.Singleton.RaiseGainWoodEvent(1);
                        }
                        else
                        {
                            EventGenerator.Singleton.RaiseGainFoodEvent(1);
                        }
                    }
                    else if (IslandTile.Sources.Count == 2)
                    {
                        EventGenerator.Singleton.RaiseSpawnShortcutPopupEvent();
                    }
                }
            }
        }
    }

    void OnGatherSuccessEvent(int islandTileId, bool isRightSource)
    {
        StartCoroutine(ApplyGatherSuccess(islandTileId, isRightSource));
    }

    IEnumerator ApplyGatherSuccess(int islandTileId, bool isRightSource)
    {
        if (islandTileId != IslandTile.Id)
        {
            yield break;
        }
        Source source;
        // Island tile 3 has its source on the right. All other tiles with only 1 source have it on the left.
        if (islandTileId == 3)
        {
            source = IslandTile.Sources[0];
        }
        else
        {
            source = isRightSource ? IslandTile.Sources[1] : IslandTile.Sources[0];
        }
        List<TokenType> tokenTypes = new List<TokenType>();
        foreach (TokenController token in positionTokenMap.Values)
        {
            tokenTypes.Add(token.tokenType);
        }
        int additionalWood = tokenTypes.Contains(TokenType.AdditionalWood) ? 1 : 0;
        int additionalFood = tokenTypes.Contains(TokenType.AdditionalFood) ? 1 : 0;

        // If the sack/basket are built and haven't been used this round, asks whether the user want to use them
        additionalResource = 0;
        if (sackIsBuilt && !sackUsedThisRound)
        {
            EventGenerator.Singleton.RaiseSpawnItemActivationPopupEvent(Invention.Sack);
            while (popupArea.childCount > 0)
            {
                yield return null;
            }
        }
        if (basketIsBuilt && !basketUsedThisRound)
        {
            EventGenerator.Singleton.RaiseSpawnItemActivationPopupEvent(Invention.Basket);
            while (popupArea.childCount > 0)
            {
                yield return null;
            }
        }

        if (source == Source.Wood)
        {
            EventGenerator.Singleton.RaiseGainWoodEvent(1 + additionalWood + additionalResource);
        }
        else
        {
            EventGenerator.Singleton.RaiseGainFoodEvent(1 + additionalFood + additionalResource);
        }
    }

    void OnSpawnTokenOnCampEvent(TokenType tokenType)
    {
        if (!IsCampTile)
        {
            return;
        }
        SpawnToken(tokenType, misc1);
    }

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt)
    {
        if (invention == Invention.Snare)
        {
            if (snareIsBuilt && IsCampTile && !isBuilt)
            {
                // Removes the snare's additional food token if the snare is destroyed
                DestroyToken(TokenType.AdditionalFood);
            }
            snareIsBuilt = isBuilt;
        }
        else if (invention == Invention.Pit)
        {
            pitIsBuilt = isBuilt;
        }
        else if (invention == Invention.Sack)
        {
            sackIsBuilt = isBuilt;
        }
        else if (invention == Invention.Basket)
        {
            basketIsBuilt = isBuilt;
        }
        else if (invention == Invention.Shortcut)
        {
            shortcutIsBuilt = isBuilt;
            if (!isBuilt && IsAdjacentToCampTile())
            {
                // Checks if the tile has the shortcut token and deletes it if it is found
                bool tileHasShortcutToken = false;
                foreach (TokenController token in positionTokenMap.Values)
                {
                    if (token.tokenType == TokenType.Shortcut)
                    {
                        tileHasShortcutToken = true;
                        break;
                    }
                }
                if (tileHasShortcutToken)
                {
                    DestroyToken(TokenType.Shortcut);
                }
            }
        }
    }

    void OnTurnStartEvent(int turnStarted)
    {
        sackUsedThisRound = false;
        basketUsedThisRound = false;
    }

    void OnAdditionalResourceFromGatherEvent(Invention itemUsed)
    {
        additionalResource++;
        if (itemUsed == Invention.Sack)
        {
            sackUsedThisRound = true;
        }
        else if (itemUsed == Invention.Basket)
        {
            basketUsedThisRound = true;
        }
    }

    void OnExhaustSourceByIslandTileIdEvent(int islandTileId, Source source, bool isExhausted)
    {
        if (islandTileId != IslandTile.Id)
        {
            return;
        }
        if (!IslandTile.Sources.Contains(source))
        {
            Debug.LogError($"Can't exhaust {source} on island tile {islandTileId} because the tile doesn't have a source of that type.");
            return;
        }
        if (isExhausted)
        {
            if (IslandTile.Sources[0] == source)
            {
                SpawnToken(TokenType.BlackMarker, Position.leftSource);
                leftGatherActionSpace.gameObject.SetActive(false);
            }
            else if (IslandTile.Sources[1] == source)
            {
                SpawnToken(TokenType.BlackMarker, Position.rightSource);
                rightGatherActionSpace.gameObject.SetActive(false);
            }
        }
        else
        {
            if (IslandTile.Sources[0] == source)
            {
                DestroyToken(TokenType.BlackMarker, Position.leftSource);
                leftGatherActionSpace.gameObject.SetActive(true);
            }
            else if (IslandTile.Sources[1] == source)
            {
                DestroyToken(TokenType.BlackMarker, Position.rightSource);
                rightGatherActionSpace.gameObject.SetActive(true);
            }
        }
    }
    void OnAdjacentTileChosenEvent(bool campIsMoving, int locationId)
    {
        if (campIsMoving)
        {
            campTileLocation = locationId;
        }
    }
    IEnumerator ApplyProductionPhase()
    {
        int foodSources = 0;
        int woodSources = 0;
        foreach (Source source in IslandTile.Sources)
        {
            if (source == Source.Wood)
            {
                woodSources++;
            }
            else
            {
                foodSources++;
            }
        }
        // Takes additonal food/wood tokens into account
        List<TokenType> tokenTypes = new List<TokenType>();
        foreach (TokenController token in positionTokenMap.Values)
        {
            tokenTypes.Add(token.tokenType);
        }
        int additionalWood = tokenTypes.Contains(TokenType.AdditionalWood) ? 1 : 0;
        int additionalFood = tokenTypes.Contains(TokenType.AdditionalFood) ? 1 : 0;

        // Rolls for the pit
        while (popupArea.childCount > 0)
        {
            yield return null;
        }
        if (pitIsBuilt)
        {
            EventGenerator.Singleton.RaiseSpawnDicePopupEvent(new List<DieType> { DieType.BuildDamage });
        }
        while (popupArea.childCount > 0)
        {
            yield return null;
        }

        EventGenerator.Singleton.RaiseGainFoodEvent(foodSources + additionalFood);
        EventGenerator.Singleton.RaiseGainWoodEvent(woodSources + additionalWood);
        yield return new WaitForSeconds(1.5f);
        EventGenerator.Singleton.RaiseEndPhaseEvent(Phase.Production);
    }

    void RevealTile()
    {
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        transform.DOMoveZ(transform.position.z - 0.3f, 0.5f)
            .OnKill(() =>
            {
                transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f)
                    .OnKill(() =>
                    {
                        EventGenerator.Singleton.RaiseIslandTileRevealedEvent(this.ComponentId, IslandTile.Sprite);
                        EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                    });
            });
    }

    void SpawnToken(TokenType tokenType, Position position)
    {
        // Checks token limits
        if (tokensLimitedToOneCopy.Contains(tokenType))
        {
            bool alreadyContainsSameTypeToken = false;
            foreach (TokenController token in positionTokenMap.Values)
            {
                if (token.tokenType == tokenType)
                {
                    alreadyContainsSameTypeToken = true;
                    break;
                }
            }
            if (alreadyContainsSameTypeToken)
            {
                // Instead of spawning a second copy, makes a note that another effect is applying the same type of token
                tokenLimitOverflow.Add(tokenType);
                return;
            }
        }

        if (position >= Position.misc1)
        {
            position = FindUnoccupiedPosition(position);
        }
        if (position == Position.None)
        {
            Debug.LogError("All misc positions are occupied. Failed to spawn island tile token.");
            return;
        }
        TokenController prefab = PrefabLoader.Singleton.GetPrefab(tokenType);
        if (prefab == null)
        {
            Debug.LogError("Island tile token prefab not found.");
            return;
        }
        TokenController newToken = Instantiate(prefab, transform, false);
        newToken.transform.SetParent(transform);
        // Some of the prefabs have TokenControllers; this code deletes their scripts and replaces them with IslandTileTokenControllers
        TokenController tokenController = newToken.GetComponent<TokenController>();
        IslandTileTokenController newTokenWithCorrectController;
        if (tokenController != null)
        {
            Destroy(tokenController);
            newTokenWithCorrectController = newToken.gameObject.AddComponent<IslandTileTokenController>();
        }
        else
        {
            newTokenWithCorrectController = newToken as IslandTileTokenController;
        }
        newTokenWithCorrectController.tokenType = tokenType;
        if (!positionTokenMap.ContainsKey(position))
        {
            positionTokenMap.Add(position, newTokenWithCorrectController);
        }
        else
        {
            positionTokenMap[position] = newTokenWithCorrectController;
        }

        EventGenerator.Singleton.RaiseSetTokenPositionEvent(newTokenWithCorrectController.ComponentId, tokenType, position);

        // If the token spawned is the camp token, checks whether the camp is built
        if (tokenType == TokenType.Camp)
        {
            EventGenerator.Singleton.RaiseShelterIsBuiltEvent();
            if (shelterIsBuilt)
            {
                newTokenWithCorrectController.transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
    }

    Position FindUnoccupiedPosition(Position position)
    {
        while (positionTokenMap.ContainsKey(position) && positionTokenMap[position] != null && position != Position.None)
        {
            int nextPosition = (int)position + 1;
            position = (Position)nextPosition;
        }
        return position;
    }

    void DestroyToken(TokenType tokenType)
    {
        DestroyToken(tokenType, Position.None);
    }

    void DestroyToken(TokenType tokenType, Position position)
    {
        // Checks token limits
        if (tokenLimitOverflow.Contains(tokenType))
        {
            // If there are multiple effects applying one token, keep the token
            tokenLimitOverflow.Remove(tokenType);
            return;
        }

        TokenController tokenToDestroy = null;
        if (position != Position.None)
        {
            foreach (TokenController token in positionTokenMap.Values)
            {
                if (token != null && token.tokenType == tokenType && positionTokenMap.ContainsKey(position) && positionTokenMap[position] != null && positionTokenMap[position] == token)
                {
                    tokenToDestroy = token;
                    break;
                }
            }
        }
        else
        {
            foreach (TokenController token in positionTokenMap.Values)
            {
                if (token != null && token.tokenType == tokenType)
                {
                    tokenToDestroy = token;
                    break;
                }
            }
        }
        if (tokenToDestroy != null)
        {
            Destroy(tokenToDestroy.gameObject);
        }
    }

    void HandleGetTokensQuery()
    {
        List<TokenType> tokenTypes = new List<TokenType>();
        foreach (TokenController token in positionTokenMap.Values)
        {
            if (!tokenTypes.Contains(token.tokenType))
            {
                tokenTypes.Add(token.tokenType);
            }
        }
        EventGenerator.Singleton.RaiseGetTokensOnIslandTileResponseEvent(IslandTile.Id, tokenTypes);
    }

    // Returns the island tile Id

    public IslandTile GetIslandTile()
    {
        return IslandTile;
    }

    public int GetLocationId()
    {
        return tileLocationId;
    }

    // Determines if this tile is adjacent to the camp

    bool IsAdjacentToCampTile()
    {
        if (tileLocationId == -1)
        // -1 means the tile is still in the island tile deck
        {
            return false;
        }
        bool[,] adjacencyMatrix = new bool[,]
        {
            { false, true, false, true, true, false, false, false, false, false },
            { true, false, true, false, true, true, false, false, false, false },
            { false, true, false, false, false, true, true, false, false, false },
            { true, false, false, false, true, false, false, true, false, false },
            { true, true, false, true, false, true, false, true, true, false },
            { false, true, true, false, true, false, true, false, true, true },
            { false, false, true, false, false, true, false, false, false, true },
            { false, false, false, true, true, false, false, false, true, false },
            { false, false, false, false, true, true, false, true, false, true },
            { false, false, false, false, false, true, true, false, true, false }
        };
        return adjacencyMatrix[campTileLocation, tileLocationId];
    }
}
