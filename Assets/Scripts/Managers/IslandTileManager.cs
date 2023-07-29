using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IslandTileManager : MonoBehaviour
{
    private static IslandTileManager singleton;

    private List<IslandTile> islandTiles = new List<IslandTile>();

    [SerializeField] private IslandTileController islandTilePrefab;
    [SerializeField] private Transform islandTileArea;
    [SerializeField] private Transform islandTileDeckArea;

    private Stack<IslandTileController> islandTileDeck = new Stack<IslandTileController>();
    private Dictionary<int, IslandTileController> locationIslandTileMap = new Dictionary<int, IslandTileController>();

    private int campTileLocation;

    const float IslandTileThickness = 0.01f;
    private int StartingTileId;
    private int StartingTileLocation;

    private int[,] locationDistanceMatrix = new int[,] {
        { 0, 1, 2, 1, 1, 2, 3, 2, 2, 3 },
        { 1, 0, 1, 2, 1, 1, 2, 2, 2, 2 },
        { 2, 1, 0, 3, 2, 1, 1, 3, 2, 2 },
        { 1, 2, 3, 0, 1, 2, 3, 1, 2, 3 },
        { 1, 1, 2, 1, 0, 1, 2, 1, 1, 2 },
        { 2, 1, 1, 2, 1, 0, 1, 2, 1, 1 },
        { 3, 2, 1, 3, 2, 1, 0, 3, 2, 1 },
        { 2, 2, 3, 1, 1, 2, 3, 0, 1, 2 },
        { 2, 2, 2, 2, 1, 1, 2, 1, 0, 1 },
        { 3, 2, 2, 3, 2, 1, 1, 2, 1, 0 }
    };

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
        InitializeIslandTiles();
    }

    void Start() {
        EventGenerator.Singleton.AddListenerToDrawIslandTileEvent(OnDrawIslandTileEvent);
        EventGenerator.Singleton.AddListenerToSpawnIslandTileTokenEvent(OnSpawnIslandTileTokenEvent);
        EventGenerator.Singleton.AddListenerToGetDistanceFromCampEvent(OnGetDistanceFromCampEvent);
        EventGenerator.Singleton.AddListenerToGetDistanceFromCampToLocationEvent(OnGetDistanceFromCampToLocationEvent);
        EventGenerator.Singleton.AddListenerToLocationIsOccupiedEvent(OnLocationIsOccupiedEvent);
        StartingTileId = ScenarioManager.Singleton.GetStartingIslandTileId();
        StartingTileLocation = ScenarioManager.Singleton.GetStartingIslandTileLocation();
        SpawnStartingIslandTile();
        SpawnIslandTileDeck();
    }

    void InitializeIslandTiles() {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.Combine("Data", "island-tiles"));
        string jsonString = jsonTextAsset.text;
        List<string> jsonStrings = Json.ToList(jsonString);
        foreach (string str in jsonStrings) {
            IslandTileUnprocessedData data = JsonUtility.FromJson<IslandTileUnprocessedData>(str);
            IslandTile islandTile = new IslandTile(data);
            islandTiles.Add(islandTile);
        }
    }

    void OnDrawIslandTileEvent(int locationId) {
        if (locationIslandTileMap.ContainsKey(locationId)) {
            Debug.LogError($"There is already an island tile at location {locationId}.");
            return;
        }
        IslandTileController drawnTile = islandTileDeck.Pop();
        locationIslandTileMap[locationId] = drawnTile;
        EventGenerator.Singleton.RaiseIslandTileDrawnEvent(drawnTile.ComponentId, locationId);
    }

    void OnSpawnIslandTileTokenEvent(TokenType tokenType, int locationId) {
        if (tokenType == TokenType.Camp) {
            campTileLocation = locationId;
        }
    }

    void OnGetDistanceFromCampEvent(string eventType, int islandTileId, int distance) {
        if (eventType == GetDistanceFromCampEvent.Query) {
            HandleDistanceFromCampQuery(islandTileId);
        }
    }

    void OnGetDistanceFromCampToLocationEvent(string eventType, int locationId, int distance) {
        if (eventType == GetDistanceFromCampEvent.Query) {
            HandleDistanceFromCampToLocationQuery(locationId);
        }
    }

    void OnLocationIsOccupiedEvent(string eventType, int locationId, bool isOccupied) {
        if (eventType == LocationIsOccupiedEvent.Query) {
            HandleLocationIsOccupiedQuery(locationId);
        }
    }

    void SpawnStartingIslandTile() {
        IslandTileController newIslandTile = Instantiate(islandTilePrefab, islandTileArea, false);
        IslandTile islandTile = islandTiles.Find(x => x.Id == StartingTileId);
        locationIslandTileMap[StartingTileLocation] = newIslandTile;  
        newIslandTile.InitializeIslandTile(islandTile);
        EventGenerator.Singleton.RaiseMoveIslandTileEvent(newIslandTile.ComponentId, StartingTileLocation);
        EventGenerator.Singleton.RaiseSpawnIslandTileTokenEvent(TokenType.Camp, StartingTileLocation);
    }

    void SpawnIslandTileDeck() {
        foreach (IslandTile islandTile in islandTiles) {
            if (islandTile.Id != StartingTileId) {
                IslandTileController newIslandTile = Instantiate(islandTilePrefab, islandTileDeckArea, false);
                newIslandTile.InitializeIslandTile(islandTile);
                newIslandTile.transform.localPosition = new Vector3 (0, 0, (-1) * islandTileDeck.Count * IslandTileThickness);
                islandTileDeck.Push(newIslandTile);
            }
        }
        DeckShuffler.Singleton.ShuffleDeck(islandTileDeck, IslandTileThickness);
    }

    void HandleDistanceFromCampQuery(int islandTileId) {
        int locationOfQueriedTile = -1;
        foreach (KeyValuePair<int, IslandTileController> kvp in locationIslandTileMap) {
            if (kvp.Value.IslandTile.Id == islandTileId) {
                locationOfQueriedTile = kvp.Key;
                break;
            }
        }
        if (locationOfQueriedTile == -1) {
            Debug.LogError($"No island tile with ID {islandTileId} found.");
            EventGenerator.Singleton.RaiseGetDistanceFromCampResponseEvent(islandTileId, -1);
            return;
        }
        int distance = locationDistanceMatrix[locationOfQueriedTile, campTileLocation];
        EventGenerator.Singleton.RaiseGetDistanceFromCampResponseEvent(islandTileId, distance);
    }

    void HandleDistanceFromCampToLocationQuery(int locationId) {
        if (locationId < 0 || locationId > 9) {
            Debug.LogError("Invalid location ID passed to distance from camp query.");
            EventGenerator.Singleton.RaiseGetDistanceFromCampToLocationResponseEvent(locationId, -1);
            return;
        }
        int distance = locationDistanceMatrix[locationId, campTileLocation];
        EventGenerator.Singleton.RaiseGetDistanceFromCampToLocationResponseEvent(locationId, distance);
    }

    void HandleLocationIsOccupiedQuery(int locationId) {
        if (locationId < 0 || locationId > 9) {
            Debug.LogError("Invalid location ID passed to location is occupied query.");
            EventGenerator.Singleton.RaiseLocationIsOccupiedResponseEvent(locationId, false);
            return;
        }
        bool isOccupied = locationIslandTileMap.ContainsKey(locationId) && locationIslandTileMap[locationId] != null;
        Debug.Log($"isOccupied = {isOccupied}");
        EventGenerator.Singleton.RaiseLocationIsOccupiedResponseEvent(locationId, isOccupied);
    }
}
