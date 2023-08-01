using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    /*
    Turns are managed as follows:
        - Raising an EndPhaseEvent and passing the current phase tells the turn manager to move to the next phase.
        - The TurnManager checks whether the phase end conditions are met before moving to the next phase.
        - The turn manager raises a PhaseStartEvent at the start of a new phase.
        - The turn manager raises a TurnStartEvent at the start of a new turn.
        - Other classes listen for these events and take appropriate actions to manage the flow of the game (e.g. the MoraleManager controls the morale phase).
    */
    
    private static TurnManager singleton;

    private Phase[] phaseOrder = { Phase.Event, Phase.Morale, Phase.Production, Phase.Action, Phase.Weather, Phase.Night };

    private int currentTurn;
    private Phase currentPhase;

    private int animationsInProgress;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            return;
        }
        currentTurn = 1;
        currentPhase = Phase.Event;
        EventGenerator.Singleton.AddListenerToEndPhaseEvent(OnEndPhaseEvent);
        EventGenerator.Singleton.AddListenerToAnimationInProgressEvent(OnAnimationInProgressEvent);
        EventGenerator.Singleton.AddListenerToGetPhaseEvent(OnGetPhaseEvent);
    }

    void Start() {
        EventGenerator.Singleton.RaiseGainDeterminationEvent(DeterminationEvent.AllPlayers, 2);
        StartCoroutine(WaitForAnimationsThenEndPhase());
    }

    void OnEndPhaseEvent(Phase phase) {
        if (phase != currentPhase) {
            Debug.LogError("Phase parameter passed to EndPhaseEvent does not match current phase.");
            return;
        }
        StartCoroutine(WaitForAnimationsThenEndPhase());
    }

    void OnAnimationInProgressEvent(bool isInProgress) {
        if (isInProgress) {
            animationsInProgress++;
        } else {
            animationsInProgress--;
        }
    }

    void OnGetPhaseEvent() {
        EventGenerator.Singleton.RaiseGetPhaseResponseEvent(currentPhase);
    }

    IEnumerator WaitForAnimationsThenEndPhase() {
        while(animationsInProgress > 0) {
            yield return null;
        }
        StartNextPhase();
    }

    void StartNextPhase() {
        currentPhase = GetNextPhase();
        EventGenerator.Singleton.RaisePhaseStartEvent(currentPhase);
        if (currentPhase == Phase.Event) {
            currentTurn++;
            EventGenerator.Singleton.RaiseTurnStartEvent(currentTurn);
        }
    }

    Phase GetNextPhase() {
        int currentIndex = Array.IndexOf(phaseOrder, currentPhase);
        int nextIndex = (currentIndex + 1) % phaseOrder.Length;
        return phaseOrder[nextIndex];
    }

}
