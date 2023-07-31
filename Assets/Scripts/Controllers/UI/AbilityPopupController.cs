using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPopupController : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Image abilityImage;
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;

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
        waitingForDetermination = true;
        EventGenerator.Singleton.RaiseGetDeterminationEvent(playerId);
        if (ability.foodCost > 0) {
            waitingForFood = true;
            EventGenerator.Singleton.RaiseGetFoodEvent();
        }
        while (waitingForDetermination || waitingForFood) {
            yield return null;
        }
        if (determination < ability.determinationCost) {
            confirmButton.interactable = false;
        }
        background.gameObject.SetActive(true);
    }
}
