using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfPossibleLoseFood : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseIfPossibleLoseResourceEvent(ResourceType.Food, 1);
    }
}
