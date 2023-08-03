using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IslandTileInputController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] IslandTileController islandTileController;
    [SerializeField] Transform islandTileArea;
    [SerializeField] Color colour = Color.white;

    private int campTileLocation;
    bool isActive;
    InputType activeInputType;
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

    void Awake()
    {
        EventGenerator.Singleton.AddListenerToGetIslandTileInputEvent(OnGetIslandTileInputEvent);
        EventGenerator.Singleton.AddListenerToSpawnIslandTileTokenEvent(OnSpawnIslandTileTokenEvent);
        islandTileArea = GameObject.Find("IslandTileArea").transform;
        MeshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        campTileLocation = ScenarioManager.Singleton.GetStartingIslandTileLocation(); // Replace this at some point
    }

    void OnGetIslandTileInputEvent(bool isActive, InputType inputType)
    {
        if (transform.parent == null || transform.parent != islandTileArea)
        {
            return;
        }
        if (inputType == InputType.MoveCamp || inputType == InputType.Shortcut)
        {
            // Toggles highlighted texture for tiles adjacent to camp
            IslandTile islandTile;
            if (isActive == false)
            {
                islandTile = islandTileController.GetIslandTile();
                MeshRenderer.material = islandTile.Material;
                this.isActive = false;
                return;
            }
            int locationId = islandTileController.GetLocationId();
            if (!adjacencyMatrix[campTileLocation, locationId])
            {
                return;
            }
            islandTile = islandTileController.GetIslandTile();
            MeshRenderer.material = islandTile.HighlightedMaterial;
            this.isActive = true;
            activeInputType = inputType;
        }
    }

    void OnSpawnIslandTileTokenEvent(TokenType tokenType, int locationId)
    {
        if (tokenType == TokenType.Camp)
        {
            campTileLocation = locationId;
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (!isActive)
        {
            return;
        }
        if (activeInputType == InputType.MoveCamp)
        {
            EventGenerator.Singleton.RaiseGetIslandTileInputEvent(false, InputType.MoveCamp);
            int locationId = islandTileController.GetLocationId();
            EventGenerator.Singleton.RaiseAdjacentTileChosenEvent(true, locationId);
        }
        else if (activeInputType == InputType.Shortcut)
        {
            EventGenerator.Singleton.RaiseGetIslandTileInputEvent(false, InputType.Shortcut);
            int locationId = islandTileController.GetLocationId();
            EventGenerator.Singleton.RaiseSpawnIslandTileTokenEvent(TokenType.Shortcut, locationId);
        }
    }
}
