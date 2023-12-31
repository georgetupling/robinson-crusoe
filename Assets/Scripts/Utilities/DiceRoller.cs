using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class raises an event to spawn the dice popup and saves information relating to the roll.
    It then listens for dice rolled events and actions them.
    The ActionResolver additionally listens for whether or not build/gather/explore rolls were successful.
*/

public class DiceRoller : MonoBehaviour
{
    public static DiceRoller Singleton;

    // Information about the current roll

    int playerId;
    int islandTileId;
    int locationId;
    Source sourceGatheredFrom;

    // Dice Lists

    List<DieType> buildDice = new List<DieType> { DieType.BuildSuccess, DieType.BuildAdventure, DieType.BuildDamage };
    List<DieType> gatherDice = new List<DieType> { DieType.GatherSuccess, DieType.GatherAdventure, DieType.GatherDamage };
    List<DieType> exploreDice = new List<DieType> { DieType.ExploreSuccess, DieType.ExploreAdventure, DieType.ExploreDamage };

    // Transforms for checking tokens

    [SerializeField] Transform buildAdventureDeckTokenArea;
    [SerializeField] Transform gatherAdventureDeckTokenArea;
    [SerializeField] Transform exploreAdventureDeckTokenArea;

    // Player determination by playerId

    Dictionary<int, int> playerDetermination = new Dictionary<int, int>();

    // Fields for querying the first player ID

    bool waitingOnFirstPlayer;
    int firstPlayerId;

    // Specific flags
    bool pitIsBuilt;
    bool isProductionPhase;

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogError("Scene contains duplicate DiceRoller.");
        }
        EventGenerator.Singleton.AddListenerToDieRolledEvent(OnDieRolledEvent);
        EventGenerator.Singleton.AddListenerToAdventureCardPopupClosedEvent(OnAdventureCardPopupClosedEvent);
        EventGenerator.Singleton.AddListenerToGetDeterminationResponseEvent(OnGetDeterminationResponseEvent);
        EventGenerator.Singleton.AddListenerToGetFirstPlayerEvent(OnGetFirstPlayerEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
    }

    // Listeners

    void OnDieRolledEvent(DieType dieType, int faceRolled)
    {
        // Checks for pit roll during production phase
        if (isProductionPhase && pitIsBuilt && dieType == DieType.BuildDamage)
        {
            if (faceRolled < 4)
            {
                EventGenerator.Singleton.RaiseGainFoodEvent(2);
            }
            return;
        }

        switch (dieType)
        {
            case DieType.BuildSuccess:
                if (faceRolled < 2)
                {
                    EventGenerator.Singleton.RaiseGainDeterminationEvent(playerId, 2);
                }
                break;
            case DieType.BuildAdventure:
                if (AdventureTokenRemoved(buildAdventureDeckTokenArea) || faceRolled < 3)
                {
                    EventGenerator.Singleton.RaiseDrawAdventureCardEvent(AdventureType.Build);
                }
                break;
            case DieType.BuildDamage:
                if (faceRolled < 4)
                {
                    EventGenerator.Singleton.RaiseLoseHealthEvent(playerId, 1);
                }
                break;
            case DieType.GatherSuccess:
                if (faceRolled < 1)
                {
                    EventGenerator.Singleton.RaiseGainDeterminationEvent(playerId, 2);
                }
                break;
            case DieType.GatherAdventure:
                if (AdventureTokenRemoved(gatherAdventureDeckTokenArea) || faceRolled < 3)
                {
                    EventGenerator.Singleton.RaiseDrawAdventureCardEvent(AdventureType.Gather);
                }
                break;
            case DieType.GatherDamage:
                if (faceRolled < 1)
                {
                    EventGenerator.Singleton.RaiseLoseHealthEvent(playerId, 1);
                }
                break;
            case DieType.ExploreSuccess:
                if (faceRolled < 1)
                {
                    EventGenerator.Singleton.RaiseGainDeterminationEvent(playerId, 2);
                }
                break;
            case DieType.ExploreAdventure:
                if (AdventureTokenRemoved(exploreAdventureDeckTokenArea) || faceRolled < 5)
                {
                    EventGenerator.Singleton.RaiseDrawAdventureCardEvent(AdventureType.Explore);
                }
                break;
            case DieType.ExploreDamage:
                if (faceRolled < 3)
                {
                    EventGenerator.Singleton.RaiseLoseHealthEvent(playerId, 1);
                }
                break;
        }
    }

    void OnAdventureCardPopupClosedEvent(int componentId, AdventureCard adventureCard, int optionChosen)
    {
        // Actions the drawn adventure card
        if (playerId == 6)
        {
            waitingOnFirstPlayer = true;
            EventGenerator.Singleton.RaiseGetFirstPlayerEvent();
            while (waitingOnFirstPlayer)
            {
                // Do nothing
            }
            playerId = firstPlayerId;
        }
        foreach (CardEffect cardEffect in adventureCard.adventureEffects)
        {
            // Sets the option chosen and the target of the effect
            if (adventureCard.adventureHasDecision)
            {
                cardEffect.SetOptionChosen(optionChosen);
            }
            if (cardEffect.targetType == TargetType.Player)
            {
                cardEffect.SetTarget(playerId);
            }
            else if (cardEffect.targetType == TargetType.IslandTile)
            {
                cardEffect.SetTarget(islandTileId);
            }
            else if (cardEffect.targetType == TargetType.Location)
            {
                cardEffect.SetTarget(locationId);
            }
            cardEffect.SetSourceGatheredFrom(sourceGatheredFrom);
            cardEffect.ApplyEffect();
        }
    }

    void OnGetDeterminationResponseEvent(int playerId, int determination)
    {
        if (!playerDetermination.ContainsKey(playerId))
        {
            playerDetermination.Add(playerId, determination);
        }
        else
        {
            playerDetermination[playerId] = determination;
        }
    }

    void OnGetFirstPlayerEvent(string eventType, int firstPlayerId)
    {
        if (waitingOnFirstPlayer)
        {
            this.firstPlayerId = firstPlayerId;
            waitingOnFirstPlayer = false;
        }
    }

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt)
    {
        if (invention == Invention.Pit)
        {
            pitIsBuilt = isBuilt;
        }
    }

    void OnPhaseStartEvent(Phase phaseStarted)
    {
        isProductionPhase = phaseStarted == Phase.Production;
    }

    // Public methods

    public void RollBuildDice(int playerId)
    {
        this.playerId = playerId;
        // Checks whether the player has a reroll available
        bool hasRerollAvailable = false;
        if (playerId < GameSettings.PlayerCount && GameSettings.PlayerCharacters[playerId] == CharacterType.Carpenter)
        {
            EventGenerator.Singleton.RaiseGetDeterminationEvent(playerId);
            int costOfReroll = 2;
            hasRerollAvailable = playerDetermination[playerId] >= costOfReroll;
        }
        EventGenerator.Singleton.RaiseSpawnDicePopupEvent(buildDice, playerId, hasRerollAvailable);
    }

    public void RollGatherDice(int playerId, int islandTileId, Source sourceGatheredFrom)
    {
        this.playerId = playerId;
        this.islandTileId = islandTileId;
        this.sourceGatheredFrom = sourceGatheredFrom;
        // Checks whether the player has a reroll available
        bool hasRerollAvailable = false;
        if (playerId < GameSettings.PlayerCount && GameSettings.PlayerCharacters[playerId] == CharacterType.Cook)
        {
            EventGenerator.Singleton.RaiseGetDeterminationEvent(playerId);
            int costOfReroll = 2;
            hasRerollAvailable = playerDetermination[playerId] >= costOfReroll;
        }
        EventGenerator.Singleton.RaiseSpawnDicePopupEvent(gatherDice, playerId, hasRerollAvailable);
    }

    public void RollExploreDice(int playerId, int locationId)
    {
        this.playerId = playerId;
        this.locationId = locationId;
        // Checks whether the player has a reroll available
        bool hasRerollAvailable = false;
        if (playerId < GameSettings.PlayerCount && GameSettings.PlayerCharacters[playerId] == CharacterType.Explorer)
        {
            EventGenerator.Singleton.RaiseGetDeterminationEvent(playerId);
            int costOfReroll = 2;
            hasRerollAvailable = playerDetermination[playerId] >= costOfReroll;
        }
        EventGenerator.Singleton.RaiseSpawnDicePopupEvent(exploreDice, playerId, hasRerollAvailable);
    }

    // Helper method that checks whether a particular adventure deck has an adventure token on it
    // If it finds one, it destroys it

    bool AdventureTokenRemoved(Transform tokenArea)
    {
        for (int i = 0; i < tokenArea.childCount; i++)
        {
            Transform childTransform = tokenArea.GetChild(i);
            for (int j = 0; j < childTransform.childCount; j++)
            {
                Transform grandchildTransform = childTransform.GetChild(j);
                TokenController token = grandchildTransform.GetComponent<TokenController>();
                if (token != null && (token.tokenType == TokenType.BuildAdventure || token.tokenType == TokenType.GatherAdventure || token.tokenType == TokenType.ExploreAdventure))
                {
                    EventGenerator.Singleton.RaiseDestroyComponentEvent(token.ComponentId);
                    return true;
                }
            }
        }
        return false;
    }
}
