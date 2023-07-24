using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUIController : MonoBehaviour
{
    public static MainUIController Singleton { get; private set; }

    private CanvasGroup canvasGroup;

    // Controls
    [SerializeField] Button devToolsToggle;
    [SerializeField] Button resetActions;
    [SerializeField] Button submitActions;
    [SerializeField] Button continueButton;

    // Text
    [SerializeField] TMP_Text currentPhase;
    [SerializeField] TMP_Text playerInstruction;

    // Panels
    [SerializeField] DevToolsUIController devToolsPanel;

    // Stored info
    List<ActionAssignment> actionAssignments;
    

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
        canvasGroup = GetComponent<CanvasGroup>();
        SetUpButtons();
        EventGenerator.Singleton.AddListenerToEnableMainUIEvent(OnEnableMainUIEvent);
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToActionsReadyToSubmitEvent(OnActionsReadyToSubmitEvent);
        EventGenerator.Singleton.AddListenerToChooseAdjacentTileEvent(OnChooseAdjacentTileEvent);
        EventGenerator.Singleton.AddListenerToChooseAdjacentTileEvent(OnChooseAdjacentTileEvent);
    }

    void SetUpButtons() {
        devToolsToggle.onClick.AddListener(() => {
            devToolsPanel.gameObject.SetActive(!devToolsPanel.gameObject.activeSelf);
        });
        submitActions.onClick.AddListener(() => {
            resetActions.gameObject.SetActive(false);
            submitActions.gameObject.SetActive(false);
            playerInstruction.text = "Resolving player actions...";
            EventGenerator.Singleton.RaiseActionsSubmittedEvent(actionAssignments);
        });
        resetActions.onClick.AddListener(() => {
            List<ActionPawnController> actionPawns = new List<ActionPawnController>(FindObjectsOfType<ActionPawnController>());
            foreach (ActionPawnController actionPawn in actionPawns) {
                EventGenerator.Singleton.RaiseReturnActionPawnEvent(actionPawn.ComponentId);
            }
        });
        continueButton.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseAdjacentTileChosenEvent(false, -1);
            EventGenerator.Singleton.RaiseChooseAdjacentTileEvent(false);
        });
    }

    void OnEnableMainUIEvent(bool disable) {
        if (disable == true) {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1.0f;
        } else if (disable == false) {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.5f;
        }
    }

    void OnPhaseStartEvent(Phase phaseStarted) {
        currentPhase.text = phaseStarted.ToString() + " Phase";
        if (phaseStarted == Phase.Action) {
            resetActions.gameObject.SetActive(true);
            submitActions.interactable = false;
            submitActions.gameObject.SetActive(true);
        } else {
            resetActions.gameObject.SetActive(false);
            submitActions.gameObject.SetActive(false);
        }
    }

    void OnActionsReadyToSubmitEvent(bool actionsAreReadyToSubmit, List<ActionAssignment> actionAssignments) {
        this.actionAssignments = actionAssignments;
        submitActions.interactable = actionsAreReadyToSubmit;
    }

    void OnChooseAdjacentTileEvent(bool isActive) {
        if (isActive) {
            playerInstruction.text = "Select a highlighted tile to move camp. Otherwise press continue.";
        } else {
            playerInstruction.text = "Resolving night phase...";
        }
        continueButton.gameObject.SetActive(isActive);
    }
}
