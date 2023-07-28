using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class disables the action pawn it is attached to until the end of the turn when it recieves
// an event telling it the pawn's owner only has 1 action this turn.

public class ActionPawnDisabler : MonoBehaviour
{
    int playerId;
    
    void Awake() {
        EventGenerator.Singleton.AddListenerToPlayerHasOnly1ActionThisTurnEvent(OnPlayerHasOnly1ActionThisTurnEvent);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
    }

    void OnPlayerHasOnly1ActionThisTurnEvent(int playerId) {
        if (playerId != this.playerId) {
            return;
        }
        gameObject.SetActive(false);
    }

    void OnTurnStartEvent(int turnStarted) {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
        }
    }

    public void SetPlayerId(int playerId) {
        this.playerId = playerId;
    }
}
