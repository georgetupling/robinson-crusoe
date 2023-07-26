using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionSetter : MonoBehaviour
{
    private TMP_Text instruction;

    void Start()
    {
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToSpawnMoraleChoicePopupEvent(OnSpawnMoraleChoicePopupEvent);
        instruction = GetComponent<TMP_Text>();
    }

    void OnPhaseStartEvent(Phase phase) {
        switch(phase) {
            case Phase.Event:
                instruction.text = "Resolving event phase...";
                break;
            case Phase.Morale:
                instruction.text = "Resolving morale phase...";
                break;
            case Phase.Production:
                instruction.text = "Resolving production phase...";
                break;
            case Phase.Action:
                instruction.text = "Drag and drop your action pawns to assign actions";
                break;
            case Phase.Weather:
                instruction.text = "Resolving weather phase...";
                break;
            case Phase.Night:
                instruction.text = "Resolving night phase...";
                break;
        }   
    }

    void OnSpawnMoraleChoicePopupEvent() {
        instruction.text = "Select an option";
    }
}
