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
    private int tileLocationId;
    public bool IsCampTile { get; private set; }

    private Dictionary<int, Vector3> tileLocations = new Dictionary<int, Vector3>();

    private Dictionary<Position, TokenController> positionTokenMap = new Dictionary<Position, TokenController>();

    const int NumberOfTileLocations = 10;
    const int TileDeckLocationId = -1;

    [SerializeField] private GatherActionSpaceController leftGatherActionSpace;
    [SerializeField] private GatherActionSpaceController rightGatherActionSpace;

    private bool shelterIsBuilt;

    protected override void Awake() {
        base.Awake();
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) {
            Debug.LogError("MeshRenderer component not found on IslandTileController.");
        }
        material = meshRenderer.material;
        islandTileArea = GameObject.Find("IslandTileArea").transform;
        tileLocationId = TileDeckLocationId;
        IsCampTile = false;
        InitializeTileLocations();
        TurnFaceDown();
        EventGenerator.Singleton.AddListenerToMoveIslandTileEvent(OnMoveIslandTileEvent);
        EventGenerator.Singleton.AddListenerToSpawnIslandTileTokenEvent(OnSpawnIslandTileTokenEvent);
        EventGenerator.Singleton.AddListenerToCampHasNaturalShelterEvent(OnCampHasNaturalShelterEvent);
    }

    protected override void Start() {
        base.Start();
        EventGenerator.Singleton.AddListenerToIslandTileDrawnEvent(OnIslandTileDrawn);
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToSpritePopupClosedEvent(OnSpritePopupClosedEvent);
        EventGenerator.Singleton.AddListenerToGetTokensOnIslandTileEvent(OnGetTokensOnIslandTileEvent);
        EventGenerator.Singleton.AddListenerToGatherSuccessEvent(OnGatherSuccessEvent);
        EventGenerator.Singleton.AddListenerToShelterIsBuiltResponseEvent(OnShelterIsBuiltResponseEvent);
    }

    void OnCampHasNaturalShelterEvent() {
        if (IsCampTile) {
            EventGenerator.Singleton.RaiseCampHasNaturalShelterResponseEvent(IslandTile.HasNaturalShelter);
        } 
    }

    void OnShelterIsBuiltResponseEvent(bool shelterIsBuilt) {
        this.shelterIsBuilt = shelterIsBuilt;
    }

    void InitializeTileLocations() {
        for (int i = 0; i < NumberOfTileLocations; i++) {
            string childName = "Location" + i;
            Transform locationTransform = islandTileArea.Find(childName);
            if (locationTransform == null) {
                Debug.LogError($"{childName} not found. Skipping island tile location initialization.");
            } else {
                tileLocations[i] = locationTransform.localPosition;
            }
        }
    }

    public void InitializeIslandTile(IslandTile islandTile) {
        if (IslandTile == null) {
            IslandTile = islandTile;
            material = IslandTile.Material;
            meshRenderer.material = material;
            leftGatherActionSpace.SetIslandTile(IslandTile);
            rightGatherActionSpace.SetIslandTile(IslandTile);
        } else {
            Debug.LogError("IslandTile is already initialized.");
        }
    }

    void OnMoveIslandTileEvent(int componentId, int locationId) {
        if (componentId != this.ComponentId) {
            return;
        }
        if (locationId == tileLocationId) {
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

    void OnIslandTileDrawn(int componentId, int locationId) {
        if (componentId == this.ComponentId) {
            tileLocationId = locationId;
            RevealTile();
        }
    }

    void OnSpawnIslandTileTokenEvent(TokenType tokenType, int locationId) {
        if (tokenType == TokenType.Camp && !IsCampTile && locationId == tileLocationId) {
            SpawnToken(Camp, misc1);
            IsCampTile = true;
            EventGenerator.Singleton.RaiseCampMovedEvent();
        } else if (tokenType == TokenType.Camp && IsCampTile && locationId != tileLocationId) {
            DestroyToken(TokenType.Camp);
            IsCampTile = false;
        }
    }
    
    void OnGetTokensOnIslandTileEvent(string eventType, int islandTileId, List<TokenType> tokenTypes) {
        if (eventType == GetTokensOnIslandTileEvent.Query && this.IslandTile.Id == islandTileId) {
            HandleGetTokensQuery();
        }
    }

    void OnSpritePopupClosedEvent(int sourceComponentId) {
        if (this.ComponentId != sourceComponentId) {
            return;
        }
        transform.SetParent(islandTileArea, true);
        MoveToLocalPosition(tileLocations[tileLocationId], MoveStyle.Slow);
        if (IslandTile.HasBeastCard) {
            EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Beast);
        }
        if (IslandTile.HasTotem) {
            // TODO - raise apply totem effect event
        }
        EventGenerator.Singleton.RaiseDrawDiscoveryTokenEvent(IslandTile.NumberOfDiscoveryTokens);
        EventGenerator.Singleton.RaiseEnableExploreActionAreaEvent(tileLocationId, false);
        EventGenerator.Singleton.RaiseEnableGatherActionAreaEvent(IslandTile.Id, true);
        EventGenerator.Singleton.RaiseTerrainRevealedEvent(IslandTile.TerrainType, true);
    }

    void OnPhaseStartEvent(Phase phase) {
        if (phase == Phase.Production && IsCampTile) {
            StartCoroutine(ApplyProductionPhase());
        }
    }

    void OnGatherSuccessEvent(int islandTileId, bool isRightSource) {
        if (islandTileId != IslandTile.Id) {
            return;
        }
        Source source;
        // Island tile 3 has its source on the right. All other tiles with only 1 source have it on the left.
        if (islandTileId == 3) { 
            source = IslandTile.Sources[0];
        } else {
            source = isRightSource ? IslandTile.Sources[1] : IslandTile.Sources[0];
        } 
        List<TokenType> tokenTypes = new List<TokenType>();
        foreach (TokenController token in positionTokenMap.Values) {
            tokenTypes.Add(token.tokenType);
        }
        int additionalWood = tokenTypes.Contains(TokenType.AdditionalWood) ? 1 : 0;
        int additionalFood = tokenTypes.Contains(TokenType.AdditionalFood) ? 1 : 0;
        if (source == Source.Wood) {
            EventGenerator.Singleton.RaiseGainWoodEvent(1 + additionalWood);
        } else {
            EventGenerator.Singleton.RaiseGainFoodEvent(1 + additionalFood);
        }
    }

    IEnumerator ApplyProductionPhase() {
        int foodSources = 0;
        int woodSources = 0;
        foreach (Source source in IslandTile.Sources) {
            if (source == Source.Wood) {
                woodSources++;
            } else {
                foodSources++;
            }
        }
        EventGenerator.Singleton.RaiseGainFoodEvent(foodSources);
        EventGenerator.Singleton.RaiseGainWoodEvent(woodSources);
        yield return new WaitForSeconds(1.5f);
        EventGenerator.Singleton.RaiseEndPhaseEvent(Phase.Production);
    }

    void RevealTile() {
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        transform.DOMoveZ(transform.position.z - 0.3f, 0.5f)
            .OnKill(() => {
            transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f)
                .OnKill(() => 
                {
                EventGenerator.Singleton.RaiseIslandTileRevealedEvent(this.ComponentId, IslandTile.Sprite);
                EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                });
            });
    }

    void SpawnToken(TokenType tokenType, Position position) {
        if (position >= Position.misc1) {
            position = FindUnoccupiedPosition(position);
        }
        if (position == Position.None) {
            Debug.LogError("All misc positions are occupied. Failed to spawn island tile token.");
            return;
        }
        TokenController prefab = PrefabLoader.Singleton.GetPrefab(tokenType);
        if (prefab == null) {
            Debug.LogError("Island tile token prefab not found.");
            return;
        }
        TokenController newToken = Instantiate(prefab, transform, false);
        newToken.transform.SetParent(transform);
        // Some of the prefabs have TokenControllers; this code deletes their scripts and replaces them with IslandTileTokenControllers
        TokenController tokenController = newToken.GetComponent<TokenController>();
        IslandTileTokenController newTokenWithCorrectController;
        if (tokenController != null) {
            Destroy(tokenController);
            newTokenWithCorrectController = newToken.gameObject.AddComponent<IslandTileTokenController>();
        } else {
            newTokenWithCorrectController = newToken as IslandTileTokenController;
        }
        newTokenWithCorrectController.tokenType = tokenType;
        positionTokenMap.Add(position, newTokenWithCorrectController);
        EventGenerator.Singleton.RaiseSetTokenPositionEvent(newTokenWithCorrectController.ComponentId, position);

        // If the token spawned is the camp token, checks whether the camp is built
        if (tokenType == TokenType.Camp) {
            EventGenerator.Singleton.RaiseShelterIsBuiltEvent();
            if (shelterIsBuilt) {
                newTokenWithCorrectController.transform.eulerAngles = new Vector3 (0, 180, 0);
            }
        }
    }

    Position FindUnoccupiedPosition(Position position) {
        while (positionTokenMap.ContainsKey(position) && position != Position.None) {
            int nextPosition = (int) position + 1;
            position = (Position) nextPosition;
        }
        return position;
    }

    void DestroyToken(TokenType tokenType) {
        foreach(TokenController token in positionTokenMap.Values) {
            if (token != null && token.tokenType == tokenType) {
                Destroy(token.gameObject);
                break;
            }
        }
    }

    void HandleGetTokensQuery() {
        List<TokenType> tokenTypes = new List<TokenType>();
        foreach(TokenController token in positionTokenMap.Values) {
            if (!tokenTypes.Contains(token.tokenType)) {
                tokenTypes.Add(token.tokenType);
            }
        }
        EventGenerator.Singleton.RaiseGetTokensOnIslandTileResponseEvent(IslandTile.Id, tokenTypes);
    }

    // Returns the island tile Id

    public IslandTile GetIslandTile() {
        return IslandTile;
    }

    public int GetLocationId() {
        return tileLocationId;
    }
}
