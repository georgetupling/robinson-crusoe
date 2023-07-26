using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeCampChoicePopupController : MonoBehaviour
{
    [SerializeField] private Button moraleButton;
    [SerializeField] private Button determinationButton;

    private int playerId;

    void Awake() {
        SetUpButtons();
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
    }

    void SetUpButtons() {
        moraleButton.onClick.AddListener(OnChooseMorale);
        determinationButton.onClick.AddListener(OnChooseDetermination);
    }

    void OnChooseMorale() {
        EventGenerator.Singleton.RaiseGainMoraleEvent(1);
        ClosePopup();
    }

    void OnChooseDetermination() {
        EventGenerator.Singleton.RaiseGainDeterminationEvent(playerId, 2);
        ClosePopup();
    }

    void ClosePopup() {
        EventGenerator.Singleton.RaiseEnableMainUIEvent();
        Destroy(gameObject);
    }

    public void Initialize(int playerId) {
        this.playerId = playerId;
    }

}
