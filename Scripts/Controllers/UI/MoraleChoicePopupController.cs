using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoraleChoicePopupController : MonoBehaviour
{
    [SerializeField] private Button healthButton;
    [SerializeField] private Button determinationButton;

    void Awake() {
        SetUpButtons();
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
    }

    void SetUpButtons() {
        healthButton.onClick.AddListener(OnChooseHealth);
        determinationButton.onClick.AddListener(OnChooseDetermination);
    }

    void OnChooseHealth() {
        EventGenerator.Singleton.RaiseGainHealthEvent(HealthEvent.FirstPlayer, 1);
        ClosePopup();
    }

    void OnChooseDetermination() {
        EventGenerator.Singleton.RaiseGainDeterminationEvent(DeterminationEvent.FirstPlayer, 2);
        ClosePopup();
    }

    void ClosePopup() {
        EventGenerator.Singleton.RaiseEnableMainUIEvent();
        EventGenerator.Singleton.RaiseEndPhaseEvent(Phase.Morale);
        Destroy(gameObject);
    }

}
