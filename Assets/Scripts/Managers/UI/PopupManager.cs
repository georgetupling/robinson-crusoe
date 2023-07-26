using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    private static PopupManager singleton;

    [SerializeField] private SpritePopupController islandTilePopupPrefab;
    [SerializeField] private SpritePopupController discoveryTokenPopupPrefab;
    [SerializeField] private CardPopupController cardPopupPrefab;
    [SerializeField] private MoraleChoicePopupController moraleChoicePopupPrefab;
    [SerializeField] private DiscoveryTokenActivationPopupController discoveryTokenActivationPopupPrefab;
    [SerializeField] private DiscoveryTokenActivationPopupController discoveryTokenActivationPopupWithTargetSelectPrefab;
    [SerializeField] private VariableCostPopupController variableCostPopupPrefab;
    [SerializeField] private MakeCampChoicePopupController makeCampChoicePopupPrefab;
    [SerializeField] private NightPhasePopupController nightPhasePopupPrefab;
    [SerializeField] private DicePopupController dicePopupPrefab;

    [SerializeField] private Transform parentTransform;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            return;
        }
        EventGenerator.Singleton.AddListenerToCardRevealedEvent(OnCardRevealedEvent);
        EventGenerator.Singleton.AddListenerToIslandTileRevealedEvent(OnIslandTileRevealedEvent);
        EventGenerator.Singleton.AddListenerToDiscoveryTokenRevealedEvent(OnDiscoveryTokenRevealedEvent);
        EventGenerator.Singleton.AddListenerToSpawnMoraleChoicePopupEvent(OnSpawnMoraleChoicePopupEvent);
        EventGenerator.Singleton.AddListenerToSpawnDiscoveryTokenActivationPopupEvent(OnSpawnDiscoveryTokenActivationPopupEvent);
        EventGenerator.Singleton.AddListenerToSpawnVariableCostPopupEvent(OnSpawnVariableCostPopupEvent);
        EventGenerator.Singleton.AddListenerToSpawnMakeCampChoicePopupEvent(OnSpawnMakeCampChoicePopupEvent);
        EventGenerator.Singleton.AddListenerToSpawnNightPhasePopupEvent(OnSpawnNightPhasePopupEvent);
        EventGenerator.Singleton.AddListenerToSpawnDicePopupEvent(OnSpawnDicePopupEvent);
    }

    void OnCardRevealedEvent(Deck deckDrawnFrom, Card revealedCard, int componentIdOfRevealedCard) {
        CardPopupController newPopup = Instantiate(cardPopupPrefab, parentTransform, false);
        EventGenerator.Singleton.RaiseInitializeCardPopupEvent(newPopup.ComponentId, componentIdOfRevealedCard, deckDrawnFrom, revealedCard);
    }

    void OnIslandTileRevealedEvent(int sourceComponentId, Sprite sprite) {
        SpritePopupController newPopup = Instantiate(islandTilePopupPrefab, parentTransform, false);
        EventGenerator.Singleton.RaiseInitializeSpritePopupEvent(newPopup.ComponentId, sourceComponentId, sprite);
    }

    void OnDiscoveryTokenRevealedEvent(int sourceComponentId, Sprite sprite) {
        SpritePopupController newPopup = Instantiate(discoveryTokenPopupPrefab, parentTransform, false);
        EventGenerator.Singleton.RaiseInitializeSpritePopupEvent(newPopup.ComponentId, sourceComponentId, sprite);
    }

    void OnSpawnMoraleChoicePopupEvent() {
        MoraleChoicePopupController newPopup = Instantiate(moraleChoicePopupPrefab, parentTransform, false);
    }

    void OnSpawnDiscoveryTokenActivationPopupEvent(int sourceComponentId, DiscoveryToken discoveryToken) {
        if (ActivationRequiresTarget(discoveryToken)) {
            DiscoveryTokenActivationPopupController newPopup = Instantiate(discoveryTokenActivationPopupWithTargetSelectPrefab, parentTransform, false);
        } else {
            DiscoveryTokenActivationPopupController newPopup = Instantiate(discoveryTokenActivationPopupPrefab, parentTransform, false);
        }
        EventGenerator.Singleton.RaiseInitializeDiscoveryTokenActivationPopupEvent(sourceComponentId, discoveryToken);
    }

    void OnSpawnVariableCostPopupEvent(ResourceCost resourceCost, ActionAssignment actionAssignment) {
        VariableCostPopupController newPopup = Instantiate(variableCostPopupPrefab, parentTransform, false);
        newPopup.Initialize(resourceCost, actionAssignment);
    }

    void OnSpawnMakeCampChoicePopupEvent(int playerId) {
        MakeCampChoicePopupController newPopup = Instantiate(makeCampChoicePopupPrefab, parentTransform, false);
        newPopup.Initialize(playerId);
    }

    void OnSpawnNightPhasePopupEvent(int totalFoodAvailable) {
        NightPhasePopupController newPopup = Instantiate(nightPhasePopupPrefab, parentTransform, false);
        newPopup.Initialize(totalFoodAvailable);
    }

    void OnSpawnDicePopupEvent(List<DieType> dieTypes) {
        DicePopupController newPopup = Instantiate(dicePopupPrefab, parentTransform, false);
        newPopup.Initialize(dieTypes);
    }

    // Helper methods

    bool ActivationRequiresTarget(DiscoveryToken discoveryToken) {
        foreach (CardEffect activationEffect in discoveryToken.effectsOnActivation) {
            if (activationEffect.targetType == TargetType.Player) {
                return true;
            }
        }
        return false;
    }
}
