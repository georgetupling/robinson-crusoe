using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionParser : MonoBehaviour
{
    static ActionParser singleton;
    public List<Transform> actionSpaces = new List<Transform>();
    public bool actionsReadyToSubmit = false;
    public List<ActionAssignment> actionAssignments = new List<ActionAssignment>();

    // A list of all action pawns in the scene (including disabled ones!)
    List<ActionPawnController> actionPawns = new List<ActionPawnController>();

    private Dictionary<int, ResourceCost> buildingCostsByPlayerCount = new Dictionary<int, ResourceCost>() {
        { 1, ResourceCost.TwoWoodOrHide },
        { 2, ResourceCost.TwoWoodOrHide },
        { 3, ResourceCost.ThreeWoodOrTwoHide },
        { 4, ResourceCost.FourWoodOrThreeHide }
    };

    // For handling queries

    List<int> distancesFromCampQueries = new List<int>();
    Dictionary<int, int> distancesFromCampByIslandTileId = new Dictionary<int, int>();

    List<int> distancesFromCampToLocationQueries = new List<int>();
    Dictionary<int, int> distancesFromCampByLocation = new Dictionary<int, int>();

    private bool isWaitingForHideAvailable = false;
    private bool isWaitingForWoodAvailable = false;
    private int hideAvailable = 0;
    private int woodAvailable = 0;

    private List<Invention> itemRequirementsMet = new List<Invention>();
    
    // Flags for applying specific effects
    
    int economicalConstructionPlayerId = -1;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        }
        EventGenerator.Singleton.AddListenerToActionPawnAssignedEvent(OnActionPawnAssignedEvent);
        EventGenerator.Singleton.AddListenerToGetDistanceFromCampEvent(OnGetDistanceFromCampEvent);
        EventGenerator.Singleton.AddListenerToGetDistanceFromCampToLocationEvent(OnGetDistanceFromCampToLocationEvent);
        EventGenerator.Singleton.AddListenerToGetResourceEvent(OnGetResourceEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
        EventGenerator.Singleton.AddListenerToActionPawnInitializedEvent(OnActionPawnInitializedEvent);
        EventGenerator.Singleton.AddListenerToAreSufficientResourcesAvailableEvent(OnAreSufficientResourcesAvailableEvent);
        EventGenerator.Singleton.AddListenerToEconomicalConstructionEvent(OnEconomicalConstructionEvent);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
    }

    // Listeners

    void OnAreSufficientResourcesAvailableEvent() {
        UpdateActionSpaces();
        ParseActions();
        bool response = SufficientResourcesAvailable();
        EventGenerator.Singleton.RaiseAreSufficientResourcesAvailableResponseEvent(response);
    }

    void OnActionPawnAssignedEvent() {
        UpdateActionSpaces();
        ParseActions();
        actionsReadyToSubmit = ValidateActions();
        EventGenerator.Singleton.RaiseActionsReadyToSubmitEvent(actionsReadyToSubmit, actionAssignments);
    }

    void UpdateActionSpaces() {
        actionSpaces.Clear();
        GameObject[] actionSpaceObjects = GameObject.FindGameObjectsWithTag("ActionSpace");
        foreach (GameObject actionSpaceObject in actionSpaceObjects) {
            actionSpaces.Add(actionSpaceObject.transform);
        }
    }

    void OnGetDistanceFromCampEvent(string eventType, int islandTileId, int distance) {
        if (eventType == GetDistanceFromCampEvent.Response && distancesFromCampQueries.Contains(islandTileId)) {
            distancesFromCampQueries.Remove(islandTileId);
            distancesFromCampByIslandTileId[islandTileId] = distance;
        }
    }

    void OnGetDistanceFromCampToLocationEvent(string eventType, int locationId, int distance) {
        if (eventType == GetDistanceFromCampEvent.Response && distancesFromCampToLocationQueries.Contains(locationId)) {
            distancesFromCampToLocationQueries.Remove(locationId);
            distancesFromCampByLocation[locationId] = distance;
        }
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

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt) {
        if (isBuilt) {
            itemRequirementsMet.Add(invention);
        } else {
            itemRequirementsMet.Remove(invention);
        }
    }

    void OnActionPawnInitializedEvent(ActionPawnController actionPawn) {
        // Adds the action pawn to the list
        actionPawns.Add(actionPawn);
    }
    
    void OnEconomicalConstructionEvent(int playerId) {
        economicalConstructionPlayerId = playerId;
    }

    void OnTurnStartEvent(int turnStarted) {
        economicalConstructionPlayerId = -1;
    }

    // Methods for parsing and validating actions

    void ParseActions() {
        actionAssignments.Clear();
        foreach(Transform actionSpace in actionSpaces) {
            if (HasPawnAssigned(actionSpace)) {
                ParseAction(actionSpace);
            }
        }
    }

    void ParseAction(Transform actionSpace) {
        ActionAssignment actionAssignment = new ActionAssignment();
        actionAssignment.Type = actionSpace.GetComponent<ActionSpace>().Type;
        actionAssignment.playerIds = GetPlayerIds(actionSpace);
        if (actionAssignment.playerIds.Count == 0) {
            Debug.LogError("Failed to assign player IDs while parsing actions.");
        }
        actionAssignment.numberOfActions = GetNumberOfActions(actionSpace);
        actionAssignment.resourceCosts = GetResourceCosts(actionSpace, actionAssignment.Type);
        actionAssignment.pawnComponentIds = GetPawnComponentIds(actionSpace);
        if (actionAssignment.Type == ActionType.Gather) {
            actionAssignment.islandTile = GetIslandTile(actionSpace);
            actionAssignment.isRightSource = GetIsRightSource(actionSpace);
        } else if (actionAssignment.Type == ActionType.Explore) {
            actionAssignment.locationId = GetLocationId(actionSpace);
        } else if (actionAssignment.Type == ActionType.BuildInvention) {
            actionAssignment.inventionCard = GetInventionCard(actionSpace);
        } else if (actionAssignment.Type == ActionType.Threat) {
            actionAssignment.eventCard = GetEventCard(actionSpace);
            actionAssignment.isTwoActionThreat = GetIsTwoActionThreat(actionSpace);
            actionAssignment.eventCardControllerComponentId = GetEventCardControllerComponentId(actionSpace);
        }
        actionAssignments.Add(actionAssignment);
    }

    bool ValidateActions() {
        // First checks that the correct number of actions is assigned for each action...
        foreach(ActionAssignment actionAssignment in actionAssignments) {
            if (!NumberOfActionsValid(actionAssignment)) {
                return false;
            };
        }
        // Then checks that all player action pawns are assigned
        int assignedPlayerActions = 0;
        int disabledActionPawns = 0;
        foreach (ActionPawnController actionPawn in actionPawns) {
            if (
                actionPawn.GetPlayerId() < GameSettings.PlayerCount && 
                actionPawn.gameObject.activeInHierarchy && 
                actionPawn.transform.parent != null &&
                actionPawn.transform.parent.parent != null &&
                actionPawn.transform.parent.parent.CompareTag("ActionSpace")
            ) {
                assignedPlayerActions++;
            } else if (!actionPawn.gameObject.activeSelf) {
                disabledActionPawns++;
            }
        }
        int totalPlayerActions = GameSettings.PlayerCount * 2;
        if (assignedPlayerActions + disabledActionPawns < totalPlayerActions) {
            return false;
        }
        // Then checks the resources are available and return if not
        if (!SufficientResourcesAvailable()) {
            return false;
        }
        return true;
    }

    // Action parsing helper methods

    bool HasPawnAssigned(Transform actionSpace) {
        for (int index = 0; index < actionSpace.childCount; index++) {
            Transform childTransform = actionSpace.GetChild(index);
            if (childTransform != null && childTransform.gameObject.activeInHierarchy && childTransform.childCount > 0) {
                return true;
            }
        }
        return false;
    }

    List<int> GetPlayerIds(Transform actionSpace) {
        List<int> playerIds = new List<int>();
        for (int index = 0; index < actionSpace.childCount; index++) {
            Transform childTransform = actionSpace.GetChild(index);
            if (childTransform != null && childTransform.gameObject.activeInHierarchy && childTransform.childCount > 0) {
                ActionPawnController actionPawn = childTransform.GetChild(0).GetComponent<ActionPawnController>();
                if (actionPawn == null) {
                    Debug.LogError("Child object is not an action pawn!");
                    continue;
                }
                playerIds.Add(actionPawn.GetPlayerId());
            }
        }
        return playerIds;
    }

    int GetNumberOfActions(Transform actionSpace) {
        int actionPawnCount = 0;
        for (int index = 0; index < actionSpace.childCount; index++) {
            Transform childTransform = actionSpace.GetChild(index);
            if (childTransform != null && childTransform.gameObject.activeInHierarchy && childTransform.childCount > 0) {
                actionPawnCount++;
            }
        }
        return actionPawnCount;
    }

    List<ResourceCost> GetResourceCosts(Transform actionSpace, ActionType actionType) {
        if (actionType == ActionType.BuildShelter || actionType == ActionType.BuildRoof || actionType == ActionType.BuildPalisade) {
            return new List<ResourceCost> { buildingCostsByPlayerCount[GameSettings.PlayerCount] };
        } else if (actionType == ActionType.BuildWeapon) {
            return new List<ResourceCost> { ResourceCost.Wood };
        } else if (actionType == ActionType.BuildInvention) {
            InventionCardController inventionCardController = actionSpace.parent.GetComponent<InventionCardController>();
            if (inventionCardController != null) {
                InventionCard data = inventionCardController.GetInventionCard();
                return data.resourceCosts;
            }
        } else if (actionType == ActionType.Threat) {
            EventCardController eventCardController = actionSpace.parent.GetComponent<EventCardController>();
            EventCard eventCard = eventCardController.GetEventCard();
            return eventCard.threatResourceCosts;
        }
        return new List<ResourceCost>();
    }

    List<int> GetPawnComponentIds(Transform actionSpace) {
        List<int> pawnComponentIds = new List<int>();
        for (int index = 0; index < actionSpace.childCount; index++) { 
            Transform childTransform = actionSpace.GetChild(index);
            if (childTransform != null && childTransform.childCount > 0) {
                ActionPawnController actionPawn = childTransform.GetChild(0).GetComponent<ActionPawnController>();
                pawnComponentIds.Add(actionPawn.ComponentId);
            }
        }
        return pawnComponentIds;
    }

    IslandTile GetIslandTile(Transform actionSpace) {
        IslandTileController islandTileController = actionSpace.parent.parent.GetComponent<IslandTileController>();
        if (islandTileController == null) {
            Debug.LogError("Failed to find island tile controller.");
        }
        return islandTileController.GetIslandTile();
    }

    bool GetIsRightSource(Transform actionSpace) {
        GatherActionSpaceController gatherActionSpaceController = actionSpace.GetComponent<GatherActionSpaceController>();
        return gatherActionSpaceController.GetIsRightSource();
    }

    int GetLocationId(Transform actionSpace) {
        ExploreActionAreaController exploreActionAreaController = actionSpace.GetComponent<ExploreActionAreaController>();
        return exploreActionAreaController.GetLocationId();
    }

    InventionCard GetInventionCard(Transform actionSpace) {
        InventionCardController inventionCardController = actionSpace.parent.GetComponent<InventionCardController>();
            if (inventionCardController != null) {
                InventionCard data = inventionCardController.GetInventionCard();
                return data;
            }
        return null; // Default
    }

    EventCard GetEventCard(Transform actionSpace) {
        EventCardController eventCardController = actionSpace.parent.GetComponent<EventCardController>();
        if (eventCardController == null) {
            Debug.LogError("EventCardController not found during Threat action assignment.");
            return null;
        } else {
            return eventCardController.GetEventCard();
        }
    }

    bool GetIsTwoActionThreat(Transform actionSpace) {
        ThreatActionSpaceController threatActionSpaceController = actionSpace.GetComponent<ThreatActionSpaceController>();
        if (threatActionSpaceController == null) {
            Debug.LogError("ThreatActionSpaceController not found during Threat action assignment.");
            return false;
        } else {
            return threatActionSpaceController.GetIsTwoActionThreat();
        }
    }

    int GetEventCardControllerComponentId(Transform actionSpace) {
        EventCardController eventCardController = actionSpace.parent.GetComponent<EventCardController>();
        if (eventCardController == null) {
            Debug.LogError("EventCardController not found during Threat action assignment.");
            return -1;
        } else {
            return eventCardController.ComponentId;
        }
    }

    // Validation helper methods

    bool NumberOfActionsValid(ActionAssignment actionAssignment) {
        ActionType actionType = actionAssignment.Type;
        if (actionType == ActionType.BuildShelter || actionType == ActionType.BuildRoof || actionType == ActionType.BuildPalisade || actionType == ActionType.BuildWeapon) {
            // TODO - check for time-consuming action tokens
            if (actionAssignment.numberOfActions == 1) {
                actionAssignment.mustRoll = true;
                return true;
            } else if (actionAssignment.numberOfActions == 2) {
                actionAssignment.mustRoll = false;
                return true;
            } else {
                return false;
            }
        } else if (actionType == ActionType.BuildInvention) {
            // TODO - check for time-consuming action tokens
            if (actionAssignment.numberOfActions == 1) {
                actionAssignment.mustRoll = true;
                return true;
            } else if (actionAssignment.numberOfActions == 2) {
                actionAssignment.mustRoll = false;
                return true;
            } else {
                return false;
            }
        } else if (actionType == ActionType.Gather) {
            int numberOfActions = actionAssignment.numberOfActions;
            StartCoroutine(UpdateDistanceFromCamp(actionAssignment.islandTile.Id));
            int distanceFromCamp = distancesFromCampByIslandTileId[actionAssignment.islandTile.Id];
            if (distanceFromCamp == -1 || distanceFromCamp == 0) {
                return false;
            }
            int numberOfActionsToRoll = distanceFromCamp;
            int numberOfActionsToGuarantee = distanceFromCamp + 1;
            if (numberOfActions == numberOfActionsToRoll) {
                actionAssignment.mustRoll = true;
                return true;
            } else if (numberOfActions == numberOfActionsToGuarantee) {
                actionAssignment.mustRoll = false;
                return true;
            } else {
                return false;
            }
        } else if (actionType == ActionType.Explore) {
            int numberOfActions = actionAssignment.numberOfActions;
            StartCoroutine(UpdateDistanceFromCampToLocation(actionAssignment.locationId));
            int distanceFromCamp = distancesFromCampByLocation[actionAssignment.locationId];
            if (distanceFromCamp == -1) {
                return false;
            }
            int numberOfActionsToRoll = distanceFromCamp;
            int numberOfActionsToGuarantee = distanceFromCamp + 1;
            if (numberOfActions == numberOfActionsToRoll) {
                actionAssignment.mustRoll = true;
                return true;
            } else if (numberOfActions == numberOfActionsToGuarantee) {
                actionAssignment.mustRoll = false;
                return true;
            } else {
                return false;
            }
        } else if (actionType == ActionType.Hunting) {
            return actionAssignment.numberOfActions == 2;
        } else if (actionType == ActionType.Threat) {
            if (actionAssignment.isTwoActionThreat) {
                return actionAssignment.numberOfActions == 2;
            } else {
                return actionAssignment.numberOfActions == 1;
            }
        } else {
            return actionAssignment.numberOfActions <= 8;
            // Make camp and rest can have up to 8 actions each!
        }
    }

    IEnumerator UpdateDistanceFromCamp(int islandTileId) {
        distancesFromCampQueries.Add(islandTileId);
        EventGenerator.Singleton.RaiseGetDistanceFromCampEvent(islandTileId);
        while (distancesFromCampQueries.Contains(islandTileId)) {
            yield return null;
        }
    }

    IEnumerator UpdateDistanceFromCampToLocation(int locationId) {
        distancesFromCampToLocationQueries.Add(locationId);
        EventGenerator.Singleton.RaiseGetDistanceFromCampToLocationEvent(locationId);
        while (distancesFromCampToLocationQueries.Contains(locationId)) {
            yield return null;
        }
    }

    bool SufficientResourcesAvailable() {
        Dictionary<ResourceCost, int> requiredResources = new Dictionary<ResourceCost, int>();

        bool economicalConstructionApplied = false;
        foreach (ActionAssignment actionAssignment in actionAssignments) {
            foreach (ResourceCost resourceCost in actionAssignment.resourceCosts) {
                // Checks if economical construction is active and takes it into account by reducing wood costs by 1
                ResourceCost requiredCost = resourceCost;
                if (actionAssignment.playerIds[0] == economicalConstructionPlayerId && !economicalConstructionApplied) {
                    switch (resourceCost) {
                        case ResourceCost.Wood: requiredCost = ResourceCost.ReducedToZero; economicalConstructionApplied = true; break;
                        case ResourceCost.TwoWood: requiredCost = ResourceCost.Wood; economicalConstructionApplied = true; break;
                        case ResourceCost.ThreeWood: requiredCost = ResourceCost.TwoWood; economicalConstructionApplied = true; break;
                        case ResourceCost.FourWood: requiredCost = ResourceCost.ThreeWood; economicalConstructionApplied = true; break;
                        case ResourceCost.TwoWoodOrHide: requiredCost = ResourceCost.WoodOrHide; economicalConstructionApplied = true; break;
                        case ResourceCost.ThreeWoodOrTwoHide: requiredCost = ResourceCost.TwoWoodOrTwoHide; economicalConstructionApplied = true; break;
                        case ResourceCost.FourWoodOrThreeHide: requiredCost = ResourceCost.ThreeWoodOrThreeHide; economicalConstructionApplied = true; break;
                    }
                }
                if (requiredCost == ResourceCost.ReducedToZero) {
                    continue;
                }

                // Then adds the resources to the dictionary
                if (requiredResources.ContainsKey(requiredCost)) {
                    requiredResources[requiredCost]++;
                } else {
                    requiredResources[requiredCost] = 1;
                }
            }
        }

        // Converts all the multiple resource requirements into just Wood and Hide requirements

        if (requiredResources.ContainsKey(ResourceCost.TwoWood)) {
            int requiredTwoWoodCount = requiredResources[ResourceCost.TwoWood];
            requiredResources.Remove(ResourceCost.TwoWood);

            if (requiredResources.ContainsKey(ResourceCost.Wood)) {
                requiredResources[ResourceCost.Wood] += requiredTwoWoodCount * 2;
            } else {
                requiredResources[ResourceCost.Wood] = requiredTwoWoodCount * 2;
            }
        }

        if (requiredResources.ContainsKey(ResourceCost.ThreeWood)) {
            int requiredThreeWoodCount = requiredResources[ResourceCost.ThreeWood];
            requiredResources.Remove(ResourceCost.ThreeWood);

            if (requiredResources.ContainsKey(ResourceCost.Wood)) {
                requiredResources[ResourceCost.Wood] += requiredThreeWoodCount * 3;
            } else {
                requiredResources[ResourceCost.Wood] = requiredThreeWoodCount * 3;
            }
        }

        if (requiredResources.ContainsKey(ResourceCost.FourWood)) {
            int requiredFourWoodCount = requiredResources[ResourceCost.FourWood];
            requiredResources.Remove(ResourceCost.FourWood);

            if (requiredResources.ContainsKey(ResourceCost.Wood)) {
                requiredResources[ResourceCost.Wood] += requiredFourWoodCount * 4;
            } else {
                requiredResources[ResourceCost.Wood] = requiredFourWoodCount * 4;
            }
        }

        if (requiredResources.ContainsKey(ResourceCost.TwoHide)) {
            int requiredTwoHideCount = requiredResources[ResourceCost.TwoHide];
            requiredResources.Remove(ResourceCost.TwoHide);

            if (requiredResources.ContainsKey(ResourceCost.Hide)) {
                requiredResources[ResourceCost.Hide] += requiredTwoHideCount * 2;
            } else {
                requiredResources[ResourceCost.Hide] = requiredTwoHideCount * 2;
            }
        }

        if (requiredResources.ContainsKey(ResourceCost.ThreeHide)) {
            int requiredThreeHideCount = requiredResources[ResourceCost.ThreeHide];
            requiredResources.Remove(ResourceCost.ThreeHide);

            if (requiredResources.ContainsKey(ResourceCost.Hide)) {
                requiredResources[ResourceCost.Hide] += requiredThreeHideCount * 3;
            } else {
                requiredResources[ResourceCost.Hide] = requiredThreeHideCount * 3;
            }
        }

        if (requiredResources.ContainsKey(ResourceCost.WoodUnlessShovel)) {
            int requiredWoodUnlessShovelCount = requiredResources[ResourceCost.WoodUnlessShovel];
            requiredResources.Remove(ResourceCost.WoodUnlessShovel);

            if (!itemRequirementsMet.Contains(Invention.Shovel)) {
                if (requiredResources.ContainsKey(ResourceCost.Wood)) {
                    requiredResources[ResourceCost.Wood] += requiredWoodUnlessShovelCount;
                } else {
                    requiredResources[ResourceCost.Wood] = requiredWoodUnlessShovelCount;
                }
            }
        }

        // Checks if enough resources are available to complete all action assignments

        isWaitingForHideAvailable = true;
        isWaitingForWoodAvailable = true;
        EventGenerator.Singleton.RaiseGetHideEvent();
        EventGenerator.Singleton.RaiseGetWoodEvent();
        while (isWaitingForHideAvailable || isWaitingForWoodAvailable) {
            // Do nothing
        }
        int unspentWood = woodAvailable;
        int unspentHide = hideAvailable;
        if (requiredResources.ContainsKey(ResourceCost.Wood)) {
            int woodRequirement = requiredResources[ResourceCost.Wood];
            if (unspentWood < woodRequirement) {
                return false;
            } else {
                unspentWood -= woodRequirement;
                requiredResources.Remove(ResourceCost.Wood);
            }
        }
        if (requiredResources.ContainsKey(ResourceCost.Hide)) {
            int hideRequirement = requiredResources[ResourceCost.Hide];
            if (unspentHide < hideRequirement) {
                return false;
            } else {
                unspentHide -= hideRequirement;
                requiredResources.Remove(ResourceCost.Hide);
            }
        }
        
        // Creates parallel lists of required resources and whether they're paid with hide or not

        List<ResourceCost> requiredResourcesList = new List<ResourceCost>();
        foreach (ResourceCost resourceCost in requiredResources.Keys) {
            for (int i = 0; i < requiredResources[resourceCost]; i++) {
                requiredResourcesList.Add(resourceCost);
            }
        }
        List<bool> paidWithHide = new List<bool>();
        foreach(ResourceCost resourceCost in requiredResourcesList) {
            paidWithHide.Add(false);
        }

        // Searches for a combination that can be paid by randomly modifying whether costs are paid with hide or not

        for (int i = 0; i < 100; i++) {
            if (CombinationIsValid(requiredResourcesList, paidWithHide, unspentWood, unspentHide)) {
                return true;
            } else {
                int randomIndex = Random.Range(0, paidWithHide.Count);
                paidWithHide[randomIndex] = paidWithHide[randomIndex] == false ? true : false;
            }
        }
        return false;

    }

    bool CombinationIsValid(List<ResourceCost> requiredResourcesList, List<bool> paidWithHide, int unspentWood, int unspentHide) {
        int hideRequirement = 0;
        int woodRequirement = 0;
        for (int i = 0; i < requiredResourcesList.Count; i++) {
            if (requiredResourcesList[i] == ResourceCost.TwoWoodOrHide) {
                if (paidWithHide[i] == true) {
                    hideRequirement++;
                } else {
                    woodRequirement += 2;
                }
            } else if (requiredResourcesList[i] == ResourceCost.ThreeWoodOrTwoHide) {
                if (paidWithHide[i] == true) {
                    hideRequirement += 2;
                } else {
                    woodRequirement += 3;
                }
            } else if (requiredResourcesList[i] == ResourceCost.FourWoodOrThreeHide) {
                if (paidWithHide[i] == true) {
                    hideRequirement += 3;
                } else {
                    woodRequirement += 4;
                }
            } else if (requiredResourcesList[i] == ResourceCost.WoodOrHide) {
                if (paidWithHide[i] == true) {
                    hideRequirement += 1;
                } else {
                    woodRequirement += 1;
                }
            } else if (requiredResourcesList[i] == ResourceCost.TwoWoodOrTwoHide) {
                if (paidWithHide[i] == true) {
                    hideRequirement += 2;
                } else {
                    woodRequirement += 2;
                } 
            } else if (requiredResourcesList[i] == ResourceCost.ThreeWoodOrThreeHide) {
                if (paidWithHide[i] == true) {
                    hideRequirement += 3;
                } else {
                    woodRequirement += 3;
                }
            } else {
                Debug.LogError("Invalid resource cost.");
            }
        }
        return hideRequirement <= unspentHide && woodRequirement <= unspentWood;
    }
    
}
