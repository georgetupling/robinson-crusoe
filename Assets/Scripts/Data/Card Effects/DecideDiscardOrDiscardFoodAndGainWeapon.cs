using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideDiscardOrDiscardFoodAndGainWeapon : CardEffect
{
    public override void ApplyEffect()
    {
        if (optionChosen == -1)
        {
            Debug.LogError("DecideDiscardOrDiscardFoodAndGainWeapon optionChosen not set.");
            return;
        }
        // 0 = discard card, 1 = discard food and gain weapon
        if (optionChosen == 1)
        {
            EventGenerator.Singleton.RaiseLoseFoodEvent(1);
            EventGenerator.Singleton.RaiseGainWeaponEvent(1);
        }
    }
}
