using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketEffect : CardEffect
{   
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseAdditionalResourceFromGatherEvent(Invention.Basket);
    }
}
