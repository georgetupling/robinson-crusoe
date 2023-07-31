using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityPopupController : MonoBehaviour
{
    [SerializeField] TMP_Text instruction;
    [SerializeField] Image background;
    [SerializeField] Image abilityImage;
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button buttonPrefab;
    [SerializeField] Transform buttonArea;

    int playerId;
    Ability ability;

    int determination;
    bool waitingForDetermination;
    int food;
    bool waitingForFood;

    void Awake() {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        EventGenerator.Singleton.AddListenerToGetDeterminationResponseEvent(OnGetDeterminationResponseEvent);
        EventGenerator.Singleton.AddListenerToGetResourceEvent(OnGetResourceEvent);
        SetUpButtons();
    }

    void OnGetDeterminationResponseEvent(int playerId, int determination) {
        if (playerId != this.playerId || !waitingForDetermination) {
            return;
        }
        this.determination = determination;
        waitingForDetermination = false;
    }

    void OnGetResourceEvent(string eventType, int amount) {
        if (waitingForFood && eventType == GetResourceEvent.GetFoodResponse) {
            food = amount;
            waitingForFood = false;
        }
    }

    void SetUpButtons() {
        confirmButton.onClick.AddListener(() => {
            // Pressing confirm applies the ability effect and then closes the popup
            if (ability != null && ability.abilityEffect != null) {
                if (ability.abilityEffect.targetType == TargetType.Player) {
                    ability.abilityEffect.SetTarget(playerId);
                }
                ability.abilityEffect.ApplyEffect();
                EventGenerator.Singleton.RaiseLoseDeterminationEvent(playerId, ability.determinationCost);
                EventGenerator.Singleton.RaiseAbilityActivatedEvent(playerId, ability);
            }
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });

        cancelButton.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }

    public void Initialize(int playerId, Ability ability) {
        StartCoroutine(InitializationCoroutine(playerId, ability));
    }

    IEnumerator InitializationCoroutine(int playerId, Ability ability) {
        this.playerId = playerId;
        this.ability = ability;
        if (ability.abilitySprite != null) {
            abilityImage.sprite = ability.abilitySprite;
        }
        if (ability.isActivatedAbility) {
            if (ability.options.Count == 0) {
                // Code for activated abilities without options to pick from
                waitingForDetermination = true;
                EventGenerator.Singleton.RaiseGetDeterminationEvent(playerId);
                if (ability.foodCost > 0) {
                    waitingForFood = true;
                    EventGenerator.Singleton.RaiseGetFoodEvent();
                }
                while (waitingForDetermination || waitingForFood) {
                    yield return null;
                }
                if (determination < ability.determinationCost || food < ability.foodCost) {
                    confirmButton.interactable = false;
                }
            } else {
                // Code for activated abilities with options
                confirmButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                List<string> options = ability.options;
                RectTransform buttonTransform = buttonPrefab.GetComponent<RectTransform>();
                float buttonHeight = buttonTransform.rect.height;
                float gapBetweenButtons = 10f;
                // Resizes the popup based on how many options there are
                float width = background.rectTransform.sizeDelta.x;
                float height = background.rectTransform.sizeDelta.y + ((buttonHeight + gapBetweenButtons) * (options.Count - 2));
                background.rectTransform.sizeDelta = new Vector2(width, height);
                abilityImage.rectTransform.anchoredPosition = new Vector3(0, abilityImage.rectTransform.anchoredPosition.y + ((buttonHeight + gapBetweenButtons) * (options.Count - 2)), 0);
                // Checks determination
                waitingForDetermination = true;
                EventGenerator.Singleton.RaiseGetDeterminationEvent(playerId);
                if (ability.foodCost > 0) {
                    waitingForFood = true;
                    EventGenerator.Singleton.RaiseGetFoodEvent();
                }
                while (waitingForDetermination || waitingForFood) {
                    yield return null;
                }
                bool costsMet = true;
                if (determination < ability.determinationCost || food < ability.foodCost) {
                    costsMet = false;
                }
                // Spawns buttons, sets their text, and adds listeners
                for (int i = 0; i < options.Count; i++) {
                    Button newButton = Instantiate(buttonPrefab, buttonArea, false);
                    RectTransform newButtonRectTransform = newButton.GetComponent<RectTransform>(); 
                    newButtonRectTransform.localPosition = new Vector3(0, i * (buttonHeight + gapBetweenButtons), 0);
                    TMP_Text newButtonText = newButtonRectTransform.GetChild(0).GetComponent<TMP_Text>();
                    newButtonText.text = options[i];
                    // Shrinks the font size if the string is too long to fit
                    int characterThreshold = 15;
                    int smallerFontSize = 15;
                    if (options[i].Length > characterThreshold) {
                        newButtonText.fontSize = smallerFontSize;
                    }
                    int optionIndex = i;
                    newButton.onClick.AddListener(() => {
                        if (ability.options.Count > 0 && optionIndex == 0) {
                            // Index is the cancel button
                            EventGenerator.Singleton.RaiseEnableMainUIEvent();
                            Destroy(gameObject);
                            return;
                        }
                        // Sets the card effect to remember the option chosen
                        ability.abilityEffect.SetOptionChosen(optionIndex);
                        if (ability.abilityEffect.targetType == TargetType.Player) {
                            ability.abilityEffect.SetTarget(playerId);
                        }
                        ability.abilityEffect.ApplyEffect();
                        EventGenerator.Singleton.RaiseLoseDeterminationEvent(playerId, ability.determinationCost);
                        EventGenerator.Singleton.RaiseAbilityActivatedEvent(playerId, ability);
                        EventGenerator.Singleton.RaiseEnableMainUIEvent();
                        Destroy(gameObject);
                    });
                    if (!costsMet && i > 0) {
                        // Index 0 is the cancel button, so it is left interactable
                        newButton.interactable = false;
                    }
                }    
            }
        } else {
            // If the ability is not activated, changes the cancel button to "Okay" and hides the confirm button
            confirmButton.gameObject.SetActive(false);
            instruction.gameObject.SetActive(false);
            TMP_Text cancelButtonText = cancelButton.transform.GetChild(0).GetComponent<TMP_Text>();
            cancelButtonText.text = "Okay";
            // Resizes the background since there's now only one button
            float spaceBetweenButtons = 10f;
            float titleSpace = 40f;
            RectTransform backgroundRectTransform = background.GetComponent<RectTransform>();
            backgroundRectTransform.sizeDelta = new Vector2(backgroundRectTransform.rect.width, backgroundRectTransform.rect.height - confirmButton.GetComponent<RectTransform>().rect.height - spaceBetweenButtons - titleSpace);
            abilityImage.rectTransform.anchoredPosition = new Vector2(0f, 215f);
        }
        
        background.gameObject.SetActive(true);
    }
}
