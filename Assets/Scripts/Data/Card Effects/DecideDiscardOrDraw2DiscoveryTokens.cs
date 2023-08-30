using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideDiscardOrDraw2DiscoveryTokens : CardEffect
{
    public override void ApplyEffect()
    {
        if (optionChosen == -1)
        {
            Debug.LogError("DecideDiscardOrDraw2DiscoveryTokens optionChosen not set.");
            return;
        }
        // 0 = discard card, 1 = draw 2 discovery tokens
        if (optionChosen == 1)
        {
            EventGenerator.Singleton.RaiseDrawDiscoveryTokenEvent(2);
        }
    }
}
