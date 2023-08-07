using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalWoodOnCamp : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseSpawnTokenOnCampEvent(TokenType.AdditionalWood);
    }
}
