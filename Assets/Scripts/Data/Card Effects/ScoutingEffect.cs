using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutingEffect : CardEffect
{   
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseScoutingEvent();
    }
}
