using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryChecker : MonoBehaviour
{
    private static VictoryChecker singleton;
    
    // Castaways
    private bool woodpileCompleted;
    private bool fireBuilt;
    private int currentTurn;
    private void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            Debug.LogError("Scene contains duplicate VictoryChecker.");
            return;
        }
        EventGenerator.Singleton.AddListenerToWoodpileCompletedEvent(OnWoodpileCompletedEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
    }

    void OnWoodpileCompletedEvent() {
        woodpileCompleted = true;
        CheckForVictory();
    }
    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt) {
        if (invention == Invention.Fire) {
            fireBuilt = isBuilt;
            CheckForVictory();
        }
    }
    void OnTurnStartEvent(int turnStarted) {
        currentTurn = turnStarted;
        CheckForVictory();
    }
    void CheckForVictory() {
        switch(GameSettings.CurrentScenario) {
            case Scenario.Castaways:
                if (woodpileCompleted && fireBuilt && currentTurn >= 9) {
                    EventGenerator.Singleton.RaiseGameEndEvent(true);
                }
                break;
            default:
                Debug.LogError($"No victory condition specified for {GameSettings.CurrentScenario} scenario.");
                break;
        }
    }
}
