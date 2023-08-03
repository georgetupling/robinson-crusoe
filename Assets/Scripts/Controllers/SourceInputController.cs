using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SourceInputController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] IslandTileController islandTileController;
    [SerializeField] bool isRightSource;

    private int campTileLocation;

    bool isActive;
    InputType activeInputType;

    // Remembers where the corral was built
    int corralIslandTileId = -1;

    private bool[,] adjacencyMatrix = new bool[,] {
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

    void Awake() {
        EventGenerator.Singleton.AddListenerToGetIslandTileInputEvent(OnGetIslandTileInputEvent);
        EventGenerator.Singleton.AddListenerToSpawnIslandTileTokenEvent(OnSpawnIslandTileTokenEvent);
        EventGenerator.Singleton.AddListenerToAdjacentTileChosenEvent(OnAdjacentTileChosenEvent); // only used during the night phase to move the camp
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
    }

    void OnGetIslandTileInputEvent(bool isActive, InputType inputType) {
        if (islandTileController == null || islandTileController.GetLocationId() == -1) {
            // locationId -1 indicates the tile is still in the island tile deck
            return;
        }
        if (inputType == InputType.Corral && GetSource() == Source.Parrot && IsAdjacentToCamp()) {
            this.isActive = isActive;
            activeInputType = inputType;
        }
    }

    void OnSpawnIslandTileTokenEvent(TokenType tokenType, int locationId) {
        if (tokenType == TokenType.Camp) {
            campTileLocation = locationId;
        }
    }

    void OnAdjacentTileChosenEvent(bool campIsMoving, int locationId) {
        if (campIsMoving) {
            campTileLocation = locationId;
        }
    }

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt) {
        if (invention == Invention.Corral && !isBuilt && corralIslandTileId == GetIslandTileId()) {
            EventGenerator.Singleton.RaiseExhaustSourceByIslandTileIdEvent(corralIslandTileId, Source.Parrot, false);
            EventGenerator.Singleton.RaiseDestroyIslandTileTokenEvent(campTileLocation, TokenType.AdditionalFood);
            corralIslandTileId = -1;
        }
    }

    public void OnPointerClick(PointerEventData data) {
        if (!isActive) {
            return;
        }
        if (activeInputType == InputType.Corral) {
            int islandTileId = GetIslandTileId();
            corralIslandTileId = islandTileId;
            EventGenerator.Singleton.RaiseExhaustSourceByIslandTileIdEvent(islandTileId, Source.Parrot, true);
            EventGenerator.Singleton.RaiseSpawnTokenOnCampEvent(TokenType.AdditionalFood);
            EventGenerator.Singleton.RaiseGetIslandTileInputEvent(false, InputType.Corral);
        }
    }

    // Helper methods

    Source GetSource() {
        if (islandTileController == null) {
            Debug.Log("islandTileController is null.");
            return Source.None;
        }
        IslandTile islandTile = islandTileController.GetIslandTile();
        Source source;
        if (isRightSource) {
            if (islandTile.Sources.Count < 2) {
                source = Source.None;
            } else {
                source = islandTile.Sources[1];
            }
        } else {
            if (islandTile.Sources.Count == 0) {
                source = Source.None;
            } else {
                source = islandTile.Sources[0];
            }
        }
        return source;
    }

    int GetIslandTileId() {
        if (islandTileController == null) {
            Debug.Log("islandTileController is null.");
            return -1;
        }
        IslandTile islandTile = islandTileController.GetIslandTile();
        return islandTile.Id;
    }

    bool IsAdjacentToCamp() {
        int locationId = islandTileController.GetLocationId();
        return adjacencyMatrix[campTileLocation, locationId];
    }
}
