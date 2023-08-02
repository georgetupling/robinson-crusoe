using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    Right now all activated items target a player.
    If later you need this popup for item activations without the dropdown, you'll need to add some code for hiding it.
*/

public class ItemActivationPopupController : MonoBehaviour
{
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Image cardImage;

    [SerializeField] private TMP_Dropdown playerSelect;
    private int selectedPlayerId;

    bool waitingOnInventionCard;
    Invention invention;
    InventionCard inventionCard;

    void Awake() {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        EventGenerator.Singleton.AddListenerToGetInventionCardEvent(OnGetInventionCardEvent);
        SetUpButtons();
        SetUpPlayerSelect();
    }

    void OnGetInventionCardEvent(string eventType, Invention invention, InventionCard inventionCard) {
        if (eventType == GetInventionCardEvent.Response && waitingOnInventionCard && invention == this.invention) {
            this.inventionCard = inventionCard;
            waitingOnInventionCard = false;
        }
    }

    void SetUpButtons() {
        confirmButton.onClick.AddListener(() => {
            if (inventionCard != null && inventionCard.effectsOnActivation != null) {
                foreach (CardEffect cardEffect in inventionCard.effectsOnActivation) {
                    if (cardEffect.targetType == TargetType.Player) {
                        cardEffect.SetTarget(selectedPlayerId);
                    }
                    cardEffect.ApplyEffect();
                }
            }
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });

        cancelButton.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }

    void SetUpPlayerSelect() {
        if (playerSelect == null) {
            return;
        }
        List<string> options = new List<string>();
        for (int i = 0; i < GameSettings.PlayerCount; i++) {
            options.Add(GameSettings.PlayerNames[i]);
        }
        playerSelect.ClearOptions();
        playerSelect.AddOptions(options);
        selectedPlayerId = 0;
        playerSelect.onValueChanged.AddListener(HandleSelectionChange);
    }

    void HandleSelectionChange(int value) {
        selectedPlayerId = value;
    }

    public void SetInvention(Invention invention) {
        this.invention = invention;
        waitingOnInventionCard = true;
        EventGenerator.Singleton.RaiseGetInventionCardEvent(invention);
        while (waitingOnInventionCard) {
            // Do nothing
        }
        if (inventionCard != null && inventionCard.cardSprite != null) {
            cardImage.sprite = inventionCard.cardSprite;
        }
    }
}
