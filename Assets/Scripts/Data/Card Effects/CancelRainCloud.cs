using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelRainCloud : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseCancelRainCloudEvent();
    }
}
