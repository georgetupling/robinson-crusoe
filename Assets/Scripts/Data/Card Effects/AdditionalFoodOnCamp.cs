using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalFoodOnCamp : CardEffect
{   
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseSpawnTokenOnCampEvent(TokenType.AdditionalFood);
    }
}
