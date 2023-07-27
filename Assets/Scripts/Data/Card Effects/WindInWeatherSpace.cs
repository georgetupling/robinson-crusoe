using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindInWeatherSpace : CardEffect
{ 
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("WindInWeatherSpace effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnTokenInWeatherAreaEvent(TokenType.Storm);
        hasBeenApplied = true;
    }
}
