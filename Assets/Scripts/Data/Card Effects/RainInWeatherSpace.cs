using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainInWeatherSpace : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("RainInWeatherSpace effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnTokenInWeatherAreaEvent(TokenType.RainCloud);
        hasBeenApplied = true;
    }
}
