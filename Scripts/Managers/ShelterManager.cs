using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterManager : MonoBehaviour
{
    private static ShelterManager Singleton;

    private bool shelterIsBuilt = false;
    private int roofLevel = 0;
    private int palisadeLevel = 0;

    private const int maximumRoof = 4;
    private const int maximumPalisade = 4;

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }

    void Start() {
        EventGenerator.Singleton.AddListenerToShelterEvent(OnShelterEvent);
        EventGenerator.Singleton.AddListenerToRoofEvent(OnRoofEvent);
        EventGenerator.Singleton.AddListenerToPalisadeEvent(OnPalisadeEvent);
        EventGenerator.Singleton.AddListenerToGetRoofLevelEvent(OnGetRoofLevelEvent);
        EventGenerator.Singleton.AddListenerToShelterIsBuiltEvent(OnShelterIsBuiltEvent);
    }

    void OnShelterEvent(string eventType) {
        if (eventType == ShelterEvent.LoseShelter) {
            LoseShelter();
        } else if (eventType == ShelterEvent.GainShelter) {
            GainShelter();
        }
    }

    void OnRoofEvent(string eventType, int amount) {
        if (eventType == RoofEvent.GainRoof) {
            GainRoof(amount);
        } else if (eventType == RoofEvent.LoseRoof) {
            LoseRoof(amount);
        } else if (eventType == RoofEvent.LoseHalfRoof) {
            LoseHalfRoof();
        }
    }

    void OnPalisadeEvent(string eventType, int amount) {
        if (eventType == PalisadeEvent.GainPalisade) {
            GainPalisade(amount);
        } else if (eventType == PalisadeEvent.LosePalisade) {
            LosePalisade(amount);
        } else if (eventType == PalisadeEvent.LoseHalfPalisade) {
            LoseHalfPalisade();
        }
    }

    void OnGetRoofLevelEvent() {
        EventGenerator.Singleton.RaiseGetRoofLevelResponseEvent(roofLevel);
    }

    void OnShelterIsBuiltEvent() {
        EventGenerator.Singleton.RaiseShelterIsBuiltResponseEvent(shelterIsBuilt);
    }

    void LoseShelter() {
        if (!shelterIsBuilt) {
            Debug.LogError("Shelter is not currently built.");
            return;
        }
        EventGenerator.Singleton.RaiseTurnCampTokenFaceUpEvent();
        shelterIsBuilt = false;
    }

    void GainShelter() {
        if (shelterIsBuilt) {
            Debug.LogError("Shelter is already built.");
            return;
        }
        EventGenerator.Singleton.RaiseTurnCampTokenFaceDownEvent();
        shelterIsBuilt = true;
    }

    void GainPalisade(int amount) {
        int newPalisadeLevel = palisadeLevel += amount;
        if (newPalisadeLevel < 0) {
            EventGenerator.Singleton.RaiseAllLoseHealthEvent(-newPalisadeLevel);
            newPalisadeLevel = 0;
        } else if (newPalisadeLevel > maximumPalisade) {
            newPalisadeLevel = maximumPalisade;
        }
        palisadeLevel = newPalisadeLevel;
        EventGenerator.Singleton.RaiseSetPalisadeTrackerEvent(palisadeLevel);
    }

    void LosePalisade(int amount) {
        GainPalisade(-amount);
    }

    void LoseHalfPalisade() {
        int amount = palisadeLevel % 2 == 0 ? palisadeLevel / 2 : palisadeLevel / 2 + 1; // Calculates half rounded up
        GainPalisade(-amount);
    }

    void GainRoof(int amount) {
        int newRoofLevel = roofLevel += amount;
        if (newRoofLevel < 0) {
            EventGenerator.Singleton.RaiseAllLoseHealthEvent(-newRoofLevel);
            newRoofLevel = 0;
        } else if (newRoofLevel > maximumRoof) {
            newRoofLevel = maximumRoof;
        }
        roofLevel = newRoofLevel;
        EventGenerator.Singleton.RaiseSetRoofTrackerEvent(roofLevel);
    }

    void LoseRoof(int amount) {
        GainRoof(-amount);
    }

    void LoseHalfRoof() {
        int amount = roofLevel % 2 == 0 ? roofLevel / 2 : roofLevel / 2 + 1; // Calculates half rounded up
        GainRoof(-amount);
    }
}
