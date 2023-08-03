using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleManager : MonoBehaviour
{
    private static MoraleManager singleton;

    [SerializeField] private Transform popupArea;

    private int moraleLevel;

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
        moraleLevel = 0;
        EventGenerator.Singleton.AddListenerToMoraleEvent(OnMoraleEvent);
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
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

    void ModifyMorale(int amount)
    {
        moraleLevel = Mathf.Clamp(moraleLevel + amount, -3, 3);
        EventGenerator.Singleton.RaiseSetMoraleTrackerEvent(moraleLevel);
    }

    IEnumerator ApplyMoralePhase()
    {
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
