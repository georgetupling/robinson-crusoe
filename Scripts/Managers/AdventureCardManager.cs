using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AdventureCardManager : MonoBehaviour
{
    private static AdventureCardManager singleton;

    [SerializeField] private List<AdventureCard> buildAdventureCards = new List<AdventureCard>();
    [SerializeField] private List<AdventureCard> gatherAdventureCards = new List<AdventureCard>();
    [SerializeField] private List<AdventureCard> exploreAdventureCards = new List<AdventureCard>();

    private Stack<CardController> buildAdventureDeck = new Stack<CardController>();
    private Stack<CardController> gatherAdventureDeck = new Stack<CardController>();
    private Stack<CardController> exploreAdventureDeck = new Stack<CardController>();

    [SerializeField] private Transform buildAdventureDeckArea;
    [SerializeField] private Transform gatherAdventureDeckArea;
    [SerializeField] private Transform exploreAdventureDeckArea;

    [SerializeField] private AdventureCardController adventureCardPrefab;

    private const float CardThickness = 0.005f;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
        LoadAdventureCards("build-adventure-cards", buildAdventureCards);
        LoadAdventureCards("gather-adventure-cards", gatherAdventureCards);
        LoadAdventureCards("explore-adventure-cards", exploreAdventureCards);
    }

    void Start() {
        SpawnAdventureDecks();
    }

    void LoadAdventureCards(string fileName, List<AdventureCard> list) {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.Combine("Data", fileName));
        string jsonString = jsonTextAsset.text;
        List<string> jsonStrings = Json.ToList(jsonString);
        foreach (string str in jsonStrings) {
            AdventureCardUnprocessedData data = JsonUtility.FromJson<AdventureCardUnprocessedData>(str);
            AdventureCard adventureCard = new AdventureCard(data);
            list.Add(adventureCard);
        }
    }

    public void SpawnAdventureDecks() {
        SpawnAdventureDeck(buildAdventureCards, buildAdventureDeckArea, Deck.BuildAdventure, buildAdventureDeck);
        SpawnAdventureDeck(gatherAdventureCards, gatherAdventureDeckArea, Deck.GatherAdventure, gatherAdventureDeck);
        SpawnAdventureDeck(exploreAdventureCards, exploreAdventureDeckArea, Deck.ExploreAdventure, exploreAdventureDeck);
        DeckShuffler.ShuffleDeck(buildAdventureDeck, CardThickness);
        DeckShuffler.ShuffleDeck(gatherAdventureDeck, CardThickness);
        DeckShuffler.ShuffleDeck(exploreAdventureDeck, CardThickness);
    }

    void SpawnAdventureDeck(List<AdventureCard> list, Transform deckArea, Deck deckType, Stack<CardController> deck) {
        foreach (AdventureCard card in list) {
                AdventureCardController newCard = Instantiate(adventureCardPrefab, deckArea, false);
                newCard.transform.localPosition = new Vector3(0, 0, (-1) * deck.Count * CardThickness);
                newCard.transform.localRotation = Quaternion.identity;
                newCard.InitializeCard(card);
                deck.Push(newCard);
                EventGenerator.Singleton.RaiseCardSpawnedEvent(deckType);
            }
    }
}
