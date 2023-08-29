using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class disables the action pawn it is attached to until the end of the turn when it recieves
// an event telling it the pawn's owner only has 1 action this turn.

public class ActionPawnDisabler : MonoBehaviour
{
    int playerId;
    int firstPlayer;

    void Awake()
    {
        EventGenerator.Singleton.AddListenerToPlayerHasOnly1ActionThisTurnEvent(OnPlayerHasOnly1ActionThisTurnEvent);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
        EventGenerator.Singleton.AddListenerToGetFirstPlayerEvent(OnGetFirstPlayerEvent);
    }

    void OnPlayerHasOnly1ActionThisTurnEvent(int playerId)
    {
        // player ID 6 = first player
        if (playerId == 6)
        {
            EventGenerator.Singleton.RaiseGetFirstPlayerEvent();
        }
        if (playerId == this.playerId || (playerId == 6 && firstPlayer == this.playerId))
        {
            gameObject.SetActive(false);
        }
    }

    void OnTurnStartEvent(int turnStarted)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    void OnGetFirstPlayerEvent(string eventType, int firstPlayer)
    {
        if (eventType == GetFirstPlayerEvent.Response)
        {
            this.firstPlayer = firstPlayer;
        }
    }

    public void SetPlayerId(int playerId)
    {
        this.playerId = playerId;
    }
}
