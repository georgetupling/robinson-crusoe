using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightPhaseManager : MonoBehaviour
{
    static NightPhaseManager singleton;

    bool isWaitingOnFood;
    bool isWaitingOnNonPerishableFood;
    bool isWaitingOnPopup;
    bool isWaitingOnMoveCampInput;
    bool isWaitingOnCampHasNaturalShelter;
    bool isWaitingOnShelterIsBuilt;

    int foodAvailable;
    int nonPerishableFoodAvailable;

    List<int> playersEating;

    bool campIsMoving;
    int locationId; // Where the camp is moving to

    bool campHasNaturalShelter;
    bool shelterIsBuilt;
    
    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            return;
        }
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToGetResourceEvent(OnGetResourceEvent);
        EventGenerator.Singleton.AddListenerToPlayersEatingEvent(OnPlayersEatingEvent);
        EventGenerator.Singleton.AddListenerToAdjacentTileChosenEvent(OnAdjacentTileChosenEvent);
        EventGenerator.Singleton.AddListenerToCampHasNaturalShelterResponseEvent(OnCampHasNaturalShelterResponseEvent);
        EventGenerator.Singleton.AddListenerToShelterIsBuiltResponseEvent(OnShelterIsBuiltResponseEvent);
    }

    // Listeners

    void OnPhaseStartEvent(Phase phase) {
        if (phase != Phase.Night) {
            return;
        }
        StartCoroutine(ApplyNightPhase());
    }

    void OnAdjacentTileChosenEvent(bool campIsMoving, int locationId) {
        if (isWaitingOnMoveCampInput) {
            this.campIsMoving = campIsMoving;
            this.locationId = locationId;
            isWaitingOnMoveCampInput = false;
        }
    }

    void OnGetResourceEvent(string eventType, int response) {
        if (eventType == GetResourceEvent.GetFoodResponse && isWaitingOnFood) {
            foodAvailable = response;
            isWaitingOnFood = false;
        } else if (eventType == GetResourceEvent.GetNonPerishableFoodResponse && isWaitingOnNonPerishableFood) {
            nonPerishableFoodAvailable = response;
            isWaitingOnNonPerishableFood = false;
        }
    }

    void OnPlayersEatingEvent(List<int> playersEating) {
        this.playersEating = playersEating;
        isWaitingOnPopup = false;
    }

    void OnCampHasNaturalShelterResponseEvent(bool campHasNaturalShelter) {
        if (isWaitingOnCampHasNaturalShelter) {
            this.campHasNaturalShelter = campHasNaturalShelter;
            isWaitingOnCampHasNaturalShelter = false;
        }
    }

    void OnShelterIsBuiltResponseEvent(bool shelterIsBuilt) {
        if (isWaitingOnShelterIsBuilt) {
            this.shelterIsBuilt = shelterIsBuilt;
            isWaitingOnShelterIsBuilt = false;
        }
    }

    // The main night phase method

    IEnumerator ApplyNightPhase() {
        isWaitingOnFood = true;
        isWaitingOnNonPerishableFood = true;
        EventGenerator.Singleton.RaiseGetFoodEvent();
        EventGenerator.Singleton.RaiseGetNonPerishableFoodEvent();
        while (isWaitingOnFood || isWaitingOnNonPerishableFood) {
            yield return null;
        }
        int foodRequired = GameSettings.PlayerCount;
        if (foodAvailable == 0 && nonPerishableFoodAvailable == 0) {
            EventGenerator.Singleton.RaiseLoseHealthEvent(HealthEvent.AllPlayers, 2);
        } else if (foodAvailable + nonPerishableFoodAvailable >= foodRequired) {
            if (foodAvailable >= foodRequired) {
                EventGenerator.Singleton.RaiseLoseFoodEvent(foodRequired);
            } else {
                EventGenerator.Singleton.RaiseLoseFoodEvent(foodAvailable);
                EventGenerator.Singleton.RaiseLoseNonPerishableFoodEvent(foodRequired - foodAvailable);
            }
        } else {
            isWaitingOnPopup = true;
            EventGenerator.Singleton.RaiseSpawnNightPhasePopupEvent(foodAvailable + nonPerishableFoodAvailable);
            while (isWaitingOnPopup) {
                yield return null;
            }
            List<int> playersGoingHungry = new List<int>();
            for (int i = 0; i < GameSettings.PlayerCount; i++) {
                if (!playersEating.Contains(i)) {
                    playersGoingHungry.Add(i);
                }
            }
            foreach (int playerId in playersGoingHungry) {
                EventGenerator.Singleton.RaiseLoseHealthEvent(playerId, 2);
            }
        }
        
        // Asks if the players want to move camp
        yield return new WaitForSeconds(1f);

        isWaitingOnMoveCampInput = true;
        EventGenerator.Singleton.RaiseChooseAdjacentTileEvent(true);
        while (isWaitingOnMoveCampInput) {
            yield return null;
        }
        if (campIsMoving) {
            EventGenerator.Singleton.RaiseSpawnIslandTileTokenEvent(TokenType.Camp, locationId);
            EventGenerator.Singleton.RaiseLoseHalfRoofEvent();
            EventGenerator.Singleton.RaiseLoseHalfPalisadeEvent();
        }

        // Players take damage if they are sleeping outside
        yield return new WaitForSeconds(1f);

        isWaitingOnCampHasNaturalShelter = true;
        EventGenerator.Singleton.RaiseCampHasNaturalShelterEvent();
        while (isWaitingOnCampHasNaturalShelter) {
            yield return null;
        }

        isWaitingOnShelterIsBuilt = true;
        EventGenerator.Singleton.RaiseShelterIsBuiltEvent();
        while (isWaitingOnShelterIsBuilt) {
            yield return null;
        }

        if (!campHasNaturalShelter && !shelterIsBuilt) {
            EventGenerator.Singleton.RaiseLoseHealthEvent(HealthEvent.AllPlayers, 1);
        }
        
        // TODO: check for effects that make a player sleep outside camp

        yield return new WaitForSeconds(1f);
        EventGenerator.Singleton.RaiseEndPhaseEvent(Phase.Night);

    }
}
