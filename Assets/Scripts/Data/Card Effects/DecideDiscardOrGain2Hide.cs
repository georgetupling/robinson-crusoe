using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideDiscardOrGain2Hide : CardEffect
{
    public override void ApplyEffect()
    {
        if (optionChosen == -1)
        {
            Debug.LogError("DecideDiscardOrGain2Hide optionChosen not set.");
            return;
        }
        // 0 = discard card, 1 = gain 2 hide
        if (optionChosen == 1)
        {
            EventGenerator.Singleton.RaiseGainHideEvent(2);
        }
        hasBeenApplied = true;
    }
}
