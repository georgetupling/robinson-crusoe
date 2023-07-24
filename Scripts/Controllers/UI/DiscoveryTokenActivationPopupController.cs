using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiscoveryTokenActivationPopupController : MonoBehaviour
{
    [SerializeField] private TMP_Text message;
    [SerializeField] private Image image;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private int sourceComponentId;
    private DiscoveryToken discoveryToken;

    private bool potIsBuilt;
    private bool waitingOnPotQuery;

    private int weaponLevel;
    private bool waitingOnWeaponQuery;

    // These fields are only for popups with target select (e.g. Vegetables)
    [SerializeField] private TMP_Dropdown playerSelect;
    private int selectedPlayerId;

    void Awake() {
        EventGenerator.Singleton.AddListenerToInitializeDiscoveryTokenActivationPopupEvent(OnInitialize);
        EventGenerator.Singleton.AddListenerToInventionIsBuiltEvent(OnInventionIsBuiltEvent);
        EventGenerator.Singleton.AddListenerToGetWeaponLevelEvent(OnGetWeaponLevelEvent);
    }

    void OnInitialize(int sourceComponentId, DiscoveryToken discoveryToken) {
        this.sourceComponentId = sourceComponentId;
        this.discoveryToken = discoveryToken;
        StartCoroutine(SetUpButtons());
        SetUpPlayerSelect();
        message.text = discoveryToken.activationMessage;
        image.sprite = discoveryToken.tokenSprite;
    }

    // Sets up the confirm and cancel buttons

    IEnumerator SetUpButtons() {
        // Disable the button if the weapon/ pot requirement isn't met
        if (discoveryToken.activationRequiresWeapon) {
            waitingOnWeaponQuery = true;
            EventGenerator.Singleton.RaiseGetWeaponLevelEvent();
            while (waitingOnWeaponQuery) {
                yield return null;
            }
            if (weaponLevel < 1) {
                confirmButton.interactable = false;
            }
        }
        if (discoveryToken.activationRequiresPot) {
            waitingOnPotQuery = true;
            EventGenerator.Singleton.RaiseInventionIsBuiltEvent(Invention.Pot);
            while (waitingOnPotQuery) {
                yield return null;
            }
            if (!potIsBuilt) {
                confirmButton.interactable = false;
            }
        }
        cancelButton.onClick.AddListener(ClosePopup);
        confirmButton.onClick.AddListener(ConfirmActivation);
        yield return new WaitForSeconds(0.1f);
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        SetPopupVisibility(true);
    }

    void ConfirmActivation() {
        EventGenerator.Singleton.RaiseActivateDiscoveryTokenEvent(sourceComponentId, selectedPlayerId);
        ClosePopup();
    }

    void ClosePopup() {
        EventGenerator.Singleton.RaiseEnableMainUIEvent();
        Destroy(gameObject);
    }

    // Sets up the player select options
    
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
        playerSelect.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(int value) {
        selectedPlayerId = value;
    }

    // Listens for responses to invention is built and weapon level queries

    void OnInventionIsBuiltEvent(string eventType, Invention invention, bool isBuilt) {
        if (eventType == InventionIsBuiltEvent.Response && waitingOnPotQuery && invention == Invention.Pot) {
            potIsBuilt = isBuilt;
            waitingOnPotQuery = false;
        }
    }

    void OnGetWeaponLevelEvent(string eventType, int weaponLevel) {
        if (eventType == GetWeaponLevelEvent.Response && waitingOnWeaponQuery) {
            this.weaponLevel = weaponLevel;
            waitingOnWeaponQuery = false;
        }
    }

    // Sets popup visibility

    void SetPopupVisibility(bool isVisible) {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = isVisible ? 1 : 0;
        canvasGroup.blocksRaycasts = isVisible;
        canvasGroup.interactable = isVisible;
    }
}
