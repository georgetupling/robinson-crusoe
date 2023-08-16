using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemActivationPopupController : MonoBehaviour
{
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Image cardImage;
    [SerializeField] RectTransform background;

    [SerializeField] private TMP_Dropdown playerSelect;
    private int selectedPlayerId;

    // For invention activations
    bool waitingOnInventionCard;
    Invention invention;
    InventionCard inventionCard;

    // For equipment activations
    EquipmentCard equipmentCard;

    void Awake()
    {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        EventGenerator.Singleton.AddListenerToGetInventionCardEvent(OnGetInventionCardEvent);
        SetUpPlayerSelect();
    }

    void OnGetInventionCardEvent(string eventType, Invention invention, InventionCard inventionCard)
    {
        if (eventType == GetInventionCardEvent.Response && waitingOnInventionCard && invention == this.invention)
        {
            this.inventionCard = inventionCard;
            waitingOnInventionCard = false;
        }
    }

    void SetUpButtonsForInvention()
    {
        confirmButton.onClick.AddListener(() =>
        {
            if (inventionCard != null && inventionCard.effectsOnActivation != null)
            {
                foreach (CardEffect cardEffect in inventionCard.effectsOnActivation)
                {
                    if (cardEffect.targetType == TargetType.Player)
                    {
                        cardEffect.SetTarget(selectedPlayerId);
                    }
                    cardEffect.ApplyEffect();
                }
            }
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });

        cancelButton.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }

    void SetUpButtonsForEquipment()
    {
        confirmButton.onClick.AddListener(() =>
        {
            if (equipmentCard != null && equipmentCard.effectsOnActivation != null)
            {
                foreach (CardEffect cardEffect in equipmentCard.effectsOnActivation)
                {
                    if (cardEffect.targetType == TargetType.Player)
                    {
                        cardEffect.SetTarget(selectedPlayerId);
                    }
                    cardEffect.ApplyEffect();
                }
            }
            EventGenerator.Singleton.RaiseEquipmentActivatedEvent(equipmentCard.equipment);
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });

        cancelButton.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }

    void SetUpPlayerSelect()
    {
        if (playerSelect == null)
        {
            return;
        }
        List<string> options = new List<string>();
        for (int i = 0; i < GameSettings.PlayerCount; i++)
        {
            options.Add(GameSettings.PlayerNames[i]);
        }
        playerSelect.ClearOptions();
        playerSelect.AddOptions(options);
        selectedPlayerId = 0;
        playerSelect.onValueChanged.AddListener(HandleSelectionChange);
    }

    void HandleSelectionChange(int value)
    {
        selectedPlayerId = value;
    }

    public void SetInvention(Invention invention)
    {
        this.invention = invention;
        waitingOnInventionCard = true;
        EventGenerator.Singleton.RaiseGetInventionCardEvent(invention);
        if (inventionCard != null && inventionCard.cardSprite != null)
        {
            cardImage.sprite = inventionCard.cardSprite;
        }
        // Hides the player select and shrinks the background if the invention doesn't target a player
        if (inventionCard != null && inventionCard.effectsOnActivation != null)
        {
            bool targetsPlayer = false;
            foreach (CardEffect activationEffect in inventionCard.effectsOnActivation)
            {
                if (activationEffect.targetType == TargetType.Player)
                {
                    targetsPlayer = true;
                    break;
                }
            }
            if (!targetsPlayer)
            {
                float buttonHeightPlusGapBetweenButtons = 50f;
                background.sizeDelta = new Vector2(background.sizeDelta.x, background.sizeDelta.y - buttonHeightPlusGapBetweenButtons);
                playerSelect.gameObject.SetActive(false);
            }
        }
        SetUpButtonsForInvention();
    }

    public void SetEquipmentCard(EquipmentCard equipmentCard)
    {
        this.equipmentCard = equipmentCard;
        if (equipmentCard != null && equipmentCard.cardSprite != null)
        {
            cardImage.sprite = equipmentCard.cardSprite;
        }

        // Hides the player select and shrinks the background if the equipment doesn't target a player
        if (equipmentCard != null && equipmentCard.effectsOnActivation != null)
        {
            bool targetsPlayer = false;
            foreach (CardEffect activationEffect in equipmentCard.effectsOnActivation)
            {
                if (activationEffect.targetType == TargetType.Player)
                {
                    targetsPlayer = true;
                    break;
                }
            }
            if (!targetsPlayer)
            {
                float buttonHeightPlusGapBetweenButtons = 50f;
                background.sizeDelta = new Vector2(background.sizeDelta.x, background.sizeDelta.y - buttonHeightPlusGapBetweenButtons);
                playerSelect.gameObject.SetActive(false);
            }
        }
        SetUpButtonsForEquipment();
    }
}
