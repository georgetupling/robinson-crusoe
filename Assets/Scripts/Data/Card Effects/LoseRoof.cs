using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseRoof : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseRoofEvent(1);
    }
}
