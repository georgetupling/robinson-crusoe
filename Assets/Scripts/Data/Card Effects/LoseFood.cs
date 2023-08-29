using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseFood : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseFoodEvent(1);
    }
}
