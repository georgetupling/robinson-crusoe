using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCardController : CardController
{
    private EventCard data;

    private Transform leftThreatArea;
    private Transform rightThreatArea;

    [SerializeField] private ThreatActionSpaceController leftThreatActionSpace;
    [SerializeField] private ThreatActionSpaceController centreThreatActionSpace;
    [SerializeField] private ThreatActionSpaceController rightThreatActionSpace;
    
    protected override void Awake() {
        base.Awake();
        leftThreatArea = GameObject.Find("LeftThreatArea").transform;
        rightThreatArea = GameObject.Find("RightThreatArea").transform;
    }

    protected override void Start() {
        base.Start();
        EventGenerator.Singleton.AddListenerToCardDrawnEvent(OnCardDrawnEvent);
        EventGenerator.Singleton.AddListenerToCardPopupClosedEvent(OnCardPopupClosedEvent);
    }

    void OnCardDrawnEvent(Deck deckDrawnFrom, int componentIdOfDrawnCard) {
        if (deckDrawnFrom == Deck.Event && componentIdOfDrawnCard == this.ComponentId) {
            RevealCard(deckDrawnFrom, data);
        }
    }

    void OnCardPopupClosedEvent(int componentIdOfCardController) {
        if (componentIdOfCardController == this.ComponentId) {
            MoveToTransform(rightThreatArea, MoveStyle.Slow);
            ApplyCardSymbolEffect();
            ApplyEventEffects();
            EventGenerator.Singleton.RaiseEnableThreatActionAreaEvent(this.ComponentId, true);
            EventGenerator.Singleton.RaiseEndPhaseEvent(Phase.Event);
        } else if (transform.parent == rightThreatArea) {
            MoveToTransform(leftThreatArea);
        } else if (transform.parent == leftThreatArea) {
            ApplyFailureEffects();
            Destroy(gameObject);
            // TODO - Maybe add some failure animation later?
        }
    }

    void ApplyCardSymbolEffect() {
        switch (data.cardSymbol) {
            case CardSymbol.Book:
                // TODO - raise an event to tell the scenario manager to apply the book effect
                break;
            case CardSymbol.BuildAdventure:
                EventGenerator.Singleton.RaiseSpawnTokenOnDeckEvent(Deck.BuildAdventure, TokenType.BuildAdventure);
                break;
            case CardSymbol.GatherAdventure:
                EventGenerator.Singleton.RaiseSpawnTokenOnDeckEvent(Deck.GatherAdventure, TokenType.GatherAdventure);
                break;
            case CardSymbol.ExploreAdventure:
                EventGenerator.Singleton.RaiseSpawnTokenOnDeckEvent(Deck.ExploreAdventure, TokenType.ExploreAdventure);
                break;
        }
    }

    void ApplyEventEffects() {
        foreach(CardEffect cardEffect in data.eventEffects) {
            cardEffect.ApplyEffect();
        }
    }

    void ApplyFailureEffects() {
        foreach(CardEffect cardEffect in data.failureEffects) {
            cardEffect.ApplyEffect();
        }
    }

    public void InitializeCard(EventCard eventCard) {
        if (!isInitialized) {
            data = eventCard;
            isInitialized = true;
            meshRenderer.material = data.cardMaterial;
            leftThreatActionSpace.Initialize(this.ComponentId, data);
            centreThreatActionSpace.Initialize(this.ComponentId, data);
            rightThreatActionSpace.Initialize(this.ComponentId, data);
        } else {
            Debug.Log("EventCardController already initialized.");
        }
    }

    public EventCard GetEventCard() {
        return data;
    }
}
