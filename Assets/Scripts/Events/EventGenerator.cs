using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static System.Type;

[DefaultExecutionOrder(-1)]
public class EventGenerator : MonoBehaviour
{
    public static EventGenerator Singleton { get; private set; }

    // Player-related
    private MoraleEvent moraleEvent = new MoraleEvent();
    private HealthEvent healthEvent = new HealthEvent();
    private DeterminationEvent determinationEvent = new DeterminationEvent();
    private GetFirstPlayerEvent getFirstPlayerEvent = new GetFirstPlayerEvent();
    private UnityEvent<int> getDeterminationEvent = new UnityEvent<int>();
    private UnityEvent<int, int> getDeterminationResponseEvent = new UnityEvent<int, int>();

    // Resources
    private ResourceEvent resourceEvent = new ResourceEvent();
    private GetResourceEvent getResourceEvent = new GetResourceEvent();

    // Island Tiles
    private SpawnIslandTileTokenEvent spawnIslandTileTokenEvent = new SpawnIslandTileTokenEvent();
    private MoveIslandTileEvent moveIslandTileEvent = new MoveIslandTileEvent();
    private DrawIslandTileEvent drawIslandTileEvent = new DrawIslandTileEvent();
    private IslandTileDrawnEvent islandTileDrawnEvent = new IslandTileDrawnEvent();
    private IslandTileRevealedEvent islandTileRevealedEvent = new IslandTileRevealedEvent();
    private IslandTileTokenEvent islandTileTokenEvent = new IslandTileTokenEvent(); // ??
    private GetDistanceFromCampEvent getDistanceFromCampEvent = new GetDistanceFromCampEvent();
    private GetTokensOnIslandTileEvent getTokensOnIslandTileEvent = new GetTokensOnIslandTileEvent();
    private GetDistanceFromCampToLocationEvent getDistanceFromCampToLocationEvent = new GetDistanceFromCampToLocationEvent();
    private LocationIsOccupiedEvent locationIsOccupiedEvent = new LocationIsOccupiedEvent();
    private UnityEvent campMovedEvent = new UnityEvent();
    private UnityEvent<Terrain, bool> terrainRevealedEvent = new UnityEvent<Terrain, bool>();
    private UnityEvent<Terrain, bool> terrainRequirementMetEvent = new UnityEvent<Terrain, bool>();
    private UnityEvent<int, bool> gatherSuccessEvent = new UnityEvent<int, bool>();
    private UnityEvent campHasNaturalShelterEvent = new UnityEvent();
    private UnityEvent<bool> campHasNaturalShelterResponseEvent = new UnityEvent<bool>();
    private UnityEvent<int, Source, bool> exhaustSourceByIslandTileIdEvent = new UnityEvent<int, Source, bool>();
    private UnityEvent<int, TokenType> destroyIslandTileTokenEvent = new UnityEvent<int, TokenType>();

    // Ongoing Effects
    private OngoingEffectEvent ongoingEffectEvent = new OngoingEffectEvent();
    private EffectIsActiveEvent effectIsActiveEvent = new EffectIsActiveEvent();
    private UnityEvent<System.Type> endOngoingEffectByTypeEvent = new UnityEvent<System.Type>();

    // Shelter, Roof, Palisade, and Weapon
    private ShelterEvent shelterEvent = new ShelterEvent();
    private RoofEvent roofEvent = new RoofEvent();
    private PalisadeEvent palisadeEvent = new PalisadeEvent();
    private GainWeaponEvent gainWeaponEvent = new GainWeaponEvent();
    private GetWeaponLevelEvent getWeaponLevelEvent = new GetWeaponLevelEvent();
    private UnityEvent getRoofLevelEvent = new UnityEvent();
    private UnityEvent<int> getRoofLevelResponseEvent = new UnityEvent<int>();
    private UnityEvent getPalisadeLevelEvent = new UnityEvent();
    private UnityEvent<int> getPalisadeLevelResponseEvent = new UnityEvent<int>();
    private UnityEvent shelterIsBuiltEvent = new UnityEvent();
    private UnityEvent<bool> shelterIsBuiltResponseEvent = new UnityEvent<bool>();

    // Inventions
    private UnityEvent<Invention> buildInventionSuccessEvent = new UnityEvent<Invention>();
    private ItemEvent itemEvent = new ItemEvent();
    private InventionIsBuiltEvent inventionIsBuiltEvent = new InventionIsBuiltEvent();
    private GetInventionCardEvent getInventionCardEvent = new GetInventionCardEvent();
    private UpdateBuiltInventionsEvent updateBuiltInventionsEvent = new UpdateBuiltInventionsEvent();
    private UnityEvent<InventionCardController, Invention> personalInventionSpawnedEvent = new UnityEvent<InventionCardController, Invention>();
    private UnityEvent<int> drawInventionCardsAndChooseOneEvent = new UnityEvent<int>();
    private UnityEvent<List<InventionCard>, int> inventionCardChosenFromSelectionEvent = new UnityEvent<List<InventionCard>, int>();


    // Miscellaneous
    private GameManagementEvent gameManagementEvent = new GameManagementEvent();

    // Turns and Phases
    private EndPhaseEvent endPhaseEvent = new EndPhaseEvent();
    private PhaseStartEvent phaseStartEvent = new PhaseStartEvent();
    private TurnStartEvent turnStartEvent = new TurnStartEvent();
    private AnimationInProgressEvent animationInProgressEvent = new AnimationInProgressEvent();
    private UnityEvent getPhaseEvent = new UnityEvent();
    private UnityEvent<Phase> getPhaseResponseEvent = new UnityEvent<Phase>();

    // Cards and Tokens
    private CardSpawnedEvent cardSpawnedEvent = new CardSpawnedEvent();
    private CardDrawnEvent cardDrawnEvent = new CardDrawnEvent();
    private CardRevealedEvent cardRevealedEvent = new CardRevealedEvent();
    private DrawCardEvent drawCardEvent = new DrawCardEvent();
    private TrackerTokenEvent trackerTokenEvent = new TrackerTokenEvent();
    private InitializeHealthTrackerTokenEvent initializeHealthTrackerTokenEvent = new InitializeHealthTrackerTokenEvent();
    private MoveComponentEvent moveComponentEvent = new MoveComponentEvent();
    private InitializeCharacterSheetEvent initializeCharacterSheetEvent = new InitializeCharacterSheetEvent();
    private SpawnTokenOnDeckEvent spawnTokenOnDeckEvent = new SpawnTokenOnDeckEvent();
    private DestroyTokenOnDeckEvent destroyTokenOnDeckEvent = new DestroyTokenOnDeckEvent();
    private UnityEvent<int, int> initializeActionPawnEvent = new UnityEvent<int, int>();
    private UnityEvent<int> shakeComponentEvent = new UnityEvent<int>();
    private UnityEvent<int> returnActionPawnEvent = new UnityEvent<int>();
    private UnityEvent<int> destroyComponentEvent = new UnityEvent<int>();
    private UnityEvent<TokenType> spawnTokenInWeatherAreaEvent = new UnityEvent<TokenType>();
    private UnityEvent<int, WoundType, TokenType> spawnWoundTokenEvent = new UnityEvent<int, WoundType, TokenType>();
    private UnityEvent<int, WoundType, TokenType> destroyWoundTokenEvent = new UnityEvent<int, WoundType, TokenType>();
    private UnityEvent<TokenType> spawnTokenOnCampEvent = new UnityEvent<TokenType>();

    // Discovery Tokens
    private InitializeDiscoveryTokenEvent initializeDiscoveryTokenEvent = new InitializeDiscoveryTokenEvent();
    private DiscoveryTokenDrawnEvent discoveryTokenDrawnEvent = new DiscoveryTokenDrawnEvent();
    private DiscoveryTokenRevealedEvent discoveryTokenRevealedEvent = new DiscoveryTokenRevealedEvent();
    private DrawDiscoveryTokenEvent drawDiscoveryTokenEvent = new DrawDiscoveryTokenEvent();
    private ActivateDiscoveryTokenEvent activateDiscoveryTokenEvent = new ActivateDiscoveryTokenEvent();

    // Actions
    private UnityEvent<int, bool> enableExploreActionAreaEvent = new UnityEvent<int, bool>();
    private UnityEvent<int, bool> enableGatherActionAreaEvent = new UnityEvent<int, bool>();
    private UnityEvent<int, bool> enableThreatActionAreaEvent = new UnityEvent<int, bool>();
    private UnityEvent actionPawnAssignedEvent = new UnityEvent();
    private UnityEvent<bool, List<ActionAssignment>> actionsReadyToSubmitEvent = new UnityEvent<bool, List<ActionAssignment>>();
    private UnityEvent<bool> enableHuntingActionSpace = new UnityEvent<bool>();
    private UnityEvent<List<ActionAssignment>> actionsSubmittedEvent = new UnityEvent<List<ActionAssignment>>();
    private UnityEvent<int> playerHasOnly1ActionThisTurnEvent = new UnityEvent<int>();
    private UnityEvent<ActionPawnController> actionPawnInitializedEvent = new UnityEvent<ActionPawnController>();
    private UnityEvent<int> playerCanOnlyRestThisTurnEvent = new UnityEvent<int>();
    private UnityEvent<int> playerCanOnlyRestBuildOrMakeCampThisTurnEvent = new UnityEvent<int>();
    private UnityEvent areSufficientResourcesAvailableEvent = new UnityEvent();
    private UnityEvent<bool> areSufficientResourcesAvailableResponseEvent = new UnityEvent<bool>();
    private UnityEvent<Invention> additionalResourceFromGatherEvent = new UnityEvent<Invention>();

    // Night Phase
    private UnityEvent<List<int>> playersEatingEvent = new UnityEvent<List<int>>(); // Used by the night phase popup to communicate which players are eating

    // UI Events
    private EnableMainUIEvent enableMainUIEvent = new EnableMainUIEvent();
    private InitializeCardPopupEvent initializeCardPopupEvent = new InitializeCardPopupEvent();
    private CardPopupClosedEvent cardPopupClosedEvent = new CardPopupClosedEvent();
    private InitializeSpritePopupEvent initializeSpritePopupEvent = new InitializeSpritePopupEvent();
    private SpritePopupClosedEvent spritePopupClosedEvent = new SpritePopupClosedEvent();
    private SpawnMoraleChoicePopupEvent spawnMoraleChoicePopupEvent = new SpawnMoraleChoicePopupEvent();
    private SpawnDiscoveryTokenActivationPopupEvent spawnDiscoveryTokenActivationPopupEvent = new SpawnDiscoveryTokenActivationPopupEvent();
    private InitializeDiscoveryTokenActivationPopupEvent initializeDiscoveryTokenActivationPopupEvent = new InitializeDiscoveryTokenActivationPopupEvent();
    private UnityEvent<ResourceCost, ActionAssignment> spawnVariableCostPopupEvent = new UnityEvent<ResourceCost, ActionAssignment>();
    private UnityEvent<int> spawnMakeCampChoicePopupEvent = new UnityEvent<int>();
    private UnityEvent<int> spawnNightPhasePopupEvent = new UnityEvent<int>();
    private UnityEvent<int, Ability> spawnAbilityPopupEvent = new UnityEvent<int, Ability>();
    private UnityEvent<List<InventionCard>> spawnChooseInventionCardPopupEvent = new UnityEvent<List<InventionCard>>();
    private UnityEvent<List<IslandTileController>, Stack<IslandTileController>> spawnReconnaissancePopupEvent = new UnityEvent<List<IslandTileController>, Stack<IslandTileController>>();
    private UnityEvent<DiscoveryTokenController, DiscoveryTokenController> spawnScoutingPopupEvent = new UnityEvent<DiscoveryTokenController, DiscoveryTokenController>();
    private UnityEvent<BeastCardController, Stack<BeastCardController>> spawnTrackingPopupEvent = new UnityEvent<BeastCardController, Stack<BeastCardController>>();
    private UnityEvent<Invention> spawnItemActivationPopupEvent = new UnityEvent<Invention>();
    private UnityEvent spawnShortcutPopupEvent = new UnityEvent();

    // Player Input
    private UnityEvent<bool, InputType> getIslandTileInputEvent = new UnityEvent<bool, InputType>();
    private UnityEvent<bool, int> adjacentTileChosenEvent = new UnityEvent<bool, int>();

    // Dice
    private UnityEvent<List<DieType>, int, bool> spawnDicePopupEvent = new UnityEvent<List<DieType>, int, bool>();
    private UnityEvent<DieType, int> dieRolledEvent = new UnityEvent<DieType, int>();

    // Adventure Cards
    private UnityEvent<AdventureType> drawAdventureCardEvent = new UnityEvent<AdventureType>();
    private UnityEvent<int, AdventureCard, int> adventureCardPopupClosedEvent = new UnityEvent<int, AdventureCard, int>();
    private UnityEvent<CardController> shuffleIntoEventDeckEvent = new UnityEvent<CardController>();

    // Abilities
    private UnityEvent<int, Ability> abilityActivatedEvent = new UnityEvent<int, Ability>();
    private UnityEvent<int> economicalConstructionEvent = new UnityEvent<int>();
    private UnityEvent<int, PawnType> spawnSingleUsePawnEvent = new UnityEvent<int, PawnType>();
    private UnityEvent reconnaissanceEvent = new UnityEvent();
    private UnityEvent scoutingEvent = new UnityEvent();
    private UnityEvent trackingEvent = new UnityEvent();

    // Weather
    private UnityEvent cancelRainCloudEvent = new UnityEvent();
    private UnityEvent convertSnowToRainEvent = new UnityEvent();

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // MoraleEvent

    public void RaiseGainMoraleEvent(int amount)
    {
        moraleEvent.Invoke(MoraleEvent.GainMorale, amount);
    }
    public void RaiseLoseMoraleEvent(int amount)
    {
        moraleEvent.Invoke(MoraleEvent.LoseMorale, amount);
    }
    public void AddListenerToMoraleEvent(UnityAction<string, int> listener)
    {
        moraleEvent.AddListener(listener);
    }

    // HealthEvent

    public void RaiseGainHealthEvent(int playerId, int amount)
    {
        healthEvent.Invoke(HealthEvent.GainHealth, playerId, amount);
    }
    public void RaiseLoseHealthEvent(int playerId, int amount)
    {
        healthEvent.Invoke(HealthEvent.LoseHealth, playerId, amount);
    }
    public void RaiseAllGainHealthEvent(int amount)
    {
        healthEvent.Invoke(HealthEvent.GainHealth, HealthEvent.AllPlayers, amount);
    }
    public void RaiseAllLoseHealthEvent(int amount)
    {
        healthEvent.Invoke(HealthEvent.LoseHealth, HealthEvent.AllPlayers, amount);
    }
    public void AddListenerToHealthEvent(UnityAction<string, int, int> listener)
    {
        healthEvent.AddListener(listener);
    }

    // DeterminationEvent

    public void RaiseGainDeterminationEvent(int playerId, int amount)
    {
        determinationEvent.Invoke(DeterminationEvent.GainDetermination, playerId, amount);
    }
    public void RaiseLoseDeterminationEvent(int playerId, int amount)
    {
        determinationEvent.Invoke(DeterminationEvent.LoseDetermination, playerId, amount);
    }
    public void RaiseAllLoseDeterminationEvent(int amount)
    {
        determinationEvent.Invoke(DeterminationEvent.LoseDetermination, DeterminationEvent.AllPlayers, amount);
    }
    public void AddListenerToDeterminationEvent(UnityAction<string, int, int> listener)
    {
        determinationEvent.AddListener(listener);
    }

    // GetFirstPlayerEvent

    public void RaiseGetFirstPlayerEvent()
    {
        getFirstPlayerEvent.Invoke(GetFirstPlayerEvent.Query, 0);
    }
    public void RaiseGetFirstPlayerResponseEvent(int playerId)
    {
        getFirstPlayerEvent.Invoke(GetFirstPlayerEvent.Response, playerId);
    }
    public void AddListenerToGetFirstPlayerEvent(UnityAction<string, int> listener)
    {
        getFirstPlayerEvent.AddListener(listener);
    }

    // GetDeterminationEvent

    public void RaiseGetDeterminationEvent(int playerId)
    {
        getDeterminationEvent.Invoke(playerId);
    }
    public void AddListenerToGetDeterminationEvent(UnityAction<int> listener)
    {
        getDeterminationEvent.AddListener(listener);
    }

    // Get determination response event

    public void RaiseGetDeterminationResponseEvent(int playerId, int determination)
    {
        getDeterminationResponseEvent.Invoke(playerId, determination);
    }
    public void AddListenerToGetDeterminationResponseEvent(UnityAction<int, int> listener)
    {
        getDeterminationResponseEvent.AddListener(listener);
    }

    // GameManagementEvent

    public void RaiseEndGameEvent(bool isVictory)
    {
        gameManagementEvent.Invoke(GameManagementEvent.EndGame, isVictory);
    }
    public void AddListenerToGameManagementEvent(UnityAction<string, bool> listener)
    {
        gameManagementEvent.AddListener(listener);
    }

    // ResourceEvent

    public void RaiseGainFoodEvent(int amount)
    {
        resourceEvent.Invoke(ResourceEvent.GainFood, amount);
    }
    public void RaiseLoseFoodEvent(int amount)
    {
        resourceEvent.Invoke(ResourceEvent.LoseFood, amount);
    }

    public void RaiseGainHideEvent(int amount)
    {
        resourceEvent.Invoke(ResourceEvent.GainHide, amount);
    }
    public void RaiseLoseHideEvent(int amount)
    {
        resourceEvent.Invoke(ResourceEvent.LoseHide, amount);
    }

    public void RaiseGainNonPerishableFoodEvent(int amount)
    {
        resourceEvent.Invoke(ResourceEvent.GainNonPerishableFood, amount);
    }
    public void RaiseLoseNonPerishableFoodEvent(int amount)
    {
        resourceEvent.Invoke(ResourceEvent.LoseNonPerishableFood, amount);
    }

    public void RaiseGainWoodEvent(int amount)
    {
        resourceEvent.Invoke(ResourceEvent.GainWood, amount);
    }
    public void RaiseLoseWoodEvent(int amount)
    {
        resourceEvent.Invoke(ResourceEvent.LoseWood, amount);
    }

    public void RaiseMakeResourcesAvailableEvent()
    {
        resourceEvent.Invoke(ResourceEvent.MakeResourcesAvailable, 0);
    }

    public void AddListenerToResourceEvent(UnityAction<string, int> listener)
    {
        resourceEvent.AddListener(listener);
    }

    // GetResourceEvent

    public void RaiseGetHideEvent()
    {
        getResourceEvent.Invoke(GetResourceEvent.GetHide, 0);
    }
    public void RaiseGetFoodEvent()
    {
        getResourceEvent.Invoke(GetResourceEvent.GetFood, 0);
    }
    public void RaiseGetNonPerishableFoodEvent()
    {
        getResourceEvent.Invoke(GetResourceEvent.GetNonPerishableFood, 0);
    }
    public void RaiseGetWoodEvent()
    {
        getResourceEvent.Invoke(GetResourceEvent.GetWood, 0);
    }

    public void RaiseGetHideResponseEvent(int amount)
    {
        getResourceEvent.Invoke(GetResourceEvent.GetHideResponse, amount);
    }
    public void RaiseGetFoodResponseEvent(int amount)
    {
        getResourceEvent.Invoke(GetResourceEvent.GetFoodResponse, amount);
    }
    public void RaiseGetNonPerishableFoodResponseEvent(int amount)
    {
        getResourceEvent.Invoke(GetResourceEvent.GetNonPerishableFoodResponse, amount);
    }
    public void RaiseGetWoodResponseEvent(int amount)
    {
        getResourceEvent.Invoke(GetResourceEvent.GetWoodResponse, amount);
    }

    public void AddListenerToGetResourceEvent(UnityAction<string, int> listener)
    {
        getResourceEvent.AddListener(listener);
    }

    // SpawnIslandTileTokenEvent

    public void RaiseSpawnIslandTileTokenEvent(TokenType tokenType, int locationId)
    {
        spawnIslandTileTokenEvent.Invoke(tokenType, locationId);
    }
    public void AddListenerToSpawnIslandTileTokenEvent(UnityAction<TokenType, int> listener)
    {
        spawnIslandTileTokenEvent.AddListener(listener);
    }

    // MoveIslandTileEvent

    public void RaiseMoveIslandTileEvent(int componentId, int locationId)
    {
        moveIslandTileEvent.Invoke(componentId, locationId);
    }
    public void AddListenerToMoveIslandTileEvent(UnityAction<int, int> listener)
    {
        moveIslandTileEvent.AddListener(listener);
    }

    // DrawIslandTileEvent

    public void RaiseDrawIslandTileEvent(int locationId)
    {
        drawIslandTileEvent.Invoke(locationId);
    }
    public void AddListenerToDrawIslandTileEvent(UnityAction<int> listener)
    {
        drawIslandTileEvent.AddListener(listener);
    }

    // IslandTileTokenEvent

    public void RaiseSetTokenPositionEvent(int componentId, IslandTileTokenController.Position position)
    {
        islandTileTokenEvent.Invoke(IslandTileTokenEvent.SetTokenPositionById, componentId, TokenType.Food, position);
    }
    public void RaiseSetTokenPositionEvent(int componentId, TokenType tokenType, IslandTileTokenController.Position position)
    {
        islandTileTokenEvent.Invoke(IslandTileTokenEvent.SetTokenPositionById, componentId, tokenType, position);
    }
    public void RaiseTurnCampTokenFaceUpEvent()
    {
        islandTileTokenEvent.Invoke(IslandTileTokenEvent.TurnCampTokenFaceUp, 0, TokenType.Food, IslandTileTokenController.Position.None);
    }
    public void RaiseTurnCampTokenFaceDownEvent()
    {
        islandTileTokenEvent.Invoke(IslandTileTokenEvent.TurnCampTokenFaceDown, 0, TokenType.Food, IslandTileTokenController.Position.None);
    }
    public void AddListenerToIslandTileTokenEvent(UnityAction<string, int, TokenType, IslandTileTokenController.Position> listener)
    {
        islandTileTokenEvent.AddListener(listener);
    }

    // IslandTileDrawnEvent

    public void RaiseIslandTileDrawnEvent(int componentId, int locationId)
    {
        islandTileDrawnEvent.Invoke(componentId, locationId);
    }
    public void AddListenerToIslandTileDrawnEvent(UnityAction<int, int> listener)
    {
        islandTileDrawnEvent.AddListener(listener);
    }

    // IslandTileRevealedEvent

    public void RaiseIslandTileRevealedEvent(int componentId, Sprite sprite)
    {
        islandTileRevealedEvent.Invoke(componentId, sprite);
    }
    public void AddListenerToIslandTileRevealedEvent(UnityAction<int, Sprite> listener)
    {
        islandTileRevealedEvent.AddListener(listener);
    }

    // OngoingEffectEvent

    public void RaiseStartOngoingEffectEvent(OngoingCardEffect ongoingEffect)
    {
        ongoingEffectEvent.Invoke(OngoingEffectEvent.StartOngoingEffect, ongoingEffect, 0, Trigger.EndTurn);
    }
    public void RaiseEndOngoingEffectEvent(int cardEffectId)
    {
        ongoingEffectEvent.Invoke(OngoingEffectEvent.EndOngoingEffect, null, cardEffectId, Trigger.EndTurn);
    }
    public void RaiseApplyEndTriggerEvent(Trigger endTrigger)
    {
        ongoingEffectEvent.Invoke(OngoingEffectEvent.ApplyEndTrigger, null, 0, endTrigger);
    }
    public void RaiseApplyEffectTriggerEvent(Trigger effectTrigger)
    {
        ongoingEffectEvent.Invoke(OngoingEffectEvent.ApplyEffectTrigger, null, 0, effectTrigger);
    }
    public void AddListenerToOngoingEffectEvent(UnityAction<string, OngoingCardEffect, int, Trigger> listener)
    {
        ongoingEffectEvent.AddListener(listener);
    }

    // EffectIsActiveEvent

    public void RaiseEffectIsActiveEvent(int cardEffectId)
    {
        effectIsActiveEvent.Invoke(EffectIsActiveEvent.RequestById, cardEffectId, this.GetType(), false); // this.GetType() is a placeholder
    }
    public void RaiseEffectIsActiveEvent(System.Type cardEffectType)
    {
        effectIsActiveEvent.Invoke(EffectIsActiveEvent.RequestByType, 0, cardEffectType, false);
    }
    public void RaiseEffectIsActiveResponseEvent(int cardEffectId, bool response)
    {
        effectIsActiveEvent.Invoke(EffectIsActiveEvent.Response, cardEffectId, this.GetType(), response);
    }
    public void RaiseEffectIsActiveResponseEvent(System.Type cardEffectType, bool response)
    {
        effectIsActiveEvent.Invoke(EffectIsActiveEvent.Response, 0, cardEffectType, response);
    }
    public void AddListenerToEffectIsActiveEvent(UnityAction<string, int, System.Type, bool> listener)
    {
        effectIsActiveEvent.AddListener(listener);
    }

    // End ongoing effect by type

    public void RaiseEndOngoingEffectByTypeEvent(System.Type ongoingEffectType)
    {
        endOngoingEffectByTypeEvent.Invoke(ongoingEffectType);
    }
    public void AddListenerToEndOngoingEffectByTypeEvent(UnityAction<System.Type> listener)
    {
        endOngoingEffectByTypeEvent.AddListener(listener);
    }

    // ShelterEvent

    public void RaiseLoseShelterEvent()
    {
        shelterEvent.Invoke(ShelterEvent.LoseShelter);
    }
    public void RaiseGainShelterEvent()
    {
        shelterEvent.Invoke(ShelterEvent.GainShelter);
    }
    public void AddListenerToShelterEvent(UnityAction<string> listener)
    {
        shelterEvent.AddListener(listener);
    }

    // RoofEvent

    public void RaiseGainRoofEvent(int amount)
    {
        roofEvent.Invoke(RoofEvent.GainRoof, amount);
    }
    public void RaiseLoseRoofEvent(int amount)
    {
        roofEvent.Invoke(RoofEvent.LoseRoof, amount);
    }
    public void RaiseLoseHalfRoofEvent()
    {
        roofEvent.Invoke(RoofEvent.LoseHalfRoof, 0);
    }
    public void AddListenerToRoofEvent(UnityAction<string, int> listener)
    {
        roofEvent.AddListener(listener);
    }

    // PalisadeEvent

    public void RaiseGainPalisadeEvent(int amount)
    {
        palisadeEvent.Invoke(PalisadeEvent.GainPalisade, amount);
    }
    public void RaiseLosePalisadeEvent(int amount)
    {
        palisadeEvent.Invoke(PalisadeEvent.LosePalisade, amount);
    }
    public void RaiseLoseHalfPalisadeEvent()
    {
        palisadeEvent.Invoke(PalisadeEvent.LoseHalfPalisade, 0);
    }
    public void AddListenerToPalisadeEvent(UnityAction<string, int> listener)
    {
        palisadeEvent.AddListener(listener);
    }

    // GainWeaponEvent

    public void RaiseGainWeaponEvent(int amount)
    {
        gainWeaponEvent.Invoke(amount);
    }
    public void RaiseLoseWeaponEvent(int amount)
    {
        gainWeaponEvent.Invoke(-amount);
    }
    public void AddListenerToGainWeaponEvent(UnityAction<int> listener)
    {
        gainWeaponEvent.AddListener(listener);
    }

    // GetWeaponLevelEvent

    public void RaiseGetWeaponLevelEvent()
    {
        getWeaponLevelEvent.Invoke(GetWeaponLevelEvent.Query, 0);
    }
    public void RaiseGetWeaponLevelResponseEvent(int response)
    {
        getWeaponLevelEvent.Invoke(GetWeaponLevelEvent.Response, response);
    }
    public void AddListenerToGetWeaponLevelEvent(UnityAction<string, int> listener)
    {
        getWeaponLevelEvent.AddListener(listener);
    }

    // Get roof level event

    public void RaiseGetRoofLevelEvent()
    {
        getRoofLevelEvent.Invoke();
    }
    public void AddListenerToGetRoofLevelEvent(UnityAction listener)
    {
        getRoofLevelEvent.AddListener(listener);
    }

    // Get roof level response event

    public void RaiseGetRoofLevelResponseEvent(int roofLevel)
    {
        getRoofLevelResponseEvent.Invoke(roofLevel);
    }
    public void AddListenerToGetRoofLevelResponseEvent(UnityAction<int> listener)
    {
        getRoofLevelResponseEvent.AddListener(listener);
    }

    // Get pal;isade level event

    public void RaiseGetPalisadeLevelEvent()
    {
        getPalisadeLevelEvent.Invoke();
    }
    public void AddListenerToGetPalisadeLevelEvent(UnityAction listener)
    {
        getPalisadeLevelEvent.AddListener(listener);
    }

    // Get palisade level response event

    public void RaiseGetPalisadeLevelResponseEvent(int palisadeLevel)
    {
        getPalisadeLevelResponseEvent.Invoke(palisadeLevel);
    }
    public void AddListenerToGetPalisadeLevelResponseEvent(UnityAction<int> listener)
    {
        getPalisadeLevelResponseEvent.AddListener(listener);
    }

    // build invention success event

    public void RaiseBuildInventionSuccessEvent(Invention invention)
    {
        buildInventionSuccessEvent.Invoke(invention);
    }
    public void AddListenerToBuildInventionSuccessEvent(UnityAction<Invention> listener)
    {
        buildInventionSuccessEvent.AddListener(listener);
    }

    // ItemEvent

    public void RaiseLoseItemEvent(Invention item)
    {
        itemEvent.Invoke(ItemEvent.LoseItem, item);
    }
    public void RaiseDiscardInventionCardEvent(Invention invention)
    {
        itemEvent.Invoke(ItemEvent.DiscardInventionCard, invention);
    }
    public void AddListenerToItemEvent(UnityAction<string, Invention> listener)
    {
        itemEvent.AddListener(listener);
    }

    // InventionIsBuiltEvent

    public void RaiseInventionIsBuiltEvent(Invention invention)
    {
        inventionIsBuiltEvent.Invoke(InventionIsBuiltEvent.Query, invention, false);
    }
    public void RaiseInventionIsBuiltResponseEvent(Invention invention, bool response)
    {
        inventionIsBuiltEvent.Invoke(InventionIsBuiltEvent.Response, invention, response);
    }
    public void AddListenerToInventionIsBuiltEvent(UnityAction<string, Invention, bool> listener)
    {
        inventionIsBuiltEvent.AddListener(listener);
    }

    // GetInventionCardEvent

    public void RaiseGetInventionCardEvent(Invention invention)
    {
        getInventionCardEvent.Invoke(GetInventionCardEvent.Query, invention, null);
    }
    public void RaiseGetInventionCardResponseEvent(Invention invention, InventionCard inventionCard)
    {
        getInventionCardEvent.Invoke(GetInventionCardEvent.Response, invention, inventionCard);
    }
    public void AddListenerToGetInventionCardEvent(UnityAction<string, Invention, InventionCard> listener)
    {
        getInventionCardEvent.AddListener(listener);
    }

    // UpdateBuiltInventionsEvent

    public void RaiseUpdateBuiltInventionsEvent(Invention invention, bool isBuilt)
    {
        updateBuiltInventionsEvent.Invoke(invention, isBuilt);
    }
    public void AddListenerToUpdateBuiltInventionsEvent(UnityAction<Invention, bool> listener)
    {
        updateBuiltInventionsEvent.AddListener(listener);
    }

    // Personal invention spawned event

    public void RaisePersonalInventionSpawnedEvent(InventionCardController inventionCardController, Invention invention)
    {
        personalInventionSpawnedEvent.Invoke(inventionCardController, invention);
    }
    public void AddListenerToPersonalInventionSpawnedEvent(UnityAction<InventionCardController, Invention> listener)
    {
        personalInventionSpawnedEvent.AddListener(listener);
    }

    // Draw invention cards and choose one

    public void RaiseDrawInventionCardsAndChooseOneEvent(int numberOfCards)
    {
        drawInventionCardsAndChooseOneEvent.Invoke(numberOfCards);
    }
    public void AddListenerToDrawInventionCardsAndChooseOneEvent(UnityAction<int> listener)
    {
        drawInventionCardsAndChooseOneEvent.AddListener(listener);
    }

    // Invention card chosen from selection event

    public void RaiseInventionCardChosenFromSelectionEvent(List<InventionCard> inventionCards, int indexOfChosenCard)
    {
        inventionCardChosenFromSelectionEvent.Invoke(inventionCards, indexOfChosenCard);
    }
    public void AddListenerToInventionCardChosenFromSelectionEvent(UnityAction<List<InventionCard>, int> listener)
    {
        inventionCardChosenFromSelectionEvent.AddListener(listener);
    }

    // TrackerTokenEvent

    public void RaiseSetMoraleTrackerEvent(int newValue)
    {
        trackerTokenEvent.Invoke(TrackerTokenEvent.SetMoraleTracker, newValue);
    }
    public void RaiseSetPalisadeTrackerEvent(int newValue)
    {
        trackerTokenEvent.Invoke(TrackerTokenEvent.SetPalisadeTracker, newValue);
    }
    public void RaiseSetRoofTrackerEvent(int newValue)
    {
        trackerTokenEvent.Invoke(TrackerTokenEvent.SetRoofTracker, newValue);
    }
    public void RaiseSetWeaponTrackerEvent(int newValue)
    {
        trackerTokenEvent.Invoke(TrackerTokenEvent.SetWeaponTracker, newValue);
    }
    public void RaiseSetHealthTrackerEvent(int playerId, int newValue)
    {
        switch (playerId)
        {
            case 0: trackerTokenEvent.Invoke(TrackerTokenEvent.SetPlayer0HealthTracker, newValue); break;
            case 1: trackerTokenEvent.Invoke(TrackerTokenEvent.SetPlayer1HealthTracker, newValue); break;
            case 2: trackerTokenEvent.Invoke(TrackerTokenEvent.SetPlayer2HealthTracker, newValue); break;
            case 3: trackerTokenEvent.Invoke(TrackerTokenEvent.SetPlayer3HealthTracker, newValue); break;
            default: Debug.LogError("Invalid player ID passed to RaiseSetHealthTrackerEvent(int playerId, int newValue)."); break;
        }
    }
    public void AddListenerToTrackerTokenEvent(UnityAction<string, int> listener)
    {
        trackerTokenEvent.AddListener(listener);
    }

    // InitializeHealthTrackerTokenEvent

    public void RaiseInitializeHealthTrackerTokenEvent(int componentId, int playerId, int maximumHealth)
    {
        initializeHealthTrackerTokenEvent.Invoke(componentId, playerId, maximumHealth);
    }
    public void AddListenerToInitializeHealthTrackerTokenEvent(UnityAction<int, int, int> listener)
    {
        initializeHealthTrackerTokenEvent.AddListener(listener);
    }

    // MoveComponentEvent

    public void RaiseMoveComponentEvent(int componentId, Transform transform, Vector3 localPosition, MoveStyle moveStyle)
    {
        moveComponentEvent.Invoke(componentId, transform, localPosition, moveStyle);
    }
    public void RaiseMoveComponentEvent(int componentId, Vector3 localPosition, MoveStyle moveStyle)
    {
        moveComponentEvent.Invoke(componentId, null, localPosition, moveStyle);
    }
    public void RaiseMoveComponentEvent(int componentId, Transform transform, Vector3 localPosition)
    {
        moveComponentEvent.Invoke(componentId, transform, localPosition, MoveStyle.Default);
    }
    public void RaiseMoveComponentEvent(int componentId, Vector3 localPosition)
    {
        moveComponentEvent.Invoke(componentId, null, localPosition, MoveStyle.Default);
    }
    public void RaiseMoveComponentEvent(int componentId, Transform transform)
    {
        moveComponentEvent.Invoke(componentId, transform, Vector3.zero, MoveStyle.Default);
    }
    public void AddListenerToMoveComponentEvent(UnityAction<int, Transform, Vector3, MoveStyle> listener)
    {
        moveComponentEvent.AddListener(listener);
    }

    // InitializeCharacterSheetEvent

    public void RaiseInitializeCharacterSheetEvent(int componentId, int playerId, Character character)
    {
        initializeCharacterSheetEvent.Invoke(componentId, playerId, character);
    }
    public void AddListenerToInitializeCharacterSheetEvent(UnityAction<int, int, Character> listener)
    {
        initializeCharacterSheetEvent.AddListener(listener);
    }

    // SpawnTokenOnDeckEvent

    public void RaiseSpawnTokenOnDeckEvent(Deck deck, TokenType tokenType)
    {
        spawnTokenOnDeckEvent.Invoke(deck, tokenType);
    }
    public void AddListenerToSpawnTokenOnDeckEvent(UnityAction<Deck, TokenType> listener)
    {
        spawnTokenOnDeckEvent.AddListener(listener);
    }

    // DestroyTokenOnDeckEvent

    public void RaiseDestroyTokenDeckevent(Deck deck, TokenType tokenType)
    {
        destroyTokenOnDeckEvent.Invoke(deck, tokenType);
    }
    public void AddListenerToDestroyTokenOnDeckEvent(UnityAction<Deck, TokenType> listener)
    {
        destroyTokenOnDeckEvent.AddListener(listener);
    }

    // InitializeActionPawn

    public void RaiseInitializeActionPawnEvent(int componentId, int playerId)
    {
        initializeActionPawnEvent.Invoke(componentId, playerId);
    }
    public void AddListenerToInitializeActionPawnEvent(UnityAction<int, int> listener)
    {
        initializeActionPawnEvent.AddListener(listener);
    }

    // Shake component event

    public void RaiseShakeComponentEvent(int componentId)
    {
        shakeComponentEvent.Invoke(componentId);
    }
    public void AddListenerToShakeComponentEvent(UnityAction<int> listener)
    {
        shakeComponentEvent.AddListener(listener);
    }

    // Return action pawn event

    public void RaiseReturnActionPawnEvent(int componentId)
    {
        returnActionPawnEvent.Invoke(componentId);
    }
    public void AddListenerToReturnActionPawnEvent(UnityAction<int> listener)
    {
        returnActionPawnEvent.AddListener(listener);
    }

    // Destroy component event

    public void RaiseDestroyComponentEvent(int componentId)
    {
        destroyComponentEvent.Invoke(componentId);
    }
    public void AddListenerToDestroyComponentEvent(UnityAction<int> listener)
    {
        destroyComponentEvent.AddListener(listener);
    }

    // Spawn token in weather area

    public void RaiseSpawnTokenInWeatherAreaEvent(TokenType tokenType)
    {
        spawnTokenInWeatherAreaEvent.Invoke(tokenType);
    }
    public void AddListenerToSpawnTokenInWeatherAreaEvent(UnityAction<TokenType> listener)
    {
        spawnTokenInWeatherAreaEvent.AddListener(listener);
    }

    // Spawn wound tokens

    public void RaiseSpawnWoundTokenEvent(int playerId, WoundType woundType, TokenType tokenType)
    {
        spawnWoundTokenEvent.Invoke(playerId, woundType, tokenType);
    }
    public void AddListenerToSpawnWoundTokenEvent(UnityAction<int, WoundType, TokenType> listener)
    {
        spawnWoundTokenEvent.AddListener(listener);
    }

    // Destroy wound tokens

    public void RaiseDestroyWoundTokenEvent(int playerId, WoundType woundType, TokenType tokenType)
    {
        destroyWoundTokenEvent.Invoke(playerId, woundType, tokenType);
    }
    public void AddListenerToDestroyWoundTokenEvent(UnityAction<int, WoundType, TokenType> listener)
    {
        destroyWoundTokenEvent.AddListener(listener);
    }

    // Spawn token on camp

    public void RaiseSpawnTokenOnCampEvent(TokenType tokenType)
    {
        spawnTokenOnCampEvent.Invoke(tokenType);
    }
    public void AddListenerToSpawnTokenOnCampEvent(UnityAction<TokenType> listener)
    {
        spawnTokenOnCampEvent.AddListener(listener);
    }

    // InitializeDiscoveryTokenEvent

    public void RaiseInitializeDiscoveryTokenEvent(int componentId, DiscoveryToken discoveryToken)
    {
        initializeDiscoveryTokenEvent.Invoke(componentId, discoveryToken);
    }
    public void AddListenerToInitializeDiscoveryTokenEvent(UnityAction<int, DiscoveryToken> listener)
    {
        initializeDiscoveryTokenEvent.AddListener(listener);
    }

    // DiscoveryTokenDrawnEvent

    public void RaiseDiscoveryTokenDrawnEvent(int componentId)
    {
        discoveryTokenDrawnEvent.Invoke(componentId);
    }
    public void AddListenerToDiscoveryTokenDrawnEvent(UnityAction<int> listener)
    {
        discoveryTokenDrawnEvent.AddListener(listener);
    }

    // DiscoveryTokenRevealedEvent

    public void RaiseDiscoveryTokenRevealedEvent(int sourceComponentId, Sprite sprite)
    {
        discoveryTokenRevealedEvent.Invoke(sourceComponentId, sprite);
    }
    public void AddListenerToDiscoveryTokenRevealedEvent(UnityAction<int, Sprite> listener)
    {
        discoveryTokenRevealedEvent.AddListener(listener);
    }

    // DrawDiscoveryTokenEvent

    public void RaiseDrawDiscoveryTokenEvent(int amount)
    {
        drawDiscoveryTokenEvent.Invoke(amount);
    }
    public void AddListenerToDrawDiscoveryTokenEvent(UnityAction<int> listener)
    {
        drawDiscoveryTokenEvent.AddListener(listener);
    }

    // ActivateDiscoveryTokenEvent

    public void RaiseActivateDiscoveryTokenEvent(int sourceComponentId)
    {
        activateDiscoveryTokenEvent.Invoke(sourceComponentId, -1);
    }
    public void RaiseActivateDiscoveryTokenEvent(int sourceComponentId, int selectedPlayerId)
    {
        activateDiscoveryTokenEvent.Invoke(sourceComponentId, selectedPlayerId);
    }
    public void AddListenerToActivateDiscoveryTokenEvent(UnityAction<int, int> listener)
    {
        activateDiscoveryTokenEvent.AddListener(listener);
    }

    // Enable explore action area

    public void RaiseEnableExploreActionAreaEvent(int locationId, bool enable)
    {
        enableExploreActionAreaEvent.Invoke(locationId, enable);
    }
    public void AddListenerToEnableExploreActionAreaEvent(UnityAction<int, bool> listener)
    {
        enableExploreActionAreaEvent.AddListener(listener);
    }

    // Enable gather action area event

    public void RaiseEnableGatherActionAreaEvent(int islandTileId, bool enable)
    {
        enableGatherActionAreaEvent.Invoke(islandTileId, enable);
    }
    public void AddListenerToEnableGatherActionAreaEvent(UnityAction<int, bool> listener)
    {
        enableGatherActionAreaEvent.AddListener(listener);
    }

    // Enable threat action area event

    public void RaiseEnableThreatActionAreaEvent(int componentId, bool enable)
    {
        enableThreatActionAreaEvent.Invoke(componentId, enable);
    }
    public void AddListenerToEnableThreatActionAreaEvent(UnityAction<int, bool> listener)
    {
        enableThreatActionAreaEvent.AddListener(listener);
    }

    // Action pawn assigned event

    public void RaiseActionPawnAssignedEvent()
    {
        actionPawnAssignedEvent.Invoke();
    }
    public void AddListenerToActionPawnAssignedEvent(UnityAction listener)
    {
        actionPawnAssignedEvent.AddListener(listener);
    }

    // Actions ready to submit event

    public void RaiseActionsReadyToSubmitEvent(bool actionsReadyToSubmit, List<ActionAssignment> actionAssignments)
    {
        actionsReadyToSubmitEvent.Invoke(actionsReadyToSubmit, actionAssignments);
    }
    public void AddListenerToActionsReadyToSubmitEvent(UnityAction<bool, List<ActionAssignment>> listener)
    {
        actionsReadyToSubmitEvent.AddListener(listener);
    }

    // Enable hunting action space

    public void RaiseEnableHuntingActionSpaceEvent(bool enable)
    {
        enableHuntingActionSpace.Invoke(enable);
    }
    public void AddListenerToEnableHuntingActionSpaceEvent(UnityAction<bool> listener)
    {
        enableHuntingActionSpace.AddListener(listener);
    }

    // Actions submitted

    public void RaiseActionsSubmittedEvent(List<ActionAssignment> actionAssignments)
    {
        actionsSubmittedEvent.Invoke(actionAssignments);
    }
    public void AddListenerToActionsSubmittedEvent(UnityAction<List<ActionAssignment>> listener)
    {
        actionsSubmittedEvent.AddListener(listener);
    }

    // Players has only 1 action this turn

    public void RaisePlayerHasOnly1ActionThisTurnEvent(int playerId)
    {
        playerHasOnly1ActionThisTurnEvent.Invoke(playerId);
    }
    public void AddListenerToPlayerHasOnly1ActionThisTurnEvent(UnityAction<int> listener)
    {
        playerHasOnly1ActionThisTurnEvent.AddListener(listener);
    }

    // Action pawn initialized event (tells the ActionParser to add the ActionPawnController to its list)

    public void RaiseActionPawnInitializedEvent(ActionPawnController actionPawn)
    {
        actionPawnInitializedEvent.Invoke(actionPawn);
    }
    public void AddListenerToActionPawnInitializedEvent(UnityAction<ActionPawnController> listener)
    {
        actionPawnInitializedEvent.AddListener(listener);
    }

    // Player can only rest this round

    public void RaisePlayerCanOnlyRestThisTurnEvent(int playerId)
    {
        playerCanOnlyRestThisTurnEvent.Invoke(playerId);
    }
    public void AddListenerToPlayerCanOnlyRestThisTurnEvent(UnityAction<int> listener)
    {
        playerCanOnlyRestThisTurnEvent.AddListener(listener);
    }

    // Player can only rest, build, or make camp this turn

    public void RaisePlayerCanOnlyRestBuildOrMakeCampThisTurnEvent(int playerId)
    {
        playerCanOnlyRestBuildOrMakeCampThisTurnEvent.Invoke(playerId);
    }
    public void AddListenerToPlayerCanOnlyRestBuildOrMakeCampThisTurnEvent(UnityAction<int> listener)
    {
        playerCanOnlyRestBuildOrMakeCampThisTurnEvent.AddListener(listener);
    }

    // Are sufficient resources available

    public void RaiseAreSufficientResourcesAvailableEvent()
    {
        areSufficientResourcesAvailableEvent.Invoke();
    }
    public void AddListenerToAreSufficientResourcesAvailableEvent(UnityAction listener)
    {
        areSufficientResourcesAvailableEvent.AddListener(listener);
    }

    // Are sufficient resources available response

    public void RaiseAreSufficientResourcesAvailableResponseEvent(bool response)
    {
        areSufficientResourcesAvailableResponseEvent.Invoke(response);
    }
    public void AddListenerToAreSufficientResourcesAvailableResponseEvent(UnityAction<bool> listener)
    {
        areSufficientResourcesAvailableResponseEvent.AddListener(listener);
    }

    // Additional resource from gather (used by the sack/ basket)

    public void RaiseAdditionalResourceFromGatherEvent(Invention itemUsed)
    {
        additionalResourceFromGatherEvent.Invoke(itemUsed);
    }
    public void AddListenerToAdditionalResourceFromGatherEvent(UnityAction<Invention> listener)
    {
        additionalResourceFromGatherEvent.AddListener(listener);
    }



    // Players eating

    public void RaisePlayersEatingEvent(List<int> playersEating)
    {
        playersEatingEvent.Invoke(playersEating);
    }
    public void AddListenerToPlayersEatingEvent(UnityAction<List<int>> listener)
    {
        playersEatingEvent.AddListener(listener);
    }

    // GetDistanceFromCampEvent

    public void RaiseGetDistanceFromCampEvent(int islandTileId)
    {
        getDistanceFromCampEvent.Invoke(GetDistanceFromCampEvent.Query, islandTileId, -1);
    }
    public void RaiseGetDistanceFromCampResponseEvent(int islandTileId, int distance)
    {
        getDistanceFromCampEvent.Invoke(GetDistanceFromCampEvent.Response, islandTileId, distance);
    }
    public void AddListenerToGetDistanceFromCampEvent(UnityAction<string, int, int> listener)
    {
        getDistanceFromCampEvent.AddListener(listener);
    }

    // GetTokensOnIslandTileEvent

    public void RaiseGetTokensOnIslandTileEvent(int islandTileId)
    {
        getTokensOnIslandTileEvent.Invoke(GetTokensOnIslandTileEvent.Query, islandTileId, null);
    }
    public void RaiseGetTokensOnIslandTileResponseEvent(int islandTileId, List<TokenType> tokenTypes)
    {
        getTokensOnIslandTileEvent.Invoke(GetTokensOnIslandTileEvent.Response, islandTileId, tokenTypes);
    }
    public void AddListenerToGetTokensOnIslandTileEvent(UnityAction<string, int, List<TokenType>> listener)
    {
        getTokensOnIslandTileEvent.AddListener(listener);
    }

    // GetDistanceFromCampToLocationEvent

    public void RaiseGetDistanceFromCampToLocationEvent(int locationId)
    {
        getDistanceFromCampToLocationEvent.Invoke(GetDistanceFromCampToLocationEvent.Query, locationId, -1);
    }
    public void RaiseGetDistanceFromCampToLocationResponseEvent(int locationId, int distance)
    {
        getDistanceFromCampToLocationEvent.Invoke(GetDistanceFromCampToLocationEvent.Response, locationId, distance);
    }
    public void AddListenerToGetDistanceFromCampToLocationEvent(UnityAction<string, int, int> listener)
    {
        getDistanceFromCampToLocationEvent.AddListener(listener);
    }

    // Location is occupied event

    public void RaiseLocationIsOccupiedEvent(int locationId)
    {
        locationIsOccupiedEvent.Invoke(LocationIsOccupiedEvent.Query, locationId, false);
    }
    public void RaiseLocationIsOccupiedResponseEvent(int locationId, bool isOccupied)
    {
        locationIsOccupiedEvent.Invoke(LocationIsOccupiedEvent.Response, locationId, isOccupied);
    }
    public void AddListenerToLocationIsOccupiedEvent(UnityAction<string, int, bool> listener)
    {
        locationIsOccupiedEvent.AddListener(listener);
    }

    // CampMovedEvent

    public void RaiseCampMovedEvent()
    {
        campMovedEvent.Invoke();
    }
    public void AddListenerToCampMovedEvent(UnityAction listener)
    {
        campMovedEvent.AddListener(listener);
    }

    // Terrain revealed event

    public void RaiseTerrainRevealedEvent(Terrain terrainType, bool isRevealed)
    {
        terrainRevealedEvent.Invoke(terrainType, isRevealed);
    }
    public void AddListenerToTerrainRevealedEvent(UnityAction<Terrain, bool> listener)
    {
        terrainRevealedEvent.AddListener(listener);
    }

    // Terrain requirement met

    public void RaiseTerrainRequirementMetEvent(Terrain terrainType, bool requirementMet)
    {
        terrainRequirementMetEvent.Invoke(terrainType, requirementMet);
    }
    public void AddListenerToTerrainRequirementMetEvent(UnityAction<Terrain, bool> listener)
    {
        terrainRequirementMetEvent.AddListener(listener);
    }

    // Gather success event

    public void RaiseGatherSuccessEvent(int islandTileId, bool isRightSource)
    {
        gatherSuccessEvent.Invoke(islandTileId, isRightSource);
    }
    public void AddListenerToGatherSuccessEvent(UnityAction<int, bool> listener)
    {
        gatherSuccessEvent.AddListener(listener);
    }

    // Camp has natural shelter event

    public void RaiseCampHasNaturalShelterEvent()
    {
        campHasNaturalShelterEvent.Invoke();
    }
    public void AddListenerToCampHasNaturalShelterEvent(UnityAction listener)
    {
        campHasNaturalShelterEvent.AddListener(listener);
    }

    // Camp has natural shelter response event

    public void RaiseCampHasNaturalShelterResponseEvent(bool campHasNaturalShelter)
    {
        campHasNaturalShelterResponseEvent.Invoke(campHasNaturalShelter);
    }
    public void AddListenerToCampHasNaturalShelterResponseEvent(UnityAction<bool> listener)
    {
        campHasNaturalShelterResponseEvent.AddListener(listener);
    }

    // Exhaust source by island tile ID event

    public void RaiseExhaustSourceByIslandTileIdEvent(int islandTileId, Source source, bool isExhausted)
    {
        exhaustSourceByIslandTileIdEvent.Invoke(islandTileId, source, isExhausted);
    }
    public void AddListenerToExhaustSourceByIslandTileIdEvent(UnityAction<int, Source, bool> listener)
    {
        exhaustSourceByIslandTileIdEvent.AddListener(listener);
    }

    // Destroy island tile token even

    public void RaiseDestroyIslandTileTokenEvent(int islandTileId, TokenType tokenType)
    {
        destroyIslandTileTokenEvent.Invoke(islandTileId, tokenType);
    }
    public void AddListenerToDestroyIslandTileTokenEvent(UnityAction<int, TokenType> listener)
    {
        destroyIslandTileTokenEvent.AddListener(listener);
    }

    // Shelter is built event

    public void RaiseShelterIsBuiltEvent()
    {
        shelterIsBuiltEvent.Invoke();
    }
    public void AddListenerToShelterIsBuiltEvent(UnityAction listener)
    {
        shelterIsBuiltEvent.AddListener(listener);
    }

    // Shelter is built response event

    public void RaiseShelterIsBuiltResponseEvent(bool shelterIsBuilt)
    {
        shelterIsBuiltResponseEvent.Invoke(shelterIsBuilt);
    }
    public void AddListenerToShelterIsBuiltResponseEvent(UnityAction<bool> listener)
    {
        shelterIsBuiltResponseEvent.AddListener(listener);
    }

    // EndPhaseEvent

    public void RaiseEndPhaseEvent(Phase phase)
    {
        endPhaseEvent.Invoke(phase);
    }
    public void AddListenerToEndPhaseEvent(UnityAction<Phase> listener)
    {
        endPhaseEvent.AddListener(listener);
    }

    // PhaseStartEvent

    public void RaisePhaseStartEvent(Phase phaseStarted)
    {
        phaseStartEvent.Invoke(phaseStarted);
    }
    public void AddListenerToPhaseStartEvent(UnityAction<Phase> listener)
    {
        phaseStartEvent.AddListener(listener);
    }

    // TurnStartEvent

    public void RaiseTurnStartEvent(int turnStarted)
    {
        turnStartEvent.Invoke(turnStarted);
    }
    public void AddListenerToTurnStartEvent(UnityAction<int> listener)
    {
        turnStartEvent.AddListener(listener);
    }

    // AnimationInProgressEvent

    public void RaiseAnimationInProgressEvent(bool isInProgress)
    {
        animationInProgressEvent.Invoke(isInProgress);
    }
    public void AddListenerToAnimationInProgressEvent(UnityAction<bool> listener)
    {
        animationInProgressEvent.AddListener(listener);
    }

    // Get phase event

    public void RaiseGetPhaseEvent()
    {
        getPhaseEvent.Invoke();
    }
    public void AddListenerToGetPhaseEvent(UnityAction listener)
    {
        getPhaseEvent.AddListener(listener);
    }

    // Get phase response event

    public void RaiseGetPhaseResponseEvent(Phase currentPhase)
    {
        getPhaseResponseEvent.Invoke(currentPhase);
    }
    public void AddListenerToGetPhaseResponseEvent(UnityAction<Phase> listener)
    {
        getPhaseResponseEvent.AddListener(listener);
    }

    // CardSpawnedEvent

    public void RaiseCardSpawnedEvent(Deck deck)
    {
        cardSpawnedEvent.Invoke(deck);
    }
    public void AddListenerToCardSpawnedEvent(UnityAction<Deck> listener)
    {
        cardSpawnedEvent.AddListener(listener);
    }

    // CardDrawnEvent

    public void RaiseCardDrawnEvent(Deck deckDrawnFrom, int componentIdOfDrawnCard)
    {
        cardDrawnEvent.Invoke(deckDrawnFrom, componentIdOfDrawnCard);
    }
    public void AddListenerToCardDrawnEvent(UnityAction<Deck, int> listener)
    {
        cardDrawnEvent.AddListener(listener);
    }

    // CardRevealedEvent

    public void RaiseCardRevealedEvent(Deck deckDrawnFrom, Card cardRevealed, int componentIdOfRevealedCard)
    {
        cardRevealedEvent.Invoke(deckDrawnFrom, cardRevealed, componentIdOfRevealedCard);
    }
    public void AddListenerToCardRevealedEvent(UnityAction<Deck, Card, int> listener)
    {
        cardRevealedEvent.AddListener(listener);
    }

    // DrawCardEvent

    public void RaiseDrawCardEvent(Deck deck)
    {
        drawCardEvent.Invoke(deck);
    }
    public void AddListenerToDrawCardEvent(UnityAction<Deck> listener)
    {
        drawCardEvent.AddListener(listener);
    }







    // EnableMainUIEvent

    public void RaiseDisableMainUIEvent()
    {
        enableMainUIEvent.Invoke(false);
    }
    public void RaiseEnableMainUIEvent()
    {
        enableMainUIEvent.Invoke(true);
    }
    public void AddListenerToEnableMainUIEvent(UnityAction<bool> listener)
    {
        enableMainUIEvent.AddListener(listener);
    }

    // InitializeCardPopupEvent

    public void RaiseInitializeCardPopupEvent(int componentIdOfPopup, int componentIdOfCardController, Deck deckDrawnFrom, Card cardToDisplay)
    {
        initializeCardPopupEvent.Invoke(componentIdOfPopup, componentIdOfCardController, deckDrawnFrom, cardToDisplay);
    }
    public void AddListenerToInitializeCardPopupEvent(UnityAction<int, int, Deck, Card> listener)
    {
        initializeCardPopupEvent.AddListener(listener);
    }

    // CardPopupClosedEvent

    public void RaiseEventCardPopupClosedEvent(int componentIdOfCardController)
    {
        cardPopupClosedEvent.Invoke(componentIdOfCardController);
    }
    public void AddListenerToEventCardPopupClosedEvent(UnityAction<int> listener)
    {
        cardPopupClosedEvent.AddListener(listener);
    }

    // InitializeSpritePopupEvent

    public void RaiseInitializeSpritePopupEvent(int popupComponentId, int sourceComponentId, Sprite sprite)
    {
        initializeSpritePopupEvent.Invoke(popupComponentId, sourceComponentId, sprite);
    }
    public void AddListenerToInitializeSpritePopupEvent(UnityAction<int, int, Sprite> listener)
    {
        initializeSpritePopupEvent.AddListener(listener);
    }

    // IslandTilePopupClosedEvent

    public void RaiseSpritePopupClosedEvent(int sourceComponentId)
    {
        spritePopupClosedEvent.Invoke(sourceComponentId);
    }
    public void AddListenerToSpritePopupClosedEvent(UnityAction<int> listener)
    {
        spritePopupClosedEvent.AddListener(listener);
    }

    // SpawnMoraleChoicePopupEvent

    public void RaiseSpawnMoraleChoicePopupEvent()
    {
        spawnMoraleChoicePopupEvent.Invoke();
    }
    public void AddListenerToSpawnMoraleChoicePopupEvent(UnityAction listener)
    {
        spawnMoraleChoicePopupEvent.AddListener(listener);
    }

    // SpawnDiscoveryTokenActivationPopupEvent

    public void RaiseSpawnDiscoveryTokenActivationPopupEvent(int sourceComponentId, DiscoveryToken discoveryToken)
    {
        spawnDiscoveryTokenActivationPopupEvent.Invoke(sourceComponentId, discoveryToken);
    }
    public void AddListenerToSpawnDiscoveryTokenActivationPopupEvent(UnityAction<int, DiscoveryToken> listener)
    {
        spawnDiscoveryTokenActivationPopupEvent.AddListener(listener);
    }

    // InitializeDiscoveryTokenActivationPopupEvent

    public void RaiseInitializeDiscoveryTokenActivationPopupEvent(int sourceComponentId, DiscoveryToken discoveryToken)
    {
        initializeDiscoveryTokenActivationPopupEvent.Invoke(sourceComponentId, discoveryToken);
    }
    public void AddListenerToInitializeDiscoveryTokenActivationPopupEvent(UnityAction<int, DiscoveryToken> listener)
    {
        initializeDiscoveryTokenActivationPopupEvent.AddListener(listener);
    }

    // Spawn variable cost popup event

    public void RaiseSpawnVariableCostPopupEvent(ResourceCost resourceCost, ActionAssignment actionAssignment)
    {
        spawnVariableCostPopupEvent.Invoke(resourceCost, actionAssignment);
    }
    public void AddListenerToSpawnVariableCostPopupEvent(UnityAction<ResourceCost, ActionAssignment> listener)
    {
        spawnVariableCostPopupEvent.AddListener(listener);
    }

    // Spawn make camp choice popup

    public void RaiseSpawnMakeCampChoicePopupEvent(int playerId)
    {
        spawnMakeCampChoicePopupEvent.Invoke(playerId);
    }
    public void AddListenerToSpawnMakeCampChoicePopupEvent(UnityAction<int> listener)
    {
        spawnMakeCampChoicePopupEvent.AddListener(listener);
    }

    // Spawn night phase popup

    public void RaiseSpawnNightPhasePopupEvent(int totalFoodAvailable)
    {
        spawnNightPhasePopupEvent.Invoke(totalFoodAvailable);
    }
    public void AddListenerToSpawnNightPhasePopupEvent(UnityAction<int> listener)
    {
        spawnNightPhasePopupEvent.AddListener(listener);
    }

    // Spawn ability popup event

    public void RaiseSpawnAbilityPopupEvent(int playerId, Ability ability)
    {
        spawnAbilityPopupEvent.Invoke(playerId, ability);
    }
    public void AddListenerToSpawnAbilityPopupEvent(UnityAction<int, Ability> listener)
    {
        spawnAbilityPopupEvent.AddListener(listener);
    }

    // Spawn choose invention card popup event

    public void RaiseSpawnChooseInventionCardPopupEvent(List<InventionCard> inventionCards)
    {
        spawnChooseInventionCardPopupEvent.Invoke(inventionCards);
    }
    public void AddListenerToSpawnChooseInventionCardPopupEvent(UnityAction<List<InventionCard>> listener)
    {
        spawnChooseInventionCardPopupEvent.AddListener(listener);
    }

    // Spawn reconnaissance popup event

    public void RaiseSpawnReconnaissancePopupEvent(List<IslandTileController> topTiles, Stack<IslandTileController> deck)
    {
        spawnReconnaissancePopupEvent.Invoke(topTiles, deck);
    }
    public void AddListenerToSpawnReconnaissancePopupEvent(UnityAction<List<IslandTileController>, Stack<IslandTileController>> listener)
    {
        spawnReconnaissancePopupEvent.AddListener(listener);
    }

    // Spawn scouting popup event

    public void RaiseSpawnScoutingPopupEvent(DiscoveryTokenController token1, DiscoveryTokenController token2)
    {
        spawnScoutingPopupEvent.Invoke(token1, token2);
    }
    public void AddListenerToSpawnScoutingPopupEvent(UnityAction<DiscoveryTokenController, DiscoveryTokenController> listener)
    {
        spawnScoutingPopupEvent.AddListener(listener);
    }

    // Spawn tracking popup event

    public void RaiseSpawnTrackingPopupEvent(BeastCardController topCard, Stack<BeastCardController> huntingDeck)
    {
        spawnTrackingPopupEvent.Invoke(topCard, huntingDeck);
    }
    public void AddListenerToSpawnTrackingPopupEvent(UnityAction<BeastCardController, Stack<BeastCardController>> listener)
    {
        spawnTrackingPopupEvent.AddListener(listener);
    }

    // Spawn item activation popup event

    public void RaiseSpawnItemActivationPopupEvent(Invention invention)
    {
        spawnItemActivationPopupEvent.Invoke(invention);
    }
    public void AddListenerToSpawnItemActivationPopupEvent(UnityAction<Invention> listener)
    {
        spawnItemActivationPopupEvent.AddListener(listener);
    }

    // Spawn item activation popup event

    public void RaiseSpawnShortcutPopupEvent()
    {
        spawnShortcutPopupEvent.Invoke();
    }
    public void AddListenerToSpawnShortcutPopupEvent(UnityAction listener)
    {
        spawnShortcutPopupEvent.AddListener(listener);
    }


    // Choose adjacent camp event

    public void RaiseGetIslandTileInputEvent(bool isActive, InputType inputType)
    {
        getIslandTileInputEvent.Invoke(isActive, inputType);
    }
    public void AddListenerToGetIslandTileInputEvent(UnityAction<bool, InputType> listener)
    {
        getIslandTileInputEvent.AddListener(listener);
    }

    // Adjacent tile chosen event

    public void RaiseAdjacentTileChosenEvent(bool campIsMoving, int locationId)
    {
        adjacentTileChosenEvent.Invoke(campIsMoving, locationId);
    }
    public void AddListenerToAdjacentTileChosenEvent(UnityAction<bool, int> listener)
    {
        adjacentTileChosenEvent.AddListener(listener);
    }

    // Spawn Build Dice Popup

    public void RaiseSpawnDicePopupEvent(List<DieType> dieTypes, int playerId, bool hasRerollAvailable)
    {
        spawnDicePopupEvent.Invoke(dieTypes, playerId, hasRerollAvailable);
    }
    public void RaiseSpawnDicePopupEvent(List<DieType> dieTypes)
    {
        spawnDicePopupEvent.Invoke(dieTypes, -1, false);
    }
    public void AddListenerToSpawnDicePopupEvent(UnityAction<List<DieType>, int, bool> listener)
    {
        spawnDicePopupEvent.AddListener(listener);
    }

    // Dice rolled event

    public void RaiseDieRolledEvent(DieType dieType, int faceRolled)
    {
        dieRolledEvent.Invoke(dieType, faceRolled);
    }
    public void AddListenerToDieRolledEvent(UnityAction<DieType, int> listener)
    {
        dieRolledEvent.AddListener(listener);
    }

    // Draw adventure card event

    public void RaiseDrawAdventureCardEvent(AdventureType adventureType)
    {
        drawAdventureCardEvent.Invoke(adventureType);
    }
    public void AddListenerToDrawAdventureCardEvent(UnityAction<AdventureType> listener)
    {
        drawAdventureCardEvent.AddListener(listener);
    }

    // Adventure card popup closed

    public void RaiseAdventureCardPopupClosedEvent(int componentId, AdventureCard adventureCard)
    {
        adventureCardPopupClosedEvent.Invoke(componentId, adventureCard, 0);
    }
    public void RaiseAdventureCardPopupClosedEvent(int componentId, AdventureCard adventureCard, int optionChosen)
    {
        adventureCardPopupClosedEvent.Invoke(componentId, adventureCard, optionChosen);
    }
    public void AddListenerToAdventureCardPopupClosedEvent(UnityAction<int, AdventureCard, int> listener)
    {
        adventureCardPopupClosedEvent.AddListener(listener);
    }

    // Shuffle into event deck

    public void RaiseShuffleIntoEventDeckEvent(CardController cardController)
    {
        shuffleIntoEventDeckEvent.Invoke(cardController);
    }
    public void AddListenerToShuffleIntoEventDeckEvent(UnityAction<CardController> listener)
    {
        shuffleIntoEventDeckEvent.AddListener(listener);
    }

    // Ability activated

    public void RaiseAbilityActivatedEvent(int playerId, Ability ability)
    {
        abilityActivatedEvent.Invoke(playerId, ability);
    }
    public void AddListenerToAbilityActivatedEvent(UnityAction<int, Ability> listener)
    {
        abilityActivatedEvent.AddListener(listener);
    }

    // Economical construction

    public void RaiseEconomicalConstructionEvent(int playerId)
    {
        economicalConstructionEvent.Invoke(playerId);
    }
    public void AddListenerToEconomicalConstructionEvent(UnityAction<int> listener)
    {
        economicalConstructionEvent.AddListener(listener);
    }

    // Spawn single use pawn

    public void RaiseSpawnSingleUsePawnEvent(int playerId, PawnType pawnType)
    {
        spawnSingleUsePawnEvent.Invoke(playerId, pawnType);
    }
    public void AddListenerToSpawnSingleUsePawnEvent(UnityAction<int, PawnType> listener)
    {
        spawnSingleUsePawnEvent.AddListener(listener);
    }

    // Reconnaissance event

    public void RaiseReconnaissanceEvent()
    {
        reconnaissanceEvent.Invoke();
    }
    public void AddListenerToReconnaissanceEvent(UnityAction listener)
    {
        reconnaissanceEvent.AddListener(listener);
    }

    // Scouting event

    public void RaiseScoutingEvent()
    {
        scoutingEvent.Invoke();
    }
    public void AddListenerToScoutingEvent(UnityAction listener)
    {
        scoutingEvent.AddListener(listener);
    }

    // Tracking event

    public void RaiseTrackingEvent()
    {
        trackingEvent.Invoke();
    }
    public void AddListenerToTrackingEvent(UnityAction listener)
    {
        trackingEvent.AddListener(listener);
    }


    // Cancel rain cloud

    public void RaiseCancelRainCloudEvent()
    {
        cancelRainCloudEvent.Invoke();
    }
    public void AddListenerToCancelRainCloudEvent(UnityAction listener)
    {
        cancelRainCloudEvent.AddListener(listener);
    }

    // Conert snow to rain

    public void RaiseConvertSnowToRainEvent()
    {
        convertSnowToRainEvent.Invoke();
    }
    public void AddListenerToConvertSnowToRainEvent(UnityAction listener)
    {
        convertSnowToRainEvent.AddListener(listener);
    }

}
