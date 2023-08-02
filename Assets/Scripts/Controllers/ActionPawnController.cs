using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionPawnController : ComponentController, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    int playerId;
    bool isActionPhase;
    bool actionsSubmitted;

    Transform originalTransform;
    Transform playArea;
    List<Transform> actionSpaces = new List<Transform>();

    // Fields for non-standard pawns
    public bool isSingleUse { get; private set; }
    public PawnType pawnType { get; private set; }

    // Effects modifying action placement
    bool canOnlyRestThisTurn;
    bool canOnlyRestBuildOrMakeCampThisTurn;
    
    protected override void Awake() {
        base.Awake();
        originalTransform = transform.parent;
        playArea = GameObject.Find("PlayArea").transform;
        UpdateActionSpaces();
        EventGenerator.Singleton.AddListenerToInitializeActionPawnEvent(OnInitializeActionPawnEvent);
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToReturnActionPawnEvent(OnReturnActionPawnEvent);
        EventGenerator.Singleton.AddListenerToActionsSubmittedEvent(OnActionsSubmittedEvent);
        EventGenerator.Singleton.AddListenerToPlayerCanOnlyRestThisTurnEvent(OnPlayerCanOnlyRestThisTurnEffect);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
        EventGenerator.Singleton.AddListenerToPlayerCanOnlyRestBuildOrMakeCampThisTurnEvent(OnPlayerCanOnlyRestBuildOrMakeCampThisTurnEvent);
        EventGenerator.Singleton.AddListenerToGetPhaseResponseEvent(OnGetPhaseResponseEvent);
    }

    void UpdateActionSpaces() {
        actionSpaces.Clear();
        GameObject[] actionSpaceObjects = GameObject.FindGameObjectsWithTag("ActionSpace");
        foreach (GameObject actionSpaceObject in actionSpaceObjects) {
            actionSpaces.Add(actionSpaceObject.transform);
        }
    }

    // Initialisation

    void OnInitializeActionPawnEvent(int componentId, int playerId) {
        // This event is used to initialize player pawns
        if (componentId != this.ComponentId) {
            return;
        }
        this.playerId = playerId;
        isSingleUse =  false;
        pawnType = PawnType.Player;
        string materialName = GameSettings.PlayerCharacters[playerId].ToString() + GameSettings.PlayerGenders[playerId].ToString() + "ActionPawnMaterial";
        Material material = Resources.Load<Material>(Path.Combine("Materials/Action Pawns", materialName));
        GetComponent<MeshRenderer>().material = material;
        // Passes the player Id to the ActionPawnDisabler if one exists
        ActionPawnDisabler actionPawnDisabler = GetComponent<ActionPawnDisabler>();
        if (actionPawnDisabler != null) {
            actionPawnDisabler.SetPlayerId(playerId);
        }
        EventGenerator.Singleton.RaiseActionPawnInitializedEvent(this);
    }

    public void InitializeNonPlayerPawn(int playerId, bool isSingleUse, PawnType pawnType) {
        // This method is used to initialize non-player pawns
        this.playerId = playerId;
        this.isSingleUse =  isSingleUse;
        this.pawnType = pawnType;
        string materialName = pawnType.ToString() + "ActionPawnMaterial";
        Material material = Resources.Load<Material>(Path.Combine("Materials/Action Pawns", materialName));
        GetComponent<MeshRenderer>().material = material;
        // Passes the player Id to the ActionPawnDisabler if one exists
        ActionPawnDisabler actionPawnDisabler = GetComponent<ActionPawnDisabler>();
        if (actionPawnDisabler != null) {
            actionPawnDisabler.SetPlayerId(playerId);
        }
        EventGenerator.Singleton.RaiseGetPhaseEvent();
    }

    // Listeners

    void OnPhaseStartEvent(Phase phase) {
        isActionPhase = phase == Phase.Action;
        if (phase == Phase.Action) {
            actionsSubmitted = false;
        }
    }

    void OnActionsSubmittedEvent(List<ActionAssignment> actionAssignments) {
        actionsSubmitted = true;
    }

    void OnReturnActionPawnEvent(int componentId) {
        if (componentId != this.ComponentId || transform.parent == originalTransform) {
            return;
        }
        MoveToTransform(originalTransform, MoveStyle.ReturnPawn);
    }

    void OnPlayerCanOnlyRestThisTurnEffect(int playerId) {
        if (playerId != this.playerId) {
            return;
        }
        canOnlyRestThisTurn = true;
    }

    void OnTurnStartEvent(int turnStarted) {
        // Certain effects end at the start of the next turn
        canOnlyRestThisTurn = false;
        canOnlyRestBuildOrMakeCampThisTurn = false;
        // Single use pawns disappear at the start of the turn
        if (isSingleUse) {
            // EventGenerator.Singleton.RaiseActionPawnDestroyedEvent(this.ComponentId);
            Destroy(gameObject);
        }
    }

    void OnPlayerCanOnlyRestBuildOrMakeCampThisTurnEvent(int playerId) {
        if (playerId != this.playerId) {
            return;
        }
        canOnlyRestBuildOrMakeCampThisTurn = true;
    }

    void OnGetPhaseResponseEvent(Phase currentPhase) {
        isActionPhase = currentPhase == Phase.Action;
    }

    // Methods for dragging and dropping

    public void OnBeginDrag(PointerEventData pointerEventData) {
        if (!isActionPhase || actionsSubmitted) {
            Shake();
            return;
        }
        Transform grandparentTransform = transform.parent.parent;
        transform.SetParent(playArea, true);
        if (grandparentTransform != null && grandparentTransform.CompareTag("ActionSpace")) {
            UpdateOtherActionPawnPositions(grandparentTransform);
        }
        UpdateActionSpaces();
    }

    public void OnDrag(PointerEventData pointerEventData) {
        while (transform.parent != playArea) {
            return;
        }
        transform.position = GetMouseWorldPosition();
    }

   public void OnEndDrag(PointerEventData pointerEventData) {
        if (transform.parent != playArea) {
            return;
        }
        Transform nearestActionSpace = GetNearestActionSpace();
        ActionType actionType = ActionType.NotFound;
        if (GetAvailablePosition(nearestActionSpace) != null) {
            transform.SetParent(GetAvailablePosition(nearestActionSpace), true);
        }
        if (nearestActionSpace != null && nearestActionSpace.GetComponent<ActionSpace>() != null) {
            actionType = nearestActionSpace.GetComponent<ActionSpace>().Type;
        }
        // Applies restrictions for specific pawn types
        List<ActionType> buildTypes = new List<ActionType> { ActionType.BuildShelter, ActionType.BuildRoof, ActionType.BuildPalisade, ActionType.BuildWeapon };
        if (pawnType != PawnType.Player) {
            if (
                (pawnType == PawnType.Build && !(buildTypes.Contains(actionType) || actionType == ActionType.BuildInvention)) ||
                (pawnType == PawnType.Gather && actionType != ActionType.Gather) ||
                (pawnType == PawnType.Explore && actionType != ActionType.Explore) ||
                (pawnType == PawnType.Hunting && actionType != ActionType.Hunting) ||
                (pawnType == PawnType.GatherOrExplore && !(actionType == ActionType.Gather || actionType == ActionType.Explore))
                // TODO: Add conditions for assigning dog and Friday!
                ) {
                nearestActionSpace = null;
            }
        }
        // If the player can only rest, sets non-rest action spaces to null
        if (nearestActionSpace != null && canOnlyRestThisTurn && actionType != ActionType.Rest) {
            nearestActionSpace = null;
        }
        // If the player can only rest, build, or make camp, sets other action spaces to null
        if (nearestActionSpace != null && canOnlyRestBuildOrMakeCampThisTurn) {
            List<ActionType> allowedActionTypes = new List<ActionType> { ActionType.Rest, ActionType.MakeCamp, ActionType.BuildShelter, ActionType.BuildRoof, ActionType.BuildPalisade, ActionType.BuildWeapon, ActionType.BuildInvention };
            if (!allowedActionTypes.Contains(actionType)) {
                nearestActionSpace = null;
            }
        }
        // Checks if the action space is an invention card and validates whether the requirements are met for assignment
        if (nearestActionSpace != null && nearestActionSpace.parent != null) {
            InventionCardController inventionCardController = nearestActionSpace.parent.GetComponent<InventionCardController>();
            if (inventionCardController != null) {
                InventionCard data = inventionCardController.GetInventionCard();
                if (!RequirementChecker.Singleton.InventionRequirementsMet(playerId, inventionCardController)) {
                    nearestActionSpace = null;
                    EventGenerator.Singleton.RaiseShakeComponentEvent(inventionCardController.ComponentId);
                }
            }
        }
        // Checks if the action space is a build action and validates whether the requirements are met
        if (nearestActionSpace != null && actionType != ActionType.NotFound) {
            if (buildTypes.Contains(actionType) && !RequirementChecker.Singleton.BuildRequirementsMet(actionType)) {
                nearestActionSpace = null;
            }
        }
        // Checks if the action space is a threat action and validates that the requirements are met
        if (nearestActionSpace != null && nearestActionSpace.parent != null && actionType == ActionType.Threat) {
            if (ThreatAlreadyAssigned(nearestActionSpace)) {
                nearestActionSpace = null;
            } else {
                EventCardController eventCardController = nearestActionSpace.parent.GetComponent<EventCardController>();
                EventCard eventCard = eventCardController.GetEventCard();
                if (!RequirementChecker.Singleton.ThreatRequirementsMet(eventCard)) {
                    nearestActionSpace = null;
                    EventGenerator.Singleton.RaiseShakeComponentEvent(eventCardController.ComponentId);
                }
            }
        }
        transform.SetParent(playArea, true);
        Transform availablePosition = nearestActionSpace == null ? originalTransform : GetAvailablePosition(nearestActionSpace);
        MoveStyle speed = nearestActionSpace == originalTransform ? MoveStyle.Default : MoveStyle.Fast;
        MoveToTransform(availablePosition, speed);
        EventGenerator.Singleton.RaiseActionPawnAssignedEvent();
    }

    // Moves the pawns that were previously on top of this pawn so that they're not floating

    void UpdateOtherActionPawnPositions(Transform actionArea) {
        for (int i = 0; i < actionArea.childCount; i++) {
            Transform positionI = actionArea.GetChild(i);
            if (positionI.childCount > 0) {
                continue;
            }
            for (int j = i + 1; j < actionArea.childCount; j++) {
                Transform positionJ = actionArea.GetChild(j);
                if (positionJ.childCount == 0) {
                    continue;
                }
                Transform positionJChild = positionJ.GetChild(0);
                ActionPawnController otherPawn = positionJChild.GetComponent<ActionPawnController>();
                EventGenerator.Singleton.RaiseMoveComponentEvent(otherPawn.ComponentId, positionI, Vector3.zero, MoveStyle.Fast);
                break;
            }
        }
    }

    // Helper methods

    Vector3 GetMouseWorldPosition() {
        Vector3 mousePosition = Input.mousePosition;
        float dragHeight = 0.5f;
        mousePosition.z = -Camera.main.transform.position.z - dragHeight;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    Transform GetNearestActionSpace() {
        Transform nearestActionSpace = null;
        float nearestDistance = Mathf.Infinity;
        float threshold = 0.75f;

        foreach (Transform actionSpace in actionSpaces) {
            if (actionSpace == null) {
                continue;
            }
            float distance = Vector3.Distance(transform.position, actionSpace.position);
            if (distance < nearestDistance && distance <= threshold && GetAvailablePosition(actionSpace) != null) {
                nearestActionSpace = actionSpace;
                nearestDistance = distance;
            }
        }
        return nearestActionSpace;
    }

    Transform GetAvailablePosition(Transform actionSpace) {
        if (actionSpace == null) {
            return null;
        }
        for (int index = 0; index < actionSpace.childCount; index++) {
            Transform childTransform = actionSpace.GetChild(index);
            if (childTransform != null && childTransform.gameObject.activeInHierarchy && childTransform.childCount == 0) {
                return childTransform;
            }
        }
        return null;
    }

    bool ThreatAlreadyAssigned(Transform actionSpace) {
        // Searches the action space's nephews to see if they already have a pawn assigned
        Transform parent = actionSpace.parent;
        for(int i = 0; i < parent.childCount; i++) {
            Transform sibling = parent.GetChild(i);
            if (sibling == actionSpace) {
                continue;
            }
            for(int j = 0; j < sibling.childCount; j++) {
                if (sibling.GetChild(j).childCount > 0) {
                    return true;
                }
            }
        }
        return false;
    }

    // Returns the player ID (used for parsing actions)

    public int GetPlayerId() {
        return playerId;
    }
}
