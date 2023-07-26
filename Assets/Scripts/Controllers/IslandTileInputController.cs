using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTileInputController : MonoBehaviour
{
    [SerializeField] IslandTileController islandTileController;
    [SerializeField] Transform islandTileArea;
    [SerializeField] Color colour = Color.white;

    private int campTileLocation;
    bool isActive;
    MeshRenderer MeshRenderer;

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
        EventGenerator.Singleton.AddListenerToChooseAdjacentTileEvent(OnChooseAdjacentTileEvent);
        EventGenerator.Singleton.AddListenerToSpawnIslandTileTokenEvent(OnSpawnIslandTileTokenEvent);
        islandTileArea = GameObject.Find("IslandTileArea").transform;
        MeshRenderer = GetComponent<MeshRenderer>();
    }

    void Start() {
        campTileLocation = ScenarioManager.Singleton.GetStartingIslandTileLocation(); // Replace this at some point
    }

    void OnChooseAdjacentTileEvent(bool isActive) {
        if (transform.parent == null || transform.parent != islandTileArea) {
            return;
        }
        IslandTile islandTile;
        if (isActive == false) {
            islandTile = islandTileController.GetIslandTile();
            MeshRenderer.material = islandTile.Material;
            this.isActive = false;
            return;
        }
        int locationId = islandTileController.GetLocationId();
        if (!adjacencyMatrix[campTileLocation, locationId]) {
            return;
        }
        islandTile = islandTileController.GetIslandTile();
        MeshRenderer.material = islandTile.HighlightedMaterial;
        this.isActive = true;
    }

    void OnSpawnIslandTileTokenEvent(TokenType tokenType, int locationId) {
        if (tokenType == TokenType.Camp) {
            campTileLocation = locationId;
        }
    }

    void OnMouseDown() {
        if (!isActive) {
            return;
        }
        EventGenerator.Singleton.RaiseChooseAdjacentTileEvent(false);
        int locationId = islandTileController.GetLocationId();
        EventGenerator.Singleton.RaiseAdjacentTileChosenEvent(true, locationId);
    }
}
