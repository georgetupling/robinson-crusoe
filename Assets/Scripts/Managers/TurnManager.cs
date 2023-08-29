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

    private int currentTurn = 0;
    private Phase currentPhase = Phase.Event;

    private int animationsInProgress;
    private int lastTurn;

    private bool skipProductionPhase;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            return;
        }
        EventGenerator.Singleton.AddListenerToEndPhaseEvent(OnEndPhaseEvent);
        EventGenerator.Singleton.AddListenerToAnimationInProgressEvent(OnAnimationInProgressEvent);
        EventGenerator.Singleton.AddListenerToGetPhaseEvent(OnGetPhaseEvent);
        EventGenerator.Singleton.AddListenerToSkipProductionPhaseEvent(OnSkipProductionPhaseEvent);
    }

    void Start()
    {
        EventGenerator.Singleton.RaiseGainDeterminationEvent(DeterminationEvent.AllPlayers, 2);
        switch (GameSettings.CurrentScenario)
        {
            case Scenario.Castaways: lastTurn = 11; break;
            default: Debug.LogError($"No turn limit set for {GameSettings.CurrentScenario} scenario."); break;
        }
        StartNextPhase();
    }

    void OnEndPhaseEvent(Phase phase)
    {
        if (phase != currentPhase)
        {
            Debug.LogError("Phase parameter passed to EndPhaseEvent does not match current phase.");
            return;
        }
        if (currentTurn == lastTurn && currentPhase == Phase.Night)
        {
            EventGenerator.Singleton.RaiseGameEndEvent(false); // The players lose the game
        }
        StartCoroutine(WaitForAnimationsThenEndPhase());
    }

    void OnAnimationInProgressEvent(bool isInProgress)
    {
        if (isInProgress)
        {
            animationsInProgress++;
        }
        else
        {
            animationsInProgress--;
        }
    }

    void OnGetPhaseEvent()
    {
        EventGenerator.Singleton.RaiseGetPhaseResponseEvent(currentPhase);
    }

    void OnSkipProductionPhaseEvent()
    {
        skipProductionPhase = true;
    }

    IEnumerator WaitForAnimationsThenEndPhase()
    {
        while (animationsInProgress > 0)
        {
            yield return null;
        }
        StartNextPhase();
    }

    void StartNextPhase()
    {
        currentPhase = GetNextPhase();
        if (currentPhase == Phase.Production && skipProductionPhase)
        {
            currentPhase = GetNextPhase();
            skipProductionPhase = false;
        }
        EventGenerator.Singleton.RaisePhaseStartEvent(currentPhase);
        if (currentPhase == Phase.Event)
        {
            currentTurn++;
            EventGenerator.Singleton.RaiseTurnStartEvent(currentTurn);
            EventGenerator.Singleton.RaiseSetTurnTrackerEvent(currentTurn);
        }
    }

    Phase GetNextPhase()
    {
        int currentIndex = Array.IndexOf(phaseOrder, currentPhase);
        int nextIndex = (currentIndex + 1) % phaseOrder.Length;
        return phaseOrder[nextIndex];
    }

}
