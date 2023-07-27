using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPopupController : ComponentController
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Button okayButton;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private Image background;
    [SerializeField] private Transform buttonArea;

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
        okayButton.gameObject.SetActive(true);
        okayButton.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            if (card is EventCard eventCard) {
                EventGenerator.Singleton.RaiseEventCardPopupClosedEvent(componentIdOfCardController);
            } else if (card is AdventureCard adventureCard) {
                // TODO
                EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Event);
            }
            Destroy(gameObject);
        });
    }

    void SetUpButtonsForDrawingFromAdventureDeck() {
        AdventureCard adventureCard = card as AdventureCard;
        if (adventureCard.adventureHasDecision) {
            List<string> adventureOptions = adventureCard.adventureOptions;
            RectTransform buttonTransform = buttonPrefab.GetComponent<RectTransform>();
            float buttonHeight = buttonTransform.rect.height;
            float gapBetweenButtons = 10f;
            // Resizes the popup based on how many options there are
            float width = background.rectTransform.sizeDelta.x;
            float height = background.rectTransform.sizeDelta.y + ((buttonHeight + gapBetweenButtons) * (adventureOptions.Count - 1));
            background.rectTransform.sizeDelta = new Vector2(width, height);
            buttonArea.localPosition = new Vector3(0, buttonArea.localPosition.y - ((gapBetweenButtons) * (adventureOptions.Count - 1)), 0);
            // Spawns buttons, sets their text, and adds listeners
            for (int i = 0; i < adventureOptions.Count; i++) {
                Button newButton = Instantiate(buttonPrefab, buttonArea, false);
                RectTransform newButtonRectTransform = newButton.GetComponent<RectTransform>(); 
                newButtonRectTransform.localPosition = new Vector3(0, i * (buttonHeight + gapBetweenButtons), 0);
                TMP_Text newButtonText = newButtonRectTransform.GetChild(0).GetComponent<TMP_Text>();
                newButtonText.text = adventureOptions[i];
                // Shrinks the font size if the string is too long to fit
                int characterThreshold = 15;
                int smallerFontSize = 15;
                if (adventureOptions[i].Length > characterThreshold) {
                    newButtonText.fontSize = smallerFontSize;
                }
                newButton.onClick.AddListener(() => {
                    EventGenerator.Singleton.RaiseAdventureCardPopupClosedEvent(componentIdOfCardController, adventureCard);
                    EventGenerator.Singleton.RaiseEnableMainUIEvent();
                    Destroy(gameObject);
                });
            }
        } else {
            // Sets up the Okay button if there isn't a decision to make
            okayButton.gameObject.SetActive(true);
            okayButton.onClick.AddListener(() => {
                EventGenerator.Singleton.RaiseAdventureCardPopupClosedEvent(componentIdOfCardController, adventureCard);
                EventGenerator.Singleton.RaiseEnableMainUIEvent();
                Destroy(gameObject);
            });
        }
        
        
    }

}
