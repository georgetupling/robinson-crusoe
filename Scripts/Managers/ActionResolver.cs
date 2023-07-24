using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionResolver : MonoBehaviour
{
    static ActionResolver singleton;

    public List<ActionAssignment> actionAssignments;
    [SerializeField] Transform popupsArea;

    const float delayBetweenActions = 0.75f;
    private int animationsInProgress = 0;

    // Verbiage for querying information from other classes

    private bool isWaitingForHideAvailable = false;
    private bool isWaitingForWoodAvailable = false;
    private int hideAvailable = 0;
    private int woodAvailable = 0;
    private List<Invention> builtInventions = new List<Invention>();

    bool rolledSuccess = false;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            return;
        }
        EventGenerator.Singleton.AddListenerToActionsSubmittedEvent(OnActionsSubmitted);
        EventGenerator.Singleton.AddListenerToGetResourceEvent(OnGetResourceEvent);
        EventGenerator.Singleton.AddListenerToAnimationInProgressEvent(OnAnimationInProgressEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
        EventGenerator.Singleton.AddListenerToDieRolledEvent(OnDieRolledEvent);
    }

    // Listeners

    void OnActionsSubmitted(List<ActionAssignment> actionAssignments) {
        this.actionAssignments = actionAssignments;
        StartCoroutine(ResolveThreatActions());
    }

    void OnGetResourceEvent(string eventType, int amount) {
        if (eventType == GetResourceEvent.GetHideResponse) {
            hideAvailable = amount;
            isWaitingForHideAvailable = false;
        } else if (eventType == GetResourceEvent.GetWoodResponse) {
            woodAvailable = amount;
            isWaitingForWoodAvailable = false;
        }
    }

    void OnAnimationInProgressEvent(bool isInProgress) {
        if (isInProgress) {
            animationsInProgress++;
        } else {
            animationsInProgress--;
        }
    }

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt) {
        if (isBuilt) {
            builtInventions.Add(invention);
        } else {
            builtInventions.Remove(invention);
        }
    }

    void OnDieRolledEvent(DieType dieType, int faceRolled) {
        if (dieType == DieType.BuildSuccess) {
            rolledSuccess = faceRolled > 1;
        } else if (dieType == DieType.GatherSuccess || dieType == DieType.ExploreSuccess) {
            rolledSuccess = faceRolled != 0;
        }
    }

    // Methods for resolving actions

    IEnumerator ResolveThreatActions() {
        foreach (ActionAssignment actionAssignment in actionAssignments) {
            if (actionAssignment.Type != ActionType.Threat) {
                continue;
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                yield return null;
            }
            foreach (int pawnComponentId in actionAssignment.pawnComponentIds) {
                    EventGenerator.Singleton.RaiseReturnActionPawnEvent(pawnComponentId);
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                yield return null;
            }
            EventCard eventCard = actionAssignment.eventCard;
            bool costsPaid = PayCosts(eventCard.threatResourceCosts, actionAssignment);
            while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                yield return null;
            }
            if (costsPaid) {
                List<CardEffect> successEffects = actionAssignment.isTwoActionThreat ? eventCard.successEffects2Action : eventCard.successEffects1Action;
                foreach (CardEffect successEffect in successEffects) {
                    successEffect.ApplyEffect();
                }
                EventGenerator.Singleton.RaiseDestroyComponentEvent(actionAssignment.eventCardControllerComponentId);
            }
            yield return new WaitForSeconds(delayBetweenActions);
        }
        StartCoroutine(ResolveHuntingActions());
    }

    IEnumerator ResolveHuntingActions() {
        foreach (ActionAssignment actionAssignment in actionAssignments) {
            if (actionAssignment.Type != ActionType.Hunting) {
                continue;
            }
            // TODO: once combat is implemented, you need to call the relevant events

            foreach (int pawnComponentId in actionAssignment.pawnComponentIds) {
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(pawnComponentId);
            }
            yield return new WaitForSeconds(delayBetweenActions);
        }
        StartCoroutine(ResolveBuildActions());
    }

    IEnumerator ResolveBuildActions() {
        foreach (ActionAssignment actionAssignment in actionAssignments) { 
            if (
                actionAssignment.Type != ActionType.BuildShelter &&
                actionAssignment.Type != ActionType.BuildRoof &&
                actionAssignment.Type != ActionType.BuildPalisade &&
                actionAssignment.Type != ActionType.BuildWeapon &&
                actionAssignment.Type != ActionType.BuildInvention
            ) {
                continue;
            }
            foreach (int pawnComponentId in actionAssignment.pawnComponentIds) {
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(pawnComponentId);
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                yield return null;
            }
            bool costsPaid = PayCosts(actionAssignment.resourceCosts, actionAssignment);
            if (costsPaid) {
                bool buildIsSuccessful = true;
                if (actionAssignment.mustRoll) {
                    DiceRoller.Singleton.RollBuildDice(actionAssignment.playerIds[0]);
                    while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                        yield return null;
                    }
                    if (!rolledSuccess) {
                        buildIsSuccessful = false;
                    }
                }
                if (buildIsSuccessful) {
                    switch (actionAssignment.Type) {
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
                            EventGenerator.Singleton.RaiseBuildInventionSuccessEvent(actionAssignment.invention);
                            break;
                        case ActionType.BuildWeapon:
                            EventGenerator.Singleton.RaiseGainWeaponEvent(1);
                            break;
                    }
                } else {
                    Debug.Log("Build unsuccessful."); // For testing purposes
                }
            }
            yield return new WaitForSeconds(delayBetweenActions);
        }
        StartCoroutine(ResolveGatherActions());
    }

    IEnumerator ResolveGatherActions() {
        foreach (ActionAssignment actionAssignment in actionAssignments) { 
            if (actionAssignment.Type != ActionType.Gather) {
                continue;
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                yield return null;
            }
            foreach (int pawnComponentId in actionAssignment.pawnComponentIds) {
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(pawnComponentId);
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                yield return null;
            }
            bool gatherIsSuccessful = true;
            if (actionAssignment.mustRoll) {
                gatherIsSuccessful = DiceRoller.RollGatherDice(actionAssignment.playerIds[0], actionAssignment.islandTile.Id);
            }
            if (gatherIsSuccessful) {
                EventGenerator.Singleton.RaiseGatherSuccessEvent(actionAssignment.islandTile.Id, actionAssignment.isRightSource);
            } else {
                Debug.Log("Gather unsuccessful."); // For testing purposes
            }
            yield return new WaitForSeconds(delayBetweenActions);
        }
        StartCoroutine(ResolveExploreActions());
    }

    IEnumerator ResolveExploreActions() {
        foreach (ActionAssignment actionAssignment in actionAssignments) { 
            if (actionAssignment.Type != ActionType.Explore) {
                continue;
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                yield return null;
            }
            foreach (int pawnComponentId in actionAssignment.pawnComponentIds) {
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(pawnComponentId);
            }
            while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                yield return null;
            }
            bool exploreIsSuccessful = true;
            if (actionAssignment.mustRoll) {
                exploreIsSuccessful = DiceRoller.RollExploreDice(actionAssignment.playerIds[0], actionAssignment.locationId);
            }
            if (exploreIsSuccessful) {
                EventGenerator.Singleton.RaiseDrawIslandTileEvent(actionAssignment.locationId);
            } else {
                Debug.Log("Explore unsuccessful."); // For testing purposes
            }
            yield return new WaitForSeconds(delayBetweenActions);
        }
        StartCoroutine(ResolveMakeCampActions());
    }

    IEnumerator ResolveMakeCampActions() {
        foreach (ActionAssignment actionAssignment in actionAssignments) { 
            if (actionAssignment.Type != ActionType.MakeCamp) {
                continue;
            }
            // The lists are reversed so that the top action pawn is removed first instead of the bottom
            List<int> reversedPlayerIds = new List<int>(actionAssignment.playerIds);
            reversedPlayerIds.Reverse();
            List<int> reversedPawnComponentIds = new List<int>(actionAssignment.pawnComponentIds);
            reversedPawnComponentIds.Reverse();
            for (int i = 0; i < reversedPlayerIds.Count; i++) {
                while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                    yield return null;
                }
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(reversedPawnComponentIds[i]);
                while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                    yield return null;
                }
                if (GameSettings.PlayerCount < 4) {
                    EventGenerator.Singleton.RaiseGainDeterminationEvent(reversedPlayerIds[i], 2);
                    EventGenerator.Singleton.RaiseGainMoraleEvent(1);
                } else {
                    EventGenerator.Singleton.RaiseSpawnMakeCampChoicePopupEvent(reversedPlayerIds[i]);
                }
                yield return new WaitForSeconds(delayBetweenActions);
            }
        }
        StartCoroutine(ResolveRestActions());
    }

    IEnumerator ResolveRestActions() {
        foreach (ActionAssignment actionAssignment in actionAssignments) { 
            if (actionAssignment.Type != ActionType.Rest) {
                continue;
            }
            // The lists are reversed so that the top action pawn is removed first instead of the bottom
            List<int> reversedPlayerIds = new List<int>(actionAssignment.playerIds);
            reversedPlayerIds.Reverse();
            List<int> reversedPawnComponentIds = new List<int>(actionAssignment.pawnComponentIds);
            reversedPawnComponentIds.Reverse();
            for (int i = 0; i < reversedPlayerIds.Count; i++) {
                while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                    yield return null;
                }
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(reversedPawnComponentIds[i]);
                while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                    yield return null;
                }
                EventGenerator.Singleton.RaiseGainHealthEvent(reversedPlayerIds[i], 1);
                yield return new WaitForSeconds(delayBetweenActions);
            }
        }
        while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                    yield return null;
            }
        EventGenerator.Singleton.RaiseMakeResourcesAvailableEvent();
        while (popupsArea.childCount > 0 || animationsInProgress > 0) {
                    yield return null;
            }
        EventGenerator.Singleton.RaiseEndPhaseEvent(Phase.Action);
    }

    // Helper methods

    bool PayCosts(List<ResourceCost> resourceCosts, ActionAssignment actionAssignment) {
        bool canPayWithHide = false;
        bool canPayWithWood = false;
        List<ResourceCost> payableCosts = new List<ResourceCost>();
        List<ResourceCost> costsToQuery = new List<ResourceCost>();
        StartCoroutine(UpdateResources());
        resourceCosts.Sort(); // The list is sorted so that fixed costs are evaluated before variable costs
        foreach (ResourceCost resourceCost in resourceCosts) {
            switch (resourceCost) {
                case ResourceCost.Wood:
                    if (woodAvailable >= 1) {
                        payableCosts.Add(resourceCost);
                        woodAvailable--;
                    } else {
                        return false;
                    }
                    break;
                case ResourceCost.Hide:
                    if (hideAvailable >= 1) {
                        payableCosts.Add(resourceCost);
                        hideAvailable--;
                    } else {
                        return false;
                    }
                    break;
                case ResourceCost.TwoWood:
                    if (woodAvailable >= 2) {
                        payableCosts.Add(resourceCost);
                        woodAvailable -= 2;
                    } else {
                        return false;
                    }
                    break;
                case ResourceCost.TwoHide:
                    if (hideAvailable >= 2) {
                        payableCosts.Add(resourceCost);
                        hideAvailable -= 2;
                    } else {
                        return false;
                    }
                    break;
                case ResourceCost.ThreeHide:
                    if (hideAvailable >= 3) {
                        payableCosts.Add(resourceCost);
                        hideAvailable -= 3;
                    } else {
                        return false;
                    }
                    break;
                case ResourceCost.ThreeWood:
                    if (woodAvailable >= 3) {
                        payableCosts.Add(resourceCost);
                        woodAvailable -= 3;
                    } else {
                        return false;
                    }
                    break;
                case ResourceCost.FourWood:
                    if (woodAvailable >= 4) {
                        payableCosts.Add(resourceCost);
                        woodAvailable -= 4;
                    } else {
                        return false;
                    }
                    break;
                case ResourceCost.TwoWoodOrHide:
                    canPayWithWood = woodAvailable >= 2;
                    canPayWithHide = hideAvailable >= 1;
                    if (canPayWithWood && canPayWithHide) {
                        costsToQuery.Add(resourceCost);
                    } else if (canPayWithWood) {
                        payableCosts.Add(ResourceCost.TwoWood);
                        woodAvailable -= 2;
                    } else if (canPayWithHide) {
                        payableCosts.Add(ResourceCost.Hide);
                        hideAvailable -= 1;
                    } else {
                        return false;
                    }
                    break;
                case ResourceCost.ThreeWoodOrTwoHide:
                    canPayWithWood = woodAvailable >= 3;
                    canPayWithHide = hideAvailable >= 2;
                    if (canPayWithWood && canPayWithHide) {
                        costsToQuery.Add(resourceCost);
                    } else if (canPayWithWood) {
                        payableCosts.Add(ResourceCost.ThreeWood);
                        woodAvailable -= 3;
                    } else if (canPayWithHide) {
                        payableCosts.Add(ResourceCost.TwoHide);
                        hideAvailable -= 2;
                    } else {
                        return false;
                    }
                    break;
                case ResourceCost.FourWoodOrThreeHide:
                    canPayWithWood = woodAvailable >= 4;
                    canPayWithHide = hideAvailable >= 3;
                    if (canPayWithWood && canPayWithHide) {
                        costsToQuery.Add(resourceCost);
                    } else if (canPayWithWood) {
                        payableCosts.Add(ResourceCost.FourWood);
                        woodAvailable -= 4;
                    } else if (canPayWithHide) {
                        payableCosts.Add(ResourceCost.ThreeHide);
                        hideAvailable -= 3;
                    } else {
                        return false;
                    }
                    break;
                case ResourceCost.WoodUnlessShovel:
                    if (!builtInventions.Contains(Invention.Shovel)) {
                        if (woodAvailable >= 1) {
                            payableCosts.Add(ResourceCost.Wood);
                            woodAvailable -= 1;
                        } else {
                            return false;
                        }
                    }
                    break;
                default:
                    Debug.LogError($"Unable to process cost {resourceCost}.");
                    return false;
            }
        }
        
        foreach (ResourceCost payableCost in payableCosts) {
            switch (payableCost) {
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

        foreach (ResourceCost costToQuery in costsToQuery) {
            EventGenerator.Singleton.RaiseSpawnVariableCostPopupEvent(costToQuery, actionAssignment);
        }

        return true;
    }

    IEnumerator UpdateResources() {
        isWaitingForHideAvailable = true;
        isWaitingForWoodAvailable = true;
        EventGenerator.Singleton.RaiseGetHideEvent();
        EventGenerator.Singleton.RaiseGetWoodEvent();
        while (isWaitingForHideAvailable || isWaitingForWoodAvailable) {
            yield return null;
        }
    }

}