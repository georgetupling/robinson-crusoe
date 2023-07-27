using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDieInWeatherSpace : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("RedDieInWeatherSpace effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnTokenInWeatherAreaEvent(TokenType.RedWeatherDie);
        hasBeenApplied = true;
    }
}
