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
            Debug.LogError("Scene contains duplicate DiceRollHandler.");
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
                    // TODO: raise an event to draw an adventure card
                }
                break;
            case DieType.BuildDamage:
                if (faceRolled < 4) {
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

    // Legacy methods - to replace
    
    public static bool RollGatherDice(int playerId, int islandTileId) {
        Debug.Log("Rolling gather dice...");
        // Adventure die
        int randInt = Random.Range(0, 2);
        if (randInt == 0) {
            // TODO - raise an adventure event for the rolling player
        }
        // Damage die
        randInt = Random.Range(0, 6);
        if (randInt == 0) {
            EventGenerator.Singleton.RaiseLoseHealthEvent(playerId, 1);
        }
        // Success die
        randInt = Random.Range(0, 6);
        if (randInt == 0) {
            EventGenerator.Singleton.RaiseGainDeterminationEvent(playerId, 2);
        }
        return randInt > 0 ? true : false;
    }

    public static bool RollExploreDice(int playerId, int locationId) {
        Debug.Log("Rolling explore dice...");
        // Adventure die
        int randInt = Random.Range(0, 2);
        if (randInt == 0) {
            // TODO - raise an adventure event for the rolling player
        }
        // Damage die
        randInt = Random.Range(0, 6);
        if (randInt == 0) {
            EventGenerator.Singleton.RaiseLoseHealthEvent(playerId, 1);
        }
        // Success die
        randInt = Random.Range(0, 6);
        if (randInt == 0) {
            EventGenerator.Singleton.RaiseGainDeterminationEvent(playerId, 2);
        }
        return randInt > 0 ? true : false;
    }

    public static List<int> RollWeatherDice(List<TokenController> diceToRoll) {
        Debug.Log("Rolling weather dice...");
        int numberOfRainClouds = 0;
        int numberOfSnowClouds = 0;
        foreach (TokenController token in diceToRoll) {
            int randInt;
            if (token.tokenType == TokenType.RedWeatherDie) {
                randInt = Random.Range(0, 6);
                if (randInt <= 1) {
                    // Nothing happens
                } else if (randInt <= 3) {
                    EventGenerator.Singleton.RaiseLosePalisadeEvent(1);
                } else if (randInt <= 4) {
                    EventGenerator.Singleton.RaiseLoseFoodEvent(1);
                } else if (randInt <= 5) {
                    // TODO: combat with strength 3 beast
                }
            } else if (token.tokenType == TokenType.WhiteWeatherDie) {
                randInt = Random.Range(0, 6);
                if (randInt <= 1) {
                    numberOfRainClouds += 2;
                } else if (randInt <= 3) {
                    numberOfSnowClouds += 1;
                } else if (randInt <= 5) {
                    numberOfSnowClouds += 2;
                }
            } else if (token.tokenType == TokenType.OrangeWeatherDie) {
                randInt = Random.Range(0, 6);
                if (randInt <= 2) {
                    numberOfRainClouds += 1;
                } else if (randInt <= 3) {
                    numberOfSnowClouds += 1;
                } else if (randInt <= 5) {
                    numberOfRainClouds += 2;
                }
            }
        }
        return new List<int> { numberOfRainClouds, numberOfSnowClouds };
    }
}
