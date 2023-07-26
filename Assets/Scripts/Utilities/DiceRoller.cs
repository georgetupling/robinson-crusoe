using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class raises an event to spawn the dice popup and saves information relating to the roll.
    It then listens for dice rolled events and actions them.
    The ActionResolver additionally listens for whether or not build/gather/explore rolls were successful.
*/

public class DiceRoller : MonoBehaviour
{
    public static DiceRoller Singleton;

    // Information about the current roll

    int playerId;
    int islandTileId;
    int locationId;

    // Dice Lists

    List<DieType> buildDice = new List<DieType> { DieType.BuildSuccess, DieType.BuildAdventure, DieType.BuildDamage };
    List<DieType> gatherDice = new List<DieType> { DieType.GatherSuccess, DieType.GatherAdventure, DieType.GatherDamage };
    List<DieType> exploreDice = new List<DieType> { DieType.ExploreSuccess, DieType.ExploreAdventure, DieType.ExploreDamage };

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
        } else {
            Debug.LogError("Scene contains duplicate DiceRoller.");
        }
        EventGenerator.Singleton.AddListenerToDieRolledEvent(OnDieRolledEvent);
    }

    // Listens for dice rolls and actions them

    void OnDieRolledEvent(DieType dieType, int faceRolled) {
        switch (dieType) {
            case DieType.BuildSuccess:
                if (faceRolled < 2) {
                    EventGenerator.Singleton.RaiseGainDeterminationEvent(playerId, 2);
                }
                break;
            case DieType.BuildAdventure:
                if (faceRolled < 3) {
                    EventGenerator.Singleton.RaiseDrawAdventureCardEvent(AdventureType.Build);
                }
                break;
            case DieType.BuildDamage:
                if (faceRolled < 4) {
                    EventGenerator.Singleton.RaiseLoseHealthEvent(playerId, 1);
                }
                break;
            case DieType.GatherSuccess:
                if (faceRolled < 1) {
                    EventGenerator.Singleton.RaiseGainDeterminationEvent(playerId, 2);
                }
                break;
            case DieType.GatherAdventure:
                if (faceRolled < 3) {
                    EventGenerator.Singleton.RaiseDrawAdventureCardEvent(AdventureType.Gather);
                }
                break;
            case DieType.GatherDamage:
                if (faceRolled < 1) {
                    EventGenerator.Singleton.RaiseLoseHealthEvent(playerId, 1);
                }
                break;
            case DieType.ExploreSuccess:
                if (faceRolled < 1) {
                    EventGenerator.Singleton.RaiseGainDeterminationEvent(playerId, 2);
                }
                break;
            case DieType.ExploreAdventure:
                if (faceRolled < 5) {
                    // TODO: raise an event to draw an adventure card
                }
                break;
            case DieType.ExploreDamage:
                if (faceRolled < 3) {
                    EventGenerator.Singleton.RaiseLoseHealthEvent(playerId, 1);
                }
                break;
        }
    }

    // Public methods

    public void RollBuildDice(int playerId) {
        this.playerId = playerId;
        EventGenerator.Singleton.RaiseSpawnDicePopupEvent(buildDice);
    }

    public void RollGatherDice(int playerId, int islandTileId) {
        this.playerId = playerId;
        this.islandTileId = islandTileId;
        EventGenerator.Singleton.RaiseSpawnDicePopupEvent(gatherDice);
    }

    public void RollExploreDice(int playerId, int locationId) {
        this.playerId = playerId;
        this.locationId = locationId;
        EventGenerator.Singleton.RaiseSpawnDicePopupEvent(exploreDice);
    }
}
