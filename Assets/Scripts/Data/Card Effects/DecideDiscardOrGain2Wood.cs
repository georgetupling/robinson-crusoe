using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideDiscardOrGain2Wood : CardEffect
{
    public override void ApplyEffect()
    {
        if (optionChosen == -1)
        {
            Debug.LogError("DecideDiscardOrGain2Wood optionChosen not set.");
            return;
        }
        // 0 = discard card, 1 = gain 2 wood
        if (optionChosen == 1)
        {
            EventGenerator.Singleton.RaiseGainWoodEvent(2);
        }
        hasBeenApplied = true;
    }
}
