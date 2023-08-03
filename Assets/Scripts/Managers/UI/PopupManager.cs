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
    [SerializeField] private AbilityPopupController abilityPopupPrefab;
    [SerializeField] private ChooseInventionCardPopupController chooseInventionCardPopupPrefab;
    [SerializeField] private ReconnaissancePopupController reconnaissancePopupPrefab;
    [SerializeField] private ScoutingPopupController scoutingPopPrefab;
    [SerializeField] private TrackingPopupController trackingPopupPrefab;
    [SerializeField] private ItemActivationPopupController itemActivationPopupPrefab;
    [SerializeField] private ShortcutPopupController shortcutPopupPrefab;

    [SerializeField] private Transform parentTransform;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
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
        EventGenerator.Singleton.AddListenerToSpawnAbilityPopupEvent(OnSpawnAbilityPopupEvent);
        EventGenerator.Singleton.AddListenerToSpawnChooseInventionCardPopupEvent(OnSpawnChooseInventionCardPopupEvent);
        EventGenerator.Singleton.AddListenerToSpawnReconnaissancePopupEvent(OnSpawnReconnaissancePopupEvent);
        EventGenerator.Singleton.AddListenerToSpawnScoutingPopupEvent(OnSpawnScoutingPopupEvent);
        EventGenerator.Singleton.AddListenerToSpawnTrackingPopupEvent(OnSpawnTrackingPopupEvent);
        EventGenerator.Singleton.AddListenerToSpawnItemActivationPopupEvent(OnSpawnItemActivationEvent);
        EventGenerator.Singleton.AddListenerToSpawnShortcutPopupEvent(OnSpawnShortcutPopupEvent);
    }

    void OnCardRevealedEvent(Deck deckDrawnFrom, Card revealedCard, int componentIdOfRevealedCard)
    {
        CardPopupController newPopup = Instantiate(cardPopupPrefab, parentTransform, false);
        EventGenerator.Singleton.RaiseInitializeCardPopupEvent(newPopup.ComponentId, componentIdOfRevealedCard, deckDrawnFrom, revealedCard);
    }

    void OnIslandTileRevealedEvent(int sourceComponentId, Sprite sprite)
    {
        SpritePopupController newPopup = Instantiate(islandTilePopupPrefab, parentTransform, false);
        EventGenerator.Singleton.RaiseInitializeSpritePopupEvent(newPopup.ComponentId, sourceComponentId, sprite);
    }

    void OnDiscoveryTokenRevealedEvent(int sourceComponentId, Sprite sprite)
    {
        SpritePopupController newPopup = Instantiate(discoveryTokenPopupPrefab, parentTransform, false);
        EventGenerator.Singleton.RaiseInitializeSpritePopupEvent(newPopup.ComponentId, sourceComponentId, sprite);
    }

    void OnSpawnMoraleChoicePopupEvent()
    {
        MoraleChoicePopupController newPopup = Instantiate(moraleChoicePopupPrefab, parentTransform, false);
    }

    void OnSpawnDiscoveryTokenActivationPopupEvent(int sourceComponentId, DiscoveryToken discoveryToken)
    {
        if (ActivationRequiresTarget(discoveryToken))
        {
            DiscoveryTokenActivationPopupController newPopup = Instantiate(discoveryTokenActivationPopupWithTargetSelectPrefab, parentTransform, false);
        }
        else
        {
            DiscoveryTokenActivationPopupController newPopup = Instantiate(discoveryTokenActivationPopupPrefab, parentTransform, false);
        }
        EventGenerator.Singleton.RaiseInitializeDiscoveryTokenActivationPopupEvent(sourceComponentId, discoveryToken);
    }

    void OnSpawnVariableCostPopupEvent(ResourceCost resourceCost, ActionAssignment actionAssignment)
    {
        VariableCostPopupController newPopup = Instantiate(variableCostPopupPrefab, parentTransform, false);
        newPopup.Initialize(resourceCost, actionAssignment);
    }

    void OnSpawnMakeCampChoicePopupEvent(int playerId)
    {
        MakeCampChoicePopupController newPopup = Instantiate(makeCampChoicePopupPrefab, parentTransform, false);
        newPopup.Initialize(playerId);
    }

    void OnSpawnNightPhasePopupEvent(int totalFoodAvailable)
    {
        NightPhasePopupController newPopup = Instantiate(nightPhasePopupPrefab, parentTransform, false);
        newPopup.Initialize(totalFoodAvailable);
    }

    void OnSpawnDicePopupEvent(List<DieType> dieTypes, int playerId, bool hasRerollAvailable)
    {
        DicePopupController newPopup = Instantiate(dicePopupPrefab, parentTransform, false);
        newPopup.Initialize(dieTypes, playerId, hasRerollAvailable);
    }

    void OnSpawnAbilityPopupEvent(int playerId, Ability ability)
    {
        AbilityPopupController newPopup = Instantiate(abilityPopupPrefab, parentTransform, false);
        newPopup.Initialize(playerId, ability);
    }

    void OnSpawnChooseInventionCardPopupEvent(List<InventionCard> inventionCards)
    {
        ChooseInventionCardPopupController newPopup = Instantiate(chooseInventionCardPopupPrefab, parentTransform, false);
        newPopup.Initialize(inventionCards);
    }

    void OnSpawnReconnaissancePopupEvent(List<IslandTileController> topTiles, Stack<IslandTileController> deck)
    {
        ReconnaissancePopupController newPopup = Instantiate(reconnaissancePopupPrefab, parentTransform, false);
        newPopup.Initialize(topTiles, deck);
    }

    void OnSpawnScoutingPopupEvent(DiscoveryTokenController token1, DiscoveryTokenController token2)
    {
        ScoutingPopupController newPopup = Instantiate(scoutingPopPrefab, parentTransform, false);
        newPopup.Initialize(token1, token2);
    }

    void OnSpawnTrackingPopupEvent(BeastCardController topCard, Stack<BeastCardController> huntingDeck)
    {
        TrackingPopupController newPopup = Instantiate(trackingPopupPrefab, parentTransform, false);
        newPopup.Initialize(topCard, huntingDeck);
    }

    void OnSpawnItemActivationEvent(Invention invention)
    {
        ItemActivationPopupController newPopup = Instantiate(itemActivationPopupPrefab, parentTransform, false);
        newPopup.SetInvention(invention);
    }

    void OnSpawnShortcutPopupEvent()
    {
        ShortcutPopupController newPopup = Instantiate(shortcutPopupPrefab, parentTransform, false);
    }

    // Helper methods

    bool ActivationRequiresTarget(DiscoveryToken discoveryToken)
    {
        foreach (CardEffect activationEffect in discoveryToken.effectsOnActivation)
        {
            if (activationEffect.targetType == TargetType.Player)
            {
                return true;
            }
        }
        return false;
    }
}
