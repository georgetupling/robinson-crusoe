using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatPopupController : MonoBehaviour
{
    [SerializeField] Image cardImage;
    [SerializeField] Button fightButton;
    [SerializeField] TMP_Text weaponLevelDisplay;
    [SerializeField] RectTransform background;
    [SerializeField] RectTransform toggleArea;
    [SerializeField] Toggle togglePrefab;

    int playerId;
    int componentId;
    BeastCard beastCard;
    int weaponLevel;
    int temporaryWeaponBonus;
    int determination;

    void Awake()
    {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        EventGenerator.Singleton.AddListenerToGetWeaponLevelEvent((string eventType, int weaponLevel) =>
        {
            if (eventType == GetWeaponLevelEvent.Response)
            {
                this.weaponLevel = weaponLevel;
            }
        });
        EventGenerator.Singleton.AddListenerToGetDeterminationResponseEvent((int playerId, int determination) =>
        {
            if (playerId != this.playerId)
            {
                return;
            }
            this.determination = determination;
        });
        SetUpFightButton();
    }
    void SetUpFightButton()
    {
        fightButton.onClick.AddListener(() =>
        {
            int totalWeaponLevel = weaponLevel + temporaryWeaponBonus;
            int arbitraryHighNumber = 100;
            int healthLost = Mathf.Clamp(beastCard.strength - totalWeaponLevel, 0, arbitraryHighNumber);
            EventGenerator.Singleton.RaiseLoseHealthEvent(playerId, healthLost);
            EventGenerator.Singleton.RaiseLoseWeaponEvent(beastCard.spearsLostAfterFight);
            EventGenerator.Singleton.RaiseGainFoodEvent(beastCard.foodGainedAfterFight);
            EventGenerator.Singleton.RaiseGainHideEvent(beastCard.hidesGainedAfterFight);
            foreach (CardEffect effect in beastCard.effectsAfterFight)
            {
                if (effect.targetType == TargetType.Player)
                {
                    effect.SetTarget(playerId);
                }
                effect.ApplyEffect();
            }
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            EventGenerator.Singleton.RaiseDestroyComponentEvent(componentId);
            Destroy(gameObject);
        });
    }
    public void Initialize(int playerId, int componentId, BeastCard beastCard)
    {
        this.playerId = playerId;
        this.componentId = componentId;
        this.beastCard = beastCard;
        cardImage.sprite = beastCard.cardSprite;
        EventGenerator.Singleton.RaiseGetWeaponLevelEvent();
        weaponLevelDisplay.text = "Your weapon level is " + weaponLevel + ".";

        float distanceBetweenToggles = 15f;
        float toggleHeight = togglePrefab.GetComponent<RectTransform>().rect.height;

        if (GameSettings.PlayerCharacters[playerId] == CharacterType.Soldier)
        {
            EventGenerator.Singleton.RaiseGetDeterminationEvent(playerId);
            int frenzyDeterminationCost = 3;
            if (determination >= frenzyDeterminationCost)
            {
                Toggle frenzyToggle = Instantiate(togglePrefab, toggleArea, false);
                TMP_Text toggleLabel = frenzyToggle.GetComponentInChildren<TMP_Text>();
                toggleLabel.text = "Frenzy (+3 weapon, -2 determination)";

                // Resizes the background and positions the toggle area
                background.sizeDelta = new Vector2(background.rect.width, background.rect.height + (distanceBetweenToggles + toggleHeight));
                toggleArea.localPosition = new Vector3(toggleArea.localPosition.x, toggleArea.localPosition.y + (distanceBetweenToggles + toggleHeight), 0f);
                frenzyToggle.transform.localPosition = Vector3.zero;

                // Adds a listener to the frenzy toggle
                frenzyToggle.onValueChanged.AddListener((bool toggledOn) =>
                {
                    int frenzyWeaponBonus = 3;
                    temporaryWeaponBonus += toggledOn ? frenzyWeaponBonus : -frenzyWeaponBonus;
                    weaponLevelDisplay.text = "Your weapon level is " + (weaponLevel + temporaryWeaponBonus) + ".";
                });
            }
        }

        // TODO: spawn toggles for items granting temporary weapon bonus
    }
}
