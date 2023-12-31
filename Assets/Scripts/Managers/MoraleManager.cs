using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleManager : MonoBehaviour
{
    private static MoraleManager singleton;

    [SerializeField] private Transform popupArea;

    private int moraleLevel = 0;
    private int moraleToGainAtStartOfRound = 0;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        EventGenerator.Singleton.AddListenerToMoraleEvent(OnMoraleEvent);
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
        EventGenerator.Singleton.AddListenerToAtStartOfNextRoundGainMoraleEvent(OnAtStartOfNextRoundGainMoraleEvent);
    }

    void OnMoraleEvent(string eventType, int amount)
    {
        if (eventType == MoraleEvent.GainMorale)
        {
            ModifyMorale(amount);
        }
        else if (eventType == MoraleEvent.LoseMorale)
        {
            ModifyMorale(-amount);
        }
    }

    void OnPhaseStartEvent(Phase phase)
    {
        if (phase == Phase.Morale)
        {
            StartCoroutine(ApplyMoralePhase());
        }
    }

    void OnTurnStartEvent(int turnStarted)
    {
        ModifyMorale(moraleToGainAtStartOfRound);
        moraleToGainAtStartOfRound = 0;
    }

    void OnAtStartOfNextRoundGainMoraleEvent()
    {
        moraleToGainAtStartOfRound++;
    }

    void ModifyMorale(int amount)
    {
        if (moraleLevel - amount < 0)
        {
            int lostHealth = -(moraleLevel - amount);
            EventGenerator.Singleton.RaiseLoseHealthEvent(HealthEvent.AllPlayers, lostHealth);
        }
        moraleLevel = Mathf.Clamp(moraleLevel + amount, -3, 3);
        EventGenerator.Singleton.RaiseSetMoraleTrackerEvent(moraleLevel);
    }

    IEnumerator ApplyMoralePhase()
    {
        // In the solo variant, morale increases by 1 at the start of the morale phase
        if (GameSettings.PlayerCount == 1)
        {
            ModifyMorale(1);
        }
        float waitTime = GameSettings.AnimationDuration;
        yield return new WaitForSeconds(waitTime);
        if (moraleLevel == 3)
        {
            EventGenerator.Singleton.RaiseSpawnMoraleChoicePopupEvent();
        }
        else
        {
            EventGenerator.Singleton.RaiseGainDeterminationEvent(DeterminationEvent.FirstPlayer, moraleLevel);
        }
        while (popupArea.childCount > 0)
        {
            yield return null;
        }
        EventGenerator.Singleton.RaiseApplyEffectTriggerEvent(Trigger.MoralePhase);
        yield return new WaitForSeconds(1.5f);
        EventGenerator.Singleton.RaiseEndPhaseEvent(Phase.Morale);
    }

}
