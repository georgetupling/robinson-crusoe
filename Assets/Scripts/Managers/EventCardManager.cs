using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;

/*
    This class manages the event deck and also plays a role in handling the event phase.
    The flow of events that manage the event phase are as follows:
        - The EventCardManager recieves a PhaseStartEvent indicating the start of the event phase, which triggers it to draw
             a card and raise a CardDrawnEvent.
        - The drawn card's CardController recieves the CardDrawnEvent, performs the card reveal animation, and raises a
            CardRevealedEvent.
        - The PopupManager recieves the CardRevealedEvent, causing it to spawn a popup which presents the player with an 
            image of the card. When the player closes the popup, the popup's controller raises a CardPopupClosedEvent
            (and a DrawCardEvent if the card was an adventure or treasure card!).
        - When the card's controller recieves the CardPopupClosedEvent, it moves the card to the threat area and
            applies the appropriate event effects.
        - Similarly, EventCardControllers already in the threat area respond to the CardPopupClosedEvent by moving their
            cards to the left and applying failure effects where necessary.
*/

public class EventCardManager : MonoBehaviour
{
    private static EventCardManager singleton;

    private List<EventCard> eventCards = new List<EventCard>();
    private Stack<CardController> eventDeck = new Stack<CardController>();

    [SerializeField] private EventCardController eventCardPrefab;
    [SerializeField] private Transform eventDeckArea;
    [SerializeField] private Transform rightThreatArea;

    private const float CardThickness = 0.005f;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeEventCards();
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToDrawCardEvent(OnDrawCardEvent);
        EventGenerator.Singleton.AddListenerToShuffleIntoEventDeckEvent(OnShuffleIntoEventDeckEvent);
    }

    void Start()
    {
        SpawnEventDeck();
        SpawnStartingEventCard();
    }

    void InitializeEventCards()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.Combine("Data", "event-cards"));
        string jsonString = jsonTextAsset.text;
        List<string> jsonStrings = Json.ToList(jsonString);
        foreach (string str in jsonStrings)
        {
            EventCardUnprocessedData data = JsonUtility.FromJson<EventCardUnprocessedData>(str);
            EventCard eventCard = new EventCard(data);
            eventCards.Add(eventCard);
        }
    }

    void SpawnEventDeck()
    {
        int numberOfTurns = ScenarioManager.Singleton.Turns;
        int deckSize = numberOfTurns % 2 == 0 ? numberOfTurns : numberOfTurns + 1; // Rounds the deck size up to the nearest even number
        int bookCards = 0;
        int adventureCards = 0;
        int limit = deckSize / 2;
        List<EventCard> eventCardsCopy = new List<EventCard>(eventCards);
        while (eventDeck.Count < deckSize)
        {
            int index = UnityEngine.Random.Range(0, eventCardsCopy.Count);
            EventCard randCard = eventCardsCopy[index];
            eventCardsCopy.RemoveAt(index);
            if (randCard.cardSymbol == CardSymbol.Book && bookCards < limit)
            {
                SpawnEventCard(randCard);
                bookCards++;
            }
            else if (
                (randCard.cardSymbol == CardSymbol.BuildAdventure ||
                randCard.cardSymbol == CardSymbol.GatherAdventure ||
                randCard.cardSymbol == CardSymbol.ExploreAdventure) &&
                adventureCards < limit
            )
            {
                SpawnEventCard(randCard);
                adventureCards++;
            }
        }
    }

    void SpawnEventCard(EventCard eventCard)
    {
        EventCardController newCard = Instantiate(eventCardPrefab, eventDeckArea, false);
        newCard.transform.localPosition = new Vector3(0, 0, (-1) * eventDeck.Count * CardThickness);
        newCard.transform.localRotation = Quaternion.identity;
        newCard.InitializeCard(eventCard);
        eventDeck.Push(newCard);
    }

    void SpawnStartingEventCard()
    {
        EventCardController newCard = Instantiate(eventCardPrefab, rightThreatArea, false);
        newCard.transform.localPosition = Vector3.zero;
        newCard.transform.localEulerAngles = new Vector3(0, 180, 0);

        List<EventCard> startingEventCards = new List<EventCard>();
        foreach (EventCard eventCard in eventCards)
        {
            if (eventCard.isStartingEvent)
            {
                startingEventCards.Add(eventCard);
            }
        }
        int randomIndex = Random.Range(0, startingEventCards.Count);
        newCard.InitializeCard(startingEventCards[randomIndex]);
        EventGenerator.Singleton.RaiseEnableThreatActionAreaEvent(newCard.ComponentId, true);
        DeckShuffler.Singleton.ShuffleDeck(eventDeck, CardThickness);
    }

    void OnPhaseStartEvent(Phase phaseStarted)
    {
        if (phaseStarted == Phase.Event)
        {
            StartCoroutine(WaitBrieflyThenDrawCard());
        }
    }

    void OnDrawCardEvent(Deck deck)
    {
        if (deck == Deck.Event)
        {
            DrawCard();
        }
    }

    void DrawCard()
    {
        CardController drawnCard = eventDeck.Pop();
        EventGenerator.Singleton.RaiseCardDrawnEvent(Deck.Event, drawnCard.ComponentId);
    }

    void OnShuffleIntoEventDeckEvent(CardController cardController)
    {
        cardController.transform.SetParent(eventDeckArea, true);
        float duration = GameSettings.AnimationDuration;
        Vector3 topOfEventDeck = new Vector3(0, 0, (-1) * eventDeck.Count * CardThickness);
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        cardController.transform.DOLocalMove(topOfEventDeck, duration)
            .OnKill(() =>
            {
                eventDeck.Push(cardController);
                StartCoroutine(WaitBrieflyThenShuffle(cardController));
                EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
            });
    }

    IEnumerator WaitBrieflyThenShuffle(CardController cardController)
    {
        yield return new WaitForSeconds(0.25f);
        cardController.transform.eulerAngles = Vector3.zero;
        DeckShuffler.Singleton.ShuffleDeck(eventDeck, CardThickness);
    }

    IEnumerator WaitBrieflyThenDrawCard()
    {
        float waitTime = GameSettings.AnimationDuration;
        yield return new WaitForSeconds(waitTime);
        DrawCard();
    }
}
