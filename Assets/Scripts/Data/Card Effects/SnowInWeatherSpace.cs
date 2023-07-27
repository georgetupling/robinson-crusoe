using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowInWeatherSpace : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("SnowInWeatherSpace effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnTokenInWeatherAreaEvent(TokenType.SnowCloud);
        hasBeenApplied = true;
    }
}
