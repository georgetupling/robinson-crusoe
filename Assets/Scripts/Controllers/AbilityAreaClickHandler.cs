using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAreaClickHandler : MonoBehaviour
{
    int playerId;
    int determination;
    Ability ability;

    void Awake() {
        EventGenerator.Singleton.AddListenerToGetDeterminationResponseEvent(OnGetDeterminationResponseEvent);
    }

    void OnGetDeterminationResponseEvent(int playerId, int determination) {
        if (playerId != this.playerId) {
            return;
        }
        this.determination = determination;
    }

    void OnMouseDown() {
        if (ability == null) {
            Debug.LogError("Ability is null.");
            return;
        }
        EventGenerator.Singleton.RaiseGetDeterminationEvent(playerId);
        if (determination < ability.determinationCost) {
            Debug.Log("Insufficent determination to activate ability.");
            return;
        }
        Debug.Log("Activating ability...");
        // EventGenerator.Singleton.RaiseSpawnAbilityPopupEvent(playerId, ability);
    }

    public void Initialize(int playerId, Ability ability) {
        this.ability = ability;
    }
}
