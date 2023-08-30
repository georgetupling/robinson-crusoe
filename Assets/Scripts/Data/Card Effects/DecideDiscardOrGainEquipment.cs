using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideDiscardOrGainEquipment : CardEffect
{
    public override void ApplyEffect()
    {
        if (optionChosen == -1)
        {
            Debug.LogError("DecideDiscardOrGainEquipment optionChosen not set.");
            return;
        }
        // 0 = discard card, 1 = gain equipment
        if (optionChosen == 1)
        {
            EventGenerator.Singleton.RaiseDrawEquipmentCardEvent();
        }
    }
}
