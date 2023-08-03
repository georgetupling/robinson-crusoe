using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SackEffect : CardEffect
{   
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseAdditionalResourceFromGatherEvent(Invention.Sack);
    }
}
