using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AdventureCardController : CardController
{
    private AdventureCard data;
    private Transform eventDeckArea;

    protected override void Awake() {
        base.Awake();
        EventGenerator.Singleton.AddListenerToCardDrawnEvent(OnCardDrawnEvent);
        EventGenerator.Singleton.AddListenerToAdventureCardPopupClosedEvent(OnAdventureCardPopupClosedEvent);
        eventDeckArea = GameObject.Find("EventDeckArea").transform;
    }
    
    protected override void Start() {
        base.Start();
    }

    void OnCardDrawnEvent(Deck deckType, int componentId) {
        if (componentId != this.ComponentId) {
            return;
        }
        RevealCard(deckType, data);
    }

    void OnAdventureCardPopupClosedEvent(int componentId, AdventureCard adventureCard, int optionChosen) {
        if (componentId != this.ComponentId) {
            return;
        }
        if (data.hasEvent) {
            if (data.adventureHasDecision && data.adventureOptions[optionChosen] == "Discard") {
                Destroy(gameObject);
                return;
            }
            EventGenerator.Singleton.RaiseShuffleIntoEventDeckEvent(this as CardController); 
        } else {
            Destroy(gameObject);
        }
    }

    public void InitializeCard(AdventureCard adventureCard) {
        if (!isInitialized) {
            data = adventureCard;
            isInitialized = true;
            meshRenderer.material = data.cardMaterial;
        } else {
            Debug.Log("AdventureCardController already initialized.");
        }
    }
}
