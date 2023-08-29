using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfPossibleLoseWood : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseIfPossibleLoseResourceEvent(ResourceType.Wood, 1);
    }
}
