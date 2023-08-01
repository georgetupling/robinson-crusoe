using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventionCardManager : MonoBehaviour
{
    private static InventionCardManager singleton;

    private List<InventionCard> inventionCards = new List<InventionCard>();
    private Stack<InventionCard> inventionCardDeck = new Stack<InventionCard>();
    private Dictionary<int, InventionCardController> inventionCardsInPlay = new Dictionary<int, InventionCardController>();
    private Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();

    [SerializeField] Transform inventionCardArea;
    [SerializeField] InventionCardController inventionCardPrefab;

    private const int NumberOfPositions = 18;
    private const int NumberOfCardsDrawnDuringSetup = 5;

    private List<Invention> builtInventions = new List<Invention>();
    
    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
        InitializeInventionCards();
        InitializeInventionCardsInPlay();
        InitializePositions();
        InitializeInventionCardDeck();
        SpawnStartingInventionCards();
        EventGenerator.Singleton.AddListenerToInventionIsBuiltEvent(OnInventionIsBuiltEvent);
        EventGenerator.Singleton.AddListenerToGetInventionCardEvent(OnGetInventionCardEvent);
        EventGenerator.Singleton.AddListenerToPersonalInventionSpawnedEvent(OnPersonalInventionSpawnedEvent);
        EventGenerator.Singleton.AddListenerToDrawInventionCardsAndChooseOneEvent(OnDrawInventionCardsAndChooseOneEvent);
        EventGenerator.Singleton.AddListenerToInventionCardChosenFromSelectionEvent(OnInventionCardChosenFromSelectonEvent);
    }

    void Start() {
        
    }

    void InitializeInventionCards() {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.Combine("Data", "invention-cards"));
        string jsonString = jsonTextAsset.text;
        List<string> jsonStrings = Json.ToList(jsonString);
        foreach (string str in jsonStrings) {
            InventionCardUnprocessedData data = JsonUtility.FromJson<InventionCardUnprocessedData>(str);
            InventionCard inventionCard = new InventionCard(data);
            inventionCards.Add(inventionCard);
        }
    }

    void InitializeInventionCardsInPlay() {
        for (int i = 0; i < NumberOfPositions; i++) {
            inventionCardsInPlay[i] = null;
        }
    }

    void InitializePositions() {
        if (inventionCardArea == null) {
            Debug.LogError("Invention card area not found.");
            return;
        }
        for (int i = 0; i < NumberOfPositions; i++) {
            string transformName = "Position" + i;
            Transform positionTransform = inventionCardArea.Find(transformName);
            if (positionTransform == null) {
                Debug.LogError($"{transformName} not found as child of parent transform.");
                return;
            }
            positions.Add(i, positionTransform.localPosition);
        }
    }

    void InitializeInventionCardDeck() {
        foreach(InventionCard inventionCard in inventionCards) {
            if (!inventionCard.isDefaultInvention && !inventionCard.isPersonalInvention) {
                inventionCardDeck.Push(inventionCard);
            }
        }
        DeckShuffler.Singleton.ShuffleDeck(inventionCardDeck);
    }

    void SpawnStartingInventionCards() {
        foreach(InventionCard inventionCard in inventionCards) {
            if (inventionCard.isDefaultInvention) {
                SpawnInventionCard(inventionCard, inventionCardArea);
            }
        }
        for (int i = 0; i < NumberOfCardsDrawnDuringSetup; i++) {
            InventionCard inventionCard = inventionCardDeck.Pop();
            SpawnInventionCard(inventionCard, inventionCardArea);
        }
    }

    void SpawnInventionCard(InventionCard inventionCard, Transform parentTransform) {
        InventionCardController newCard = Instantiate(inventionCardPrefab, parentTransform, false);
        if (parentTransform == inventionCardArea) {
            for (int i = 0; i < NumberOfPositions; i++) {
                if (inventionCardsInPlay[i] == null) {
                    inventionCardsInPlay[i] = newCard;
                    newCard.transform.localPosition = positions[i];
                    break;
                }
            }
        } else {
            newCard.transform.localPosition = Vector3.zero;
        }
        newCard.transform.localRotation = Quaternion.identity;
        newCard.InitializeCard(inventionCard);
    }

    void OnDrawInventionCardsAndChooseOneEvent(int numberOfCards) {
        List<InventionCard> drawnCards = new List<InventionCard>();
        for (int i = 0; i < numberOfCards; i++) {
            InventionCard drawnCard = inventionCardDeck.Pop();
            drawnCards.Add(drawnCard);
        }
        EventGenerator.Singleton.RaiseSpawnChooseInventionCardPopupEvent(drawnCards);
    }

    void OnInventionCardChosenFromSelectonEvent(List<InventionCard> inventionCards, int indexOfChosenCard) {
        for (int i = 0; i < inventionCards.Count; i++) {
            if (i == indexOfChosenCard) {
                SpawnInventionCard(inventionCards[i], inventionCardArea);
            } else {
                inventionCardDeck.Push(inventionCards[i]);
            }
        }
        DeckShuffler.Singleton.ShuffleDeck(inventionCardDeck);
    }

    
    // Tracks built inventions and responds to queries about them
    
    void OnUpdateBuiltInventions(Invention invention, bool isBuilt) {
        if (isBuilt) {
            builtInventions.Add(invention);
        } else {
            builtInventions.Remove(invention);
        }
    }
    
    void OnInventionIsBuiltEvent(string eventType, Invention invention, bool response) {
        if (eventType != InventionIsBuiltEvent.Query) {
            return;
        }
        bool isBuilt = builtInventions.Contains(invention);
        EventGenerator.Singleton.RaiseInventionIsBuiltResponseEvent(invention, isBuilt);
    }

    // Responds to queries for InventionCard objects
    
    void OnGetInventionCardEvent(string eventType, Invention invention, InventionCard inventionCard) {
        if (eventType == GetInventionCardEvent.Query) {
            HandleGetInventionCardQuery(invention);
        }
    }

    void HandleGetInventionCardQuery(Invention invention) {
        InventionCard inventionCard = inventionCards.Find(x => x.invention == invention);
        if (inventionCard == null) {
            Debug.LogError("Invention card not found.");
            return;
        }
        EventGenerator.Singleton.RaiseGetInventionCardResponseEvent(invention, inventionCard);
    }

    // Initialises personal inventions

    void OnPersonalInventionSpawnedEvent(InventionCardController inventionCardController, Invention invention) {
        InventionCard inventionCard = inventionCards.Find(x => x.invention == invention);
        if (inventionCard == null) {
            Debug.LogError("Failed to find personal invention card {invention}.");
            return;
        }
        inventionCardController.InitializeCard(inventionCard);
    }
}
