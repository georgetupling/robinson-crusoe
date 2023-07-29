using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VariableCostPopupController : MonoBehaviour
{
    [SerializeField] private TMP_Text instruction;
    [SerializeField] private Button woodButton;
    [SerializeField] private Button hideButton;

    int woodAmount;
    int hideAmount;

    void Awake() {
        SetUpButtons();
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
    }

    void SetUpButtons() {
        woodButton.onClick.AddListener(OnChooseWood);
        hideButton.onClick.AddListener(OnChooseHide);
    }

    void OnChooseWood() {
        EventGenerator.Singleton.RaiseLoseWoodEvent(woodAmount);
        ClosePopup();
    }

    void OnChooseHide() {
        EventGenerator.Singleton.RaiseLoseHideEvent(hideAmount);
        ClosePopup();
    }

    void ClosePopup() {
        EventGenerator.Singleton.RaiseEnableMainUIEvent();
        Destroy(gameObject);
    }

    public void Initialize(ResourceCost resourceCost, ActionAssignment actionAssignment) {
        switch(resourceCost) {
            case ResourceCost.TwoWoodOrHide:
                woodAmount = 2;
                hideAmount = 1;
                break;
            case ResourceCost.ThreeWoodOrTwoHide:
                woodAmount = 3;
                hideAmount = 2;
                break;
            case ResourceCost.FourWoodOrThreeHide:
                woodAmount = 4;
                hideAmount = 3;
                break;
            default:
                Debug.LogError($"Unable to process cost {resourceCost}.");
                break;
        }

        switch (actionAssignment.Type) {
            case ActionType.BuildShelter:
                instruction.text = "Build shelter with wood or hide?";
                break;
            case ActionType.BuildRoof:
                instruction.text = "Build roof with wood or hide?";
                break;
            case ActionType.BuildPalisade:
                instruction.text = "Build palisade with wood or hide?";
                break;
            case ActionType.BuildInvention:
                instruction.text = "Build " + actionAssignment.inventionCard.invention.ToString() + " with wood or hide?";
                break;
            case ActionType.Threat:
                string threatName = actionAssignment.eventCard.threatName;
                instruction.text = "Spend wood or hide to resolve " + threatName + "?";
                break;
            default:
                Debug.LogError($"Unable to process action type {actionAssignment.Type}.");
                break;
        }
    }

}
