using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseAllResources : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseAllResourcesEvent();
    }
}
