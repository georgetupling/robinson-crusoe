using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public int id { get; private set; }
    public string playerName { get; private set; }
    public Character character { get; private set; }
    public int health { get; private set; }
    public int determination { get; private set; }


    const int FridaysPlayerId = 4;

    public Player(int playerId, string playerName, Character character)
    {
        id = playerId;
        this.playerName = playerName;
        this.character = character;
        if (playerId != FridaysPlayerId)
        {
            health = character.maximumHealth;
        }
        else
        {
            health = 4;
        }
        determination = 0;
    }

    public void ModifyHealth(int amount)
    {
        int maximumHealth;
        if (id != FridaysPlayerId)
        {
            maximumHealth = character.maximumHealth;
        }
        else
        {
            maximumHealth = 4;
        }

        int newHealth = health + amount > maximumHealth ? maximumHealth : health + amount;
        int moraleThresholdsPassed = MoraleThresholdsPassed(newHealth);
        if (moraleThresholdsPassed > 0)
        {
            EventGenerator.Singleton.RaiseLoseMoraleEvent(moraleThresholdsPassed);
        }
        health = newHealth;
        EventGenerator.Singleton.RaiseSetHealthTrackerEvent(id, health);
        if (health <= 0)
        {
            if (id != FridaysPlayerId)
            {
                EventGenerator.Singleton.RaiseGameEndEvent(false);
            }
            else
            {
                EventGenerator.Singleton.RaiseFridayDiesEvent();
            }

        }
    }

    public void ModifyDetermination(int amount)
    {
        int newDetermination = determination + amount;
        if (newDetermination < 0)
        {
            ModifyHealth(newDetermination); // Due to unfulfilled demand
            newDetermination = 0;
        }
        determination = newDetermination;
        EventGenerator.Singleton.RaiseSetDeterminationTokensEvent(id, newDetermination);
    }

    private int MoraleThresholdsPassed(int newHealth)
    {
        if (id == FridaysPlayerId)
        {
            return 0;
        }
        List<int> thresholds = character.moraleThresholds;
        int counter = 0;
        foreach (int threshold in thresholds)
        {
            if (health > threshold && newHealth <= threshold)
            {
                counter++;
            }
        }
        return counter;
    }
}
