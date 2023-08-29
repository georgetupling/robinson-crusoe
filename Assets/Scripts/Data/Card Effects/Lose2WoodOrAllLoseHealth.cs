using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lose2WoodOrAllLoseHealth : CardEffect
{
    public override void ApplyEffect()
    {
        if (optionChosen == -1)
        {
            Debug.LogError("Lose2WoodOrAllLoseHealth optionChosen not set.");
            return;
        }
        // 0 = lose 2 wood, 1 = all love health
        if (optionChosen == 0)
        {
            EventGenerator.Singleton.RaiseLoseWoodEvent(2);
        }
        else if (optionChosen == 1)
        {
            EventGenerator.Singleton.RaiseLoseHealthEvent(HealthEvent.AllPlayers, 1);
        }
    }
}
