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
        if (card is EventCard eventCard) {
            if (!eventCard.eventHasDecision) {
                okayButton.gameObject.SetActive(true);
                okayButton.onClick.AddListener(() => {
                    EventGenerator.Singleton.RaiseEventCardPopupClosedEvent(componentIdOfCardController);
                    EventGenerator.Singleton.RaiseEnableMainUIEvent();
                    Destroy(gameObject);
                });
            } else if (eventCard.eventHasDecision) {
                List<string> eventOptions = eventCard.eventOptions;
                RectTransform buttonTransform = buttonPrefab.GetComponent<RectTransform>();
                float buttonHeight = buttonTransform.rect.height;
                float gapBetweenButtons = 10f;
                // Resizes the popup based on how many options there are
                float width = background.rectTransform.sizeDelta.x;
                float height = background.rectTransform.sizeDelta.y + ((buttonHeight + gapBetweenButtons) * (eventOptions.Count - 1));
                background.rectTransform.sizeDelta = new Vector2(width, height);
                buttonArea.localPosition = new Vector3(0, buttonArea.localPosition.y - ((buttonHeight * 0.5f) * (eventOptions.Count - 1)), 0);
                // Spawns buttons, sets their text, and adds listeners
                for (int i = 0; i < eventOptions.Count; i++) {
                    Button newButton = Instantiate(buttonPrefab, buttonArea, false);
                    RectTransform newButtonRectTransform = newButton.GetComponent<RectTransform>(); 
                    newButtonRectTransform.localPosition = new Vector3(0, i * (buttonHeight + gapBetweenButtons), 0);
                    TMP_Text newButtonText = newButtonRectTransform.GetChild(0).GetComponent<TMP_Text>();
                    newButtonText.text = eventOptions[i];
                    // Shrinks the font size if the string is too long to fit
                    int characterThreshold = 15;
                    int smallerFontSize = 15;
                    if (eventOptions[i].Length > characterThreshold) {
                        newButtonText.fontSize = smallerFontSize;
                    }
                    int optionIndex = i;
                    newButton.onClick.AddListener(() => {
                        // Sets the card effects to remember the option chosen
                        foreach (CardEffect cardEffect in eventCard.eventEffects) {
                            cardEffect.SetOptionChosen(optionIndex);
                        }
                        EventGenerator.Singleton.RaiseEventCardPopupClosedEvent(componentIdOfCardController);
                        EventGenerator.Singleton.RaiseEnableMainUIEvent();
                        Destroy(gameObject);
                    });
                }
            }
            
        } else if (card is AdventureCard adventureCard) {
            if (!adventureCard.eventHasDecision) {
            okayButton.gameObject.SetActive(true);
            okayButton.onClick.AddListener(() => {
                    // Adventure cards apply their event effects and are then destroyed
                    foreach (CardEffect cardEffect in adventureCard.eventEffects) {
                        cardEffect.ApplyEffect();
                    }
                    EventGenerator.Singleton.RaiseDestroyComponentEvent(componentIdOfCardController);
                    EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Event);
                    EventGenerator.Singleton.RaiseEnableMainUIEvent();
                    Destroy(gameObject);
            });
            } else if (adventureCard.eventHasDecision) {
                List<string> eventOptions = adventureCard.eventOptions;
                RectTransform buttonTransform = buttonPrefab.GetComponent<RectTransform>();
                float buttonHeight = buttonTransform.rect.height;
                float gapBetweenButtons = 10f;
                // Resizes the popup based on how many options there are
                float width = background.rectTransform.sizeDelta.x;
                float height = background.rectTransform.sizeDelta.y + ((buttonHeight + gapBetweenButtons) * (eventOptions.Count - 1));
                background.rectTransform.sizeDelta = new Vector2(width, height);
                buttonArea.localPosition = new Vector3(0, buttonArea.localPosition.y - ((buttonHeight * 0.5f) * (eventOptions.Count - 1)), 0);
                // Spawns buttons, sets their text, and adds listeners
                for (int i = 0; i < eventOptions.Count; i++) {
                    Button newButton = Instantiate(buttonPrefab, buttonArea, false);
                    RectTransform newButtonRectTransform = newButton.GetComponent<RectTransform>(); 
                    newButtonRectTransform.localPosition = new Vector3(0, i * (buttonHeight + gapBetweenButtons), 0);
                    TMP_Text newButtonText = newButtonRectTransform.GetChild(0).GetComponent<TMP_Text>();
                    newButtonText.text = eventOptions[i];
                    // Shrinks the font size if the string is too long to fit
                    int characterThreshold = 15;
                    int smallerFontSize = 15;
                    if (eventOptions[i].Length > characterThreshold) {
                        newButtonText.fontSize = smallerFontSize;
                    }
                    int optionIndex = i;
                    newButton.onClick.AddListener(() => {
                        // Immediately sets the chosen option and applies the effects
                        foreach (CardEffect cardEffect in adventureCard.eventEffects) {
                            cardEffect.SetOptionChosen(optionIndex);
                            cardEffect.ApplyEffect();
                        }
                        EventGenerator.Singleton.RaiseDestroyComponentEvent(componentIdOfCardController);
                        EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Event);
                        EventGenerator.Singleton.RaiseEnableMainUIEvent();
                        Destroy(gameObject);
                    });
                }
            }
        }
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
            buttonArea.localPosition = new Vector3(0, buttonArea.localPosition.y - ((buttonHeight + gapBetweenButtons) * 0.5f * (adventureOptions.Count - 1)), 0);
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
                int optionIndex = i;
                newButton.onClick.AddListener(() => {
                    EventGenerator.Singleton.RaiseAdventureCardPopupClosedEvent(componentIdOfCardController, card as AdventureCard, optionIndex);
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
