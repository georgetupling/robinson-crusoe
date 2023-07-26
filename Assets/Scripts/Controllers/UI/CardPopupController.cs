using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPopupController : ComponentController
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Button okayButton;

    private int componentIdOfCardController;
    private Card card;
    private Deck deckDrawnFrom;

    protected override void Awake() {
        base.Awake();
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        EventGenerator.Singleton.AddListenerToInitializeCardPopupEvent(OnInitializeCardPopupEvent);
    }

    void OnInitializeCardPopupEvent(int componentIdOfPopup, int componentIdOfCardController, Deck deckDrawnFrom, Card card) {
        if (this.ComponentId == componentIdOfPopup) {
            this.componentIdOfCardController = componentIdOfCardController;
            this.card = card;
            this.deckDrawnFrom = deckDrawnFrom;
            SetCardImage();
            SetUpButtons();
        }
    }

    void SetCardImage() {
        if (card == null) {
            Debug.LogError($"card is null. Failed to initialize card popup.");
            return;
        }
        if (card.cardSprite == null) {
            Debug.LogError($"card.cardSprite is null. Failed to initialize card popup.");
            return;
        }
        cardImage.sprite = card.cardSprite;
    }
    
    void SetUpButtons() {
        if (deckDrawnFrom == Deck.Event) {
            SetUpButtonsForDrawingFromEventDeck();
        } else if (deckDrawnFrom == Deck.BuildAdventure || deckDrawnFrom == Deck.GatherAdventure || deckDrawnFrom == Deck.ExploreAdventure) {
            SetUpButtonsForDrawingFromAdventureDeck();
        }
    }

    void SetUpButtonsForDrawingFromEventDeck() {
        okayButton.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            if (card is EventCard eventCard) {
                EventGenerator.Singleton.RaiseEventCardPopupClosedEvent(componentIdOfCardController);
            } else if (card is AdventureCard adventureCard) {
                EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Event);
            }
            Destroy(gameObject);
        });
    }

    void SetUpButtonsForDrawingFromAdventureDeck() {
        okayButton.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            // TODO - raise event telling the DiceRoller to action the action card (n.b. the Diceroller stores the target info)
            EventGenerator.Singleton.RaiseDestroyComponentEvent(componentIdOfCardController);
            Destroy(gameObject);
        });
    }

}
