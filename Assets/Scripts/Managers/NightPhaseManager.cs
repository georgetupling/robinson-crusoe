using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightPhaseManager : MonoBehaviour
{
    static NightPhaseManager singleton;

    [SerializeField] Transform popupArea;

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

    // Flags for night phase events
    bool potBuilt;
    bool fireplaceBuilt;
    
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
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventions);
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

    void OnUpdateBuiltInventions(Invention invention, bool isBuilt) {
        if (invention == Invention.Pot) {
            potBuilt = isBuilt;
        } else if (invention == Invention.Fireplace) {
            fireplaceBuilt = isBuilt;
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
            EventGenerator.Singleton.RaiseLoseFoodEvent(foodAvailable);
            EventGenerator.Singleton.RaiseLoseNonPerishableFoodEvent(nonPerishableFoodAvailable);
        }
        
        // Asks if the players want to move camp
        yield return new WaitForSeconds(1f);

        isWaitingOnMoveCampInput = true;
        EventGenerator.Singleton.RaiseGetIslandTileInputEvent(true, InputType.MoveCamp);
        while (isWaitingOnMoveCampInput) {
            yield return null;
        }
        if (campIsMoving) {
            EventGenerator.Singleton.RaiseSpawnIslandTileTokenEvent(TokenType.Camp, locationId);
            EventGenerator.Singleton.RaiseLoseHalfRoofEvent();
            EventGenerator.Singleton.RaiseLoseHalfPalisadeEvent();
        }

        // Asks if the players want to use the pot
        EventGenerator.Singleton.RaiseGetFoodEvent();
        while (isWaitingOnFood) {
            yield return null;
        }
        if (potBuilt && foodAvailable > 0) {
            yield return new WaitForSeconds(1f);
            EventGenerator.Singleton.RaiseSpawnItemActivationPopupEvent(Invention.Pot);
            while (popupArea.childCount > 0) {
                yield return null;
            }
        }

        // Asks if the players want to use the fireplace
        EventGenerator.Singleton.RaiseGetFoodEvent();
        while (isWaitingOnFood) {
            yield return null;
        }
        if (fireplaceBuilt && foodAvailable > 0) {
            yield return new WaitForSeconds(1f);
            EventGenerator.Singleton.RaiseSpawnItemActivationPopupEvent(Invention.Fireplace);
            while (popupArea.childCount > 0) {
                yield return null;
            }
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
