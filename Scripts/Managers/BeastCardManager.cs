using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BeastCardManager : MonoBehaviour
{
    private BeastCardManager singleton;

    private List<BeastCard> beastCards = new List<BeastCard>();

    private Stack<BeastCardController> beastDeck = new Stack<BeastCardController>();
    private Stack<BeastCardController> huntingDeck = new Stack<BeastCardController>();

    [SerializeField] private Transform beastDeckArea;
    [SerializeField] private Transform huntingDeckArea;

    [SerializeField] private BeastCardController beastCardPrefab;

    private float CardThickness = 0.005f;

    void Awake()
    {
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
        InitializeBeastCards();
    }

    void Start() {
        SpawnBeastDeck();
        EventGenerator.Singleton.AddListenerToDrawCardEvent(OnDrawCardEvent);
    }

    void InitializeBeastCards() {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.Combine("Data", "beast-cards"));
        string jsonString = jsonTextAsset.text;
        List<string> jsonStrings = Json.ToList(jsonString);
        foreach (string str in jsonStrings) {
            BeastCardUnprocessedData data = JsonUtility.FromJson<BeastCardUnprocessedData>(str);
            BeastCard beastCard = new BeastCard(data);
            beastCards.Add(beastCard);
        }
    }

    void SpawnBeastDeck() {
        foreach (BeastCard beastCard in beastCards) {
            BeastCardController newCard = Instantiate(beastCardPrefab, beastDeckArea, false);
            Vector3 localPosition = new Vector3(0, 0, (-1) * beastDeck.Count * CardThickness);
            EventGenerator.Singleton.RaiseMoveComponentEvent(newCard.ComponentId, localPosition);
            newCard.InitializeCard(beastCard);
            beastDeck.Push(newCard);
        }
        DeckShuffler.ShuffleDeck(beastDeck, CardThickness);
    }

    void OnDrawCardEvent(Deck deck) {
        if (deck == Deck.Beast) {
            DrawCard();
        }
    }

    void DrawCard() {
        BeastCardController drawnCard = beastDeck.Pop();
        if (huntingDeck.Count == 0) {
            EventGenerator.Singleton.RaiseEnableHuntingActionSpaceEvent(true);
        }
        huntingDeck.Push(drawnCard);
        Vector3 localPosition = new Vector3(0, 0, (-1) * huntingDeck.Count * CardThickness);
        EventGenerator.Singleton.RaiseMoveComponentEvent(drawnCard.ComponentId, huntingDeckArea, localPosition, MoveStyle.LiftUp);
    }
}
