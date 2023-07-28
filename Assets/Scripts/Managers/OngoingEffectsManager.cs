using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static OngoingCardEffect;
using static System.Type;

public class OngoingEffectsManager : MonoBehaviour
{
    public static OngoingEffectsManager Singleton { get; private set; }

    private List<OngoingCardEffect> activeEffects = new List<OngoingCardEffect>();
    private List<OngoingCardEffect> expiredEffects = new List<OngoingCardEffect>();

    // Information to pass to new events
    bool medicineBuilt;

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }

    void Start() {
        EventGenerator.Singleton.AddListenerToOngoingEffectEvent(OnOngoingEffectEvent);
        EventGenerator.Singleton.AddListenerToEffectIsActiveEvent(OnEffectIsActiveEvent);
        EventGenerator.Singleton.AddListenerToEndOngoingEffectByTypeEvent(OnEndOngoingEffectByTypeEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
    }

    // Listeners

    void OnOngoingEffectEvent(string eventType, OngoingCardEffect ongoingEffect, int cardEffectId, Trigger endTrigger) {
        if (eventType == OngoingEffectEvent.StartOngoingEffect) {
            StartOngoingEffect(ongoingEffect);
        } else if (eventType == OngoingEffectEvent.EndOngoingEffect) {
            EndOngoingEffect(cardEffectId);
        } else if (eventType == OngoingEffectEvent.ApplyEndTrigger) {
            ApplyEndTrigger(endTrigger);
        }
    }

    void OnEffectIsActiveEvent(string eventType, int cardEffectId, System.Type cardEffectType, bool response) {
        if (eventType == EffectIsActiveEvent.RequestById) {
            ProcessEffectIsActiveRequest(cardEffectId);
        } else if (eventType == EffectIsActiveEvent.RequestByType) {
            ProcessEffectIsActiveRequest(cardEffectType);
        }
    }

    void OnEndOngoingEffectByTypeEvent(System.Type ongoingEffectType) {
        if (!typeof(OngoingCardEffect).IsAssignableFrom(ongoingEffectType)) {
            Debug.LogError($"{ongoingEffectType} is not a type of ongoing card effect.");
            return;
        }
        OngoingCardEffect effectToRemove = activeEffects.Find(effect => effect.GetType() == ongoingEffectType);
        if (effectToRemove != null) {
            effectToRemove.EndEffect();
            activeEffects.Remove(effectToRemove);
            expiredEffects.Add(effectToRemove);
        } else {
            Debug.LogError($"There is no ongoing effect of type {ongoingEffectType} to end.");
        }
    }

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt) {
        // This method tracks whether medicine is built
        if (invention == Invention.Medicine) {
            medicineBuilt = isBuilt;
        }
    }

    // Stores new ongoing effects in a list

    void StartOngoingEffect(OngoingCardEffect ongoingEffect) {
        if (ongoingEffect == null) {
            Debug.LogError("ongoingEffect is null. Failed to start new ongoing effect.");
            return;
        }
        ongoingEffect.SetMedicineBuilt(medicineBuilt); // Required for wound effects
        activeEffects.Add(ongoingEffect);
    }

    // Ends ongoing effects

    void EndOngoingEffect(int cardEffectId) {
        OngoingCardEffect effectToRemove = activeEffects.Find(x => x.CardEffectId == cardEffectId);
        if (effectToRemove == null) {
            Debug.LogError($"There is no ongoing effect with card effect ID {cardEffectId}.");
            return;
        }
        effectToRemove.EndEffect();
        activeEffects.Remove(effectToRemove);
        expiredEffects.Add(effectToRemove);
    }

    void ApplyEndTrigger(Trigger endTrigger) {
        List<OngoingCardEffect> effectsToRemove = new List<OngoingCardEffect>();
        foreach (OngoingCardEffect effect in activeEffects) {
            if (effect.endTrigger == endTrigger) {
                effectsToRemove.Add(effect);
            }
        }
        foreach (OngoingCardEffect effect in effectsToRemove) {
            effect.EndEffect();
            activeEffects.Remove(effect);
            expiredEffects.Add(effect);
        }
    }

    void ProcessEffectIsActiveRequest(int cardEffectId) {
        bool response = false;
        foreach (OngoingCardEffect effect in activeEffects) {
            if (effect.CardEffectId == cardEffectId) {
                response = true;
                break;
            }
        }
        EventGenerator.Singleton.RaiseEffectIsActiveResponseEvent(cardEffectId, response);
    }

    void ProcessEffectIsActiveRequest(System.Type cardEffectType) {
        if (!cardEffectType.IsSubclassOf(typeof(OngoingCardEffect)) && !(cardEffectType == typeof(OngoingCardEffect))) {
            Debug.LogError($"{cardEffectType} is not a type of ongoing card effect.");
            return;
        }
        bool response = false;
        foreach (OngoingCardEffect effect in activeEffects) {
            if (effect.GetType() == cardEffectType) {
                response = true;
                break;
            }
        }
        EventGenerator.Singleton.RaiseEffectIsActiveResponseEvent(cardEffectType, response);
    }
}
