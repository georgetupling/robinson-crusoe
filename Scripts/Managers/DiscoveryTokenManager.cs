using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
    This class spawns discovery tokens and manages the discovery token deck.
*/

public class DiscoveryTokenManager : MonoBehaviour
{
    private static DiscoveryTokenManager singleton;

    public List<DiscoveryToken> discoveryTokens = new List<DiscoveryToken>();
    private Stack<DiscoveryTokenController> discoveryTokenDeck = new Stack<DiscoveryTokenController>();

    [SerializeField] private DiscoveryTokenController discoveryTokenPrefab;
    [SerializeField] private Transform discoveryTokenDeckArea;

    private const float TokenThickness = 0.005f;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            return;
        }
        InitializeDiscoveryTokens();
        SpawnDiscoveryTokenDeck();
        EventGenerator.Singleton.AddListenerToDrawDiscoveryTokenEvent(OnDrawDiscoveryTokenEvent);
    }

    // Loads the discovery tokens from discovery-tokens.json

    void InitializeDiscoveryTokens() {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.Combine("Data", "discovery-tokens"));
        string jsonString = jsonTextAsset.text;
        List<string> jsonStrings = Json.ToList(jsonString);
        foreach (string str in jsonStrings) {
            DiscoveryTokenUnprocessedData data = JsonUtility.FromJson<DiscoveryTokenUnprocessedData>(str);
            DiscoveryToken discoveryToken = new DiscoveryToken(data);
            discoveryTokens.Add(discoveryToken);
        }
    }

    // Spawns the discovery token deck

    void SpawnDiscoveryTokenDeck() {
        foreach(DiscoveryToken discoveryToken in discoveryTokens) {
            SpawnDiscoveryToken(discoveryToken);
        }
        DeckShuffler.ShuffleDeck(discoveryTokenDeck, TokenThickness);
    }

    void SpawnDiscoveryToken(DiscoveryToken discoveryToken) {
        DiscoveryTokenController spawnedDiscoveryToken = Instantiate(discoveryTokenPrefab, discoveryTokenDeckArea, false);
        EventGenerator.Singleton.RaiseMoveComponentEvent(
            spawnedDiscoveryToken.ComponentId,
            new Vector3(0, 0, (-1) * discoveryTokenDeck.Count * TokenThickness),
            MoveStyle.Instant
        );
        EventGenerator.Singleton.RaiseInitializeDiscoveryTokenEvent(spawnedDiscoveryToken.ComponentId, discoveryToken);
        discoveryTokenDeck.Push(spawnedDiscoveryToken);
    }

    // Draws discovery tokens

    void OnDrawDiscoveryTokenEvent(int amount) {
        if (amount < 0) {
            Debug.LogError("Amount parameter passed to DrawDiscoveryTokenEvent must be positive.");
            return;
        }
        for (int i = 0; i < amount; i++) {
            DrawDiscoveryToken();
        }
    }

    void DrawDiscoveryToken() {
        DiscoveryTokenController drawnDiscoveryToken = discoveryTokenDeck.Pop();
        EventGenerator.Singleton.RaiseDiscoveryTokenDrawnEvent(drawnDiscoveryToken.ComponentId);
    }
}
