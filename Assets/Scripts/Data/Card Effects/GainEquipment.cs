using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainEquipment : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseDrawEquipmentCardEvent();
    }
}
