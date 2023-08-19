using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionResolver : MonoBehaviour
{
    static ActionResolver singleton;

    public List<ActionAssignment> actionAssignments;
    [SerializeField] Transform popupsArea;
    [SerializeField] Transform gatherTokenArea;
    [SerializeField] Transform exploreTokenArea;
    [SerializeField] Transform buildTokenArea;

    const float delayBetweenActions = 0.75f;
    private int animationsInProgress = 0;
    private bool gettingIslandTileInput;

    // Verbiage for querying information from other classes

    private bool isWaitingForHideAvailable = false;
    private bool isWaitingForWoodAvailable = false;
    private int hideAvailable = 0;
    private int woodAvailable = 0;
    private List<Invention> builtInventions = new List<Invention>();
    private Dictionary<int, int> playerDetermination = new Dictionary<int, int>(); // Maps playerId to determination

    bool rolledSuccess = false;

    // Flags for applying specific effects

    int economicalConstructionPlayerId = -1;
    bool economicalConstructionApplied = false;

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
        EventGenerator.Singleton.AddListenerToActionsSubmittedEvent(OnActionsSubmitted);
        EventGenerator.Singleton.AddListenerToGetResourceEvent(OnGetResourceEvent);
        EventGenerator.Singleton.AddListenerToAnimationInProgressEvent(OnAnimationInProgressEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
        EventGenerator.Singleton.AddListenerToDieRolledEvent(OnDieRolledEvent);
        EventGenerator.Singleton.AddListenerToGetDeterminationResponseEvent(OnGetDeterminationResponseEvent);
        EventGenerator.Singleton.AddListenerToEconomicalConstructionEvent(OnEconomicalConstructionEvent);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
        EventGenerator.Singleton.AddListenerToGetIslandTileInputEvent(OnGetIslandTileInputEvent);
    }

    // Listeners

    void OnActionsSubmitted(List<ActionAssignment> actionAssignments)
    {
        this.actionAssignments = actionAssignments;
        StartCoroutine(ResolveThreatActions());
    }

    void OnGetResourceEvent(string eventType, int amount)
    {
        if (eventType == GetResourceEvent.GetHideResponse)
        {
            hideAvailable = amount;
            isWaitingForHideAvailable = false;
        }
        else if (eventType == GetResourceEvent.GetWoodResponse)
        {
            woodAvailable = amount;
            isWaitingForWoodAvailable = false;
        }
    }

    void OnAnimationInProgressEvent(bool isInProgress)
    {
        if (isInProgress)
        {
            animationsInProgress++;
        }
        else
        {
            animationsInProgress--;
        }
    }

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt)
    {
        if (isBuilt)
        {
            builtInventions.Add(invention);
        }
        else
        {
            builtInventions.Remove(invention);
        }
    }

    void OnDieRolledEvent(DieType dieType, int faceRolled)
    {
        if (dieType == DieType.BuildSuccess)
        {
            rolledSuccess = faceRolled > 1;
        }
        else if (dieType == DieType.GatherSuccess || dieType == DieType.ExploreSuccess)
        {
            rolledSuccess = faceRolled != 0;
        }
    }

    void OnGetDeterminationResponseEvent(int playerId, int determination)
    {
        if (playerDetermination.ContainsKey(playerId))
        {
            playerDetermination[playerId] = determination;
        }
        else
        {
            playerDetermination.Add(playerId, determination);
        }
    }

    void OnEconomicalConstructionEvent(int playerId)
    {
        economicalConstructionPlayerId = playerId;
    }

    void OnTurnStartEvent(int turnStarted)
    {
        economicalConstructionPlayerId = -1;
        economicalConstructionApplied = false;
    }

    void OnGetIslandTileInputEvent(bool isActive, InputType inputType)
    {
        gettingIslandTileInput = isActive;
    }

    // Methods for resolving actions

    IEnumerator ResolveThreatActions()
    {
        foreach (ActionAssignment actionAssignment in actionAssignments)
        {
            if (actionAssignment.Type != ActionType.Threat)
            {
                continue;
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            foreach (int pawnComponentId in actionAssignment.pawnComponentIds)
            {
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(pawnComponentId);
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            EventCard eventCard = actionAssignment.eventCard;
            bool costsPaid = PayCosts(eventCard.threatResourceCosts, actionAssignment);
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            if (costsPaid)
            {
                List<CardEffect> successEffects = actionAssignment.isTwoActionThreat ? eventCard.successEffects2Action : eventCard.successEffects1Action;
                foreach (CardEffect successEffect in successEffects)
                {
                    if (successEffect.targetType == TargetType.Player)
                    {
                        successEffect.SetTarget(actionAssignment.playerIds[0]);
                    }
                    successEffect.ApplyEffect();
                }
                EventGenerator.Singleton.RaiseDestroyComponentEvent(actionAssignment.eventCardControllerComponentId);
            }
            yield return new WaitForSeconds(delayBetweenActions);
        }
        StartCoroutine(ResolveHuntingActions());
    }

    IEnumerator ResolveHuntingActions()
    {
        foreach (ActionAssignment actionAssignment in actionAssignments)
        {
            if (actionAssignment.Type != ActionType.Hunting)
            {
                continue;
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            foreach (int pawnComponentId in actionAssignment.pawnComponentIds)
            {
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(pawnComponentId);
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            // The only time the ID of the second pawn is used is when the first one is a neutral pawn (e.g. the Map pawn)
            // These pawns have playerId 6 since they're "owned" by the first player
            if (actionAssignment.playerIds[0] == 6 && actionAssignment.playerIds.Count > 1)
            {
                EventGenerator.Singleton.RaiseResolveHuntingActionEvent(actionAssignment.playerIds[1]);
            }
            else
            {
                EventGenerator.Singleton.RaiseResolveHuntingActionEvent(actionAssignment.playerIds[0]);
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            yield return new WaitForSeconds(delayBetweenActions);
        }
        StartCoroutine(ResolveBuildActions());
    }

    IEnumerator ResolveBuildActions()
    {
        foreach (ActionAssignment actionAssignment in actionAssignments)
        {
            if (
                actionAssignment.Type != ActionType.BuildShelter &&
                actionAssignment.Type != ActionType.BuildRoof &&
                actionAssignment.Type != ActionType.BuildPalisade &&
                actionAssignment.Type != ActionType.BuildWeapon &&
                actionAssignment.Type != ActionType.BuildInvention
            )
            {
                continue;
            }
            foreach (int pawnComponentId in actionAssignment.pawnComponentIds)
            {
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(pawnComponentId);
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            // Checks whether the player still has enough determination to build their signature invention
            // This is necessary because the player could spend determination between assigning actions and this action resolving
            if (actionAssignment.Type == ActionType.BuildInvention && actionAssignment.inventionCard.isPersonalInvention)
            {
                EventGenerator.Singleton.RaiseGetDeterminationEvent(actionAssignment.playerIds[0]);
                int determinationCost = 2;
                if (playerDetermination[actionAssignment.playerIds[0]] < determinationCost)
                {
                    Debug.LogError("Unable to pay determination cost for personal invention.");
                    yield return new WaitForSeconds(delayBetweenActions);
                    continue;
                }
            }
            bool costsPaid = PayCosts(actionAssignment.resourceCosts, actionAssignment);
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            if (costsPaid)
            {
                bool buildIsSuccessful = true;
                if (actionAssignment.mustRoll)
                {
                    // The only time the ID of the second pawn is used is when the first one is a neutral pawn (e.g. the Map pawn)
                    // These pawns have playerId 6 since they're "owned" by the first player
                    if (actionAssignment.playerIds[0] == 6 && actionAssignment.playerIds.Count > 1)
                    {
                        DiceRoller.Singleton.RollBuildDice(actionAssignment.playerIds[1]);
                    }
                    else
                    {
                        DiceRoller.Singleton.RollBuildDice(actionAssignment.playerIds[0]);
                    }
                    while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
                    {
                        yield return null;
                    }
                    if (!rolledSuccess)
                    {
                        buildIsSuccessful = false;
                    }
                }
                else
                {
                    // Check for an adventure token on the build deck
                    if (AdventureTokenRemoved(buildTokenArea))
                    {
                        EventGenerator.Singleton.RaiseDrawAdventureCardEvent(AdventureType.Build);
                    }
                    while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
                    {
                        yield return null;
                    }
                }
                if (buildIsSuccessful)
                {
                    switch (actionAssignment.Type)
                    {
                        case ActionType.BuildPalisade:
                            EventGenerator.Singleton.RaiseGainPalisadeEvent(1);
                            break;
                        case ActionType.BuildRoof:
                            EventGenerator.Singleton.RaiseGainRoofEvent(1);
                            break;
                        case ActionType.BuildShelter:
                            EventGenerator.Singleton.RaiseGainShelterEvent();
                            break;
                        case ActionType.BuildInvention:
                            EventGenerator.Singleton.RaiseBuildInventionSuccessEvent(actionAssignment.inventionCard.invention);
                            break;
                        case ActionType.BuildWeapon:
                            EventGenerator.Singleton.RaiseGainWeaponEvent(1);
                            break;
                    }
                }
            }
            else
            {
                Debug.LogError("Unable to pay build costs.");
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            yield return new WaitForSeconds(delayBetweenActions);
        }
        StartCoroutine(ResolveGatherActions());
    }

    IEnumerator ResolveGatherActions()
    {
        foreach (ActionAssignment actionAssignment in actionAssignments)
        {
            if (actionAssignment.Type != ActionType.Gather)
            {
                continue;
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            foreach (int pawnComponentId in actionAssignment.pawnComponentIds)
            {
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(pawnComponentId);
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            bool gatherIsSuccessful = true;
            if (actionAssignment.mustRoll)
            {
                // The only time the ID of the second pawn is used is when the first one is a neutral pawn (e.g. the Map pawn)
                // These pawns have playerId 6 since they're "owned" by the first player
                if (actionAssignment.playerIds[0] == 6 && actionAssignment.playerIds.Count > 1)
                {
                    DiceRoller.Singleton.RollGatherDice(actionAssignment.playerIds[1], actionAssignment.islandTile.Id);
                }
                else
                {
                    DiceRoller.Singleton.RollGatherDice(actionAssignment.playerIds[0], actionAssignment.islandTile.Id);
                }
                while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
                {
                    yield return null;
                }
                if (!rolledSuccess)
                {
                    gatherIsSuccessful = false;
                }
            }
            else
            {
                // Check for an adventure token on the gather deck
                if (AdventureTokenRemoved(gatherTokenArea))
                {
                    EventGenerator.Singleton.RaiseDrawAdventureCardEvent(AdventureType.Gather);
                }
                while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
                {
                    yield return null;
                }
            }
            if (gatherIsSuccessful)
            {
                EventGenerator.Singleton.RaiseGatherSuccessEvent(actionAssignment.islandTile.Id, actionAssignment.isRightSource);
            }
            yield return new WaitForSeconds(delayBetweenActions);
        }
        StartCoroutine(ResolveExploreActions());
    }

    IEnumerator ResolveExploreActions()
    {
        foreach (ActionAssignment actionAssignment in actionAssignments)
        {
            if (actionAssignment.Type != ActionType.Explore)
            {
                continue;
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            foreach (int pawnComponentId in actionAssignment.pawnComponentIds)
            {
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(pawnComponentId);
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
            {
                yield return null;
            }
            bool exploreIsSuccessful = true;
            if (actionAssignment.mustRoll)
            {
                // The only time the ID of the second pawn is used is when the first one is a neutral pawn (e.g. the Map pawn)
                // These pawns have playerId 6 since they're "owned" by the first player
                if (actionAssignment.playerIds[0] == 6 && actionAssignment.playerIds.Count > 1)
                {
                    DiceRoller.Singleton.RollExploreDice(actionAssignment.playerIds[1], actionAssignment.locationId);
                }
                else
                {
                    DiceRoller.Singleton.RollExploreDice(actionAssignment.playerIds[0], actionAssignment.locationId);
                }
                while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
                {
                    yield return null;
                }
                if (!rolledSuccess)
                {
                    exploreIsSuccessful = false;
                }
            }
            else
            {
                // Check for an adventure token on the build deck
                if (AdventureTokenRemoved(exploreTokenArea))
                {
                    EventGenerator.Singleton.RaiseDrawAdventureCardEvent(AdventureType.Explore);
                }
                while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
                {
                    yield return null;
                }
            }
            if (exploreIsSuccessful)
            {
                EventGenerator.Singleton.RaiseDrawIslandTileEvent(actionAssignment.locationId);
            }
            yield return new WaitForSeconds(delayBetweenActions);
        }
        StartCoroutine(ResolveMakeCampActions());
    }

    IEnumerator ResolveMakeCampActions()
    {
        foreach (ActionAssignment actionAssignment in actionAssignments)
        {
            if (actionAssignment.Type != ActionType.MakeCamp)
            {
                continue;
            }
            // The lists are reversed so that the top action pawn is removed first instead of the bottom
            List<int> reversedPlayerIds = new List<int>(actionAssignment.playerIds);
            reversedPlayerIds.Reverse();
            List<int> reversedPawnComponentIds = new List<int>(actionAssignment.pawnComponentIds);
            reversedPawnComponentIds.Reverse();
            for (int i = 0; i < reversedPlayerIds.Count; i++)
            {
                while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
                {
                    yield return null;
                }
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(reversedPawnComponentIds[i]);
                while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
                {
                    yield return null;
                }
                if (GameSettings.PlayerCount < 4)
                {
                    EventGenerator.Singleton.RaiseGainDeterminationEvent(reversedPlayerIds[i], 2);
                    EventGenerator.Singleton.RaiseGainMoraleEvent(1);
                }
                else
                {
                    EventGenerator.Singleton.RaiseSpawnMakeCampChoicePopupEvent(reversedPlayerIds[i]);
                }
                yield return new WaitForSeconds(delayBetweenActions / 2f);
            }
        }
        StartCoroutine(ResolveRestActions());
    }

    IEnumerator ResolveRestActions()
    {
        foreach (ActionAssignment actionAssignment in actionAssignments)
        {
            if (actionAssignment.Type != ActionType.Rest)
            {
                continue;
            }
            // The lists are reversed so that the top action pawn is removed first instead of the bottom
            List<int> reversedPlayerIds = new List<int>(actionAssignment.playerIds);
            reversedPlayerIds.Reverse();
            List<int> reversedPawnComponentIds = new List<int>(actionAssignment.pawnComponentIds);
            reversedPawnComponentIds.Reverse();
            for (int i = 0; i < reversedPlayerIds.Count; i++)
            {
                while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
                {
                    yield return null;
                }
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(reversedPawnComponentIds[i]);
                while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
                {
                    yield return null;
                }
                if (builtInventions.Contains(Invention.Bed))
                {
                    EventGenerator.Singleton.RaiseGainDeterminationEvent(reversedPlayerIds[i], 1);
                    EventGenerator.Singleton.RaiseGainHealthEvent(reversedPlayerIds[i], 2);
                }
                else
                {
                    EventGenerator.Singleton.RaiseGainHealthEvent(reversedPlayerIds[i], 1);
                }
                yield return new WaitForSeconds(delayBetweenActions / 2f);
            }
        }
        while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
        {
            yield return null;
        }
        EventGenerator.Singleton.RaiseMakeResourcesAvailableEvent();
        while (popupsArea.childCount > 0 || animationsInProgress > 0 || gettingIslandTileInput)
        {
            yield return null;
        }
        EventGenerator.Singleton.RaiseEndPhaseEvent(Phase.Action);
    }

    // Helper methods

    bool PayCosts(List<ResourceCost> resourceCosts, ActionAssignment actionAssignment)
    {
        // This loop checks if economical construction is active and if it is reduces a wood cost by 1
        // The (potentially) modified values are stored in a new list to avoid changing the original list as we iterate over it
        List<ResourceCost> modifiedResourceCosts = new List<ResourceCost>();
        if (economicalConstructionPlayerId == actionAssignment.playerIds[0] && !economicalConstructionApplied)
        {
            foreach (ResourceCost resourceCost in resourceCosts)
            {
                ResourceCost modifiedCost = resourceCost;
                switch (resourceCost)
                {
                    case ResourceCost.Wood: modifiedCost = ResourceCost.ReducedToZero; economicalConstructionApplied = true; break;
                    case ResourceCost.TwoWood: modifiedCost = ResourceCost.Wood; economicalConstructionApplied = true; break;
                    case ResourceCost.ThreeWood: modifiedCost = ResourceCost.TwoWood; economicalConstructionApplied = true; break;
                    case ResourceCost.FourWood: modifiedCost = ResourceCost.ThreeWood; economicalConstructionApplied = true; break;
                    case ResourceCost.TwoWoodOrHide: modifiedCost = ResourceCost.WoodOrHide; economicalConstructionApplied = true; break;
                    case ResourceCost.ThreeWoodOrTwoHide: modifiedCost = ResourceCost.TwoWoodOrTwoHide; economicalConstructionApplied = true; break;
                    case ResourceCost.FourWoodOrThreeHide: modifiedCost = ResourceCost.ThreeWoodOrThreeHide; economicalConstructionApplied = true; break;
                }
                modifiedResourceCosts.Add(modifiedCost);
                if (economicalConstructionApplied)
                {
                    break;
                }
            }
        }
        else
        {
            modifiedResourceCosts = resourceCosts;
        }

        // This code sorts costs into payable costs (i.e. fixed costs) and costs to query (i.e. variable costs that the player can afford to pay more than one way)
        bool canPayWithHide = false;
        bool canPayWithWood = false;
        List<ResourceCost> payableCosts = new List<ResourceCost>();
        List<ResourceCost> costsToQuery = new List<ResourceCost>();
        StartCoroutine(UpdateResources());
        modifiedResourceCosts.Sort(); // The list is sorted so that fixed costs are evaluated before variable costs
        foreach (ResourceCost resourceCost in modifiedResourceCosts)
        {
            switch (resourceCost)
            {
                case ResourceCost.Wood:
                    if (woodAvailable >= 1)
                    {
                        payableCosts.Add(resourceCost);
                        woodAvailable--;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.Hide:
                    if (hideAvailable >= 1)
                    {
                        payableCosts.Add(resourceCost);
                        hideAvailable--;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.TwoWood:
                    if (woodAvailable >= 2)
                    {
                        payableCosts.Add(resourceCost);
                        woodAvailable -= 2;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.TwoHide:
                    if (hideAvailable >= 2)
                    {
                        payableCosts.Add(resourceCost);
                        hideAvailable -= 2;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.ThreeHide:
                    if (hideAvailable >= 3)
                    {
                        payableCosts.Add(resourceCost);
                        hideAvailable -= 3;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.ThreeWood:
                    if (woodAvailable >= 3)
                    {
                        payableCosts.Add(resourceCost);
                        woodAvailable -= 3;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.FourWood:
                    if (woodAvailable >= 4)
                    {
                        payableCosts.Add(resourceCost);
                        woodAvailable -= 4;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.FiveWood:
                    if (woodAvailable >= 5)
                    {
                        payableCosts.Add(resourceCost);
                        woodAvailable -= 5;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.TwoWoodOrHide:
                    canPayWithWood = woodAvailable >= 2;
                    canPayWithHide = hideAvailable >= 1;
                    if (canPayWithWood && canPayWithHide)
                    {
                        costsToQuery.Add(resourceCost);
                    }
                    else if (canPayWithWood)
                    {
                        payableCosts.Add(ResourceCost.TwoWood);
                        woodAvailable -= 2;
                    }
                    else if (canPayWithHide)
                    {
                        payableCosts.Add(ResourceCost.Hide);
                        hideAvailable -= 1;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.ThreeWoodOrTwoHide:
                    canPayWithWood = woodAvailable >= 3;
                    canPayWithHide = hideAvailable >= 2;
                    if (canPayWithWood && canPayWithHide)
                    {
                        costsToQuery.Add(resourceCost);
                    }
                    else if (canPayWithWood)
                    {
                        payableCosts.Add(ResourceCost.ThreeWood);
                        woodAvailable -= 3;
                    }
                    else if (canPayWithHide)
                    {
                        payableCosts.Add(ResourceCost.TwoHide);
                        hideAvailable -= 2;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.FourWoodOrThreeHide:
                    canPayWithWood = woodAvailable >= 4;
                    canPayWithHide = hideAvailable >= 3;
                    if (canPayWithWood && canPayWithHide)
                    {
                        costsToQuery.Add(resourceCost);
                    }
                    else if (canPayWithWood)
                    {
                        payableCosts.Add(ResourceCost.FourWood);
                        woodAvailable -= 4;
                    }
                    else if (canPayWithHide)
                    {
                        payableCosts.Add(ResourceCost.ThreeHide);
                        hideAvailable -= 3;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.WoodOrHide:
                    canPayWithWood = woodAvailable >= 1;
                    canPayWithHide = hideAvailable >= 1;
                    if (canPayWithWood && canPayWithHide)
                    {
                        costsToQuery.Add(resourceCost);
                    }
                    else if (canPayWithWood)
                    {
                        payableCosts.Add(ResourceCost.Wood);
                        woodAvailable -= 1;
                    }
                    else if (canPayWithHide)
                    {
                        payableCosts.Add(ResourceCost.Hide);
                        hideAvailable -= 1;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.TwoWoodOrTwoHide:
                    canPayWithWood = woodAvailable >= 2;
                    canPayWithHide = hideAvailable >= 2;
                    if (canPayWithWood && canPayWithHide)
                    {
                        costsToQuery.Add(resourceCost);
                    }
                    else if (canPayWithWood)
                    {
                        payableCosts.Add(ResourceCost.TwoWood);
                        woodAvailable -= 2;
                    }
                    else if (canPayWithHide)
                    {
                        payableCosts.Add(ResourceCost.TwoHide);
                        hideAvailable -= 2;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.ThreeWoodOrThreeHide:
                    canPayWithWood = woodAvailable >= 3;
                    canPayWithHide = hideAvailable >= 3;
                    if (canPayWithWood && canPayWithHide)
                    {
                        costsToQuery.Add(resourceCost);
                    }
                    else if (canPayWithWood)
                    {
                        payableCosts.Add(ResourceCost.ThreeWood);
                        woodAvailable -= 3;
                    }
                    else if (canPayWithHide)
                    {
                        payableCosts.Add(ResourceCost.ThreeHide);
                        hideAvailable -= 3;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case ResourceCost.WoodUnlessShovel:
                    if (!builtInventions.Contains(Invention.Shovel))
                    {
                        if (woodAvailable >= 1)
                        {
                            payableCosts.Add(ResourceCost.Wood);
                            woodAvailable -= 1;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;
                default:
                    Debug.LogError($"Unable to process cost {resourceCost}.");
                    return false;
            }
        }

        foreach (ResourceCost payableCost in payableCosts)
        {
            switch (payableCost)
            {
                case ResourceCost.Wood:
                    EventGenerator.Singleton.RaiseLoseWoodEvent(1);
                    break;
                case ResourceCost.TwoWood:
                    EventGenerator.Singleton.RaiseLoseWoodEvent(2);
                    break;
                case ResourceCost.ThreeWood:
                    EventGenerator.Singleton.RaiseLoseWoodEvent(3);
                    break;
                case ResourceCost.FourWood:
                    EventGenerator.Singleton.RaiseLoseWoodEvent(4);
                    break;
                case ResourceCost.Hide:
                    EventGenerator.Singleton.RaiseLoseHideEvent(1);
                    break;
                case ResourceCost.TwoHide:
                    EventGenerator.Singleton.RaiseLoseHideEvent(2);
                    break;
                case ResourceCost.ThreeHide:
                    EventGenerator.Singleton.RaiseLoseHideEvent(3);
                    break;
                default:
                    Debug.LogError($"Unable to process cost {payableCost}.");
                    break;
            }
        }

        foreach (ResourceCost costToQuery in costsToQuery)
        {
            EventGenerator.Singleton.RaiseSpawnVariableCostPopupEvent(costToQuery, actionAssignment);
        }

        // Pays the 2 determination for building a personal invention
        if (actionAssignment.Type == ActionType.BuildInvention && actionAssignment.inventionCard != null && actionAssignment.inventionCard.isPersonalInvention)
        {
            EventGenerator.Singleton.RaiseLoseDeterminationEvent(actionAssignment.playerIds[0], 2);
        }

        return true;
    }

    IEnumerator UpdateResources()
    {
        isWaitingForHideAvailable = true;
        isWaitingForWoodAvailable = true;
        EventGenerator.Singleton.RaiseGetHideEvent();
        EventGenerator.Singleton.RaiseGetWoodEvent();
        while (isWaitingForHideAvailable || isWaitingForWoodAvailable)
        {
            yield return null;
        }
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