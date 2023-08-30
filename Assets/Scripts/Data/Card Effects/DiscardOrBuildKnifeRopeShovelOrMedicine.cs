using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardOrBuildKnifeRopeShovelOrMedicine : CardEffect
{
    public override void ApplyEffect()
    {
        if (optionChosen == -1)
        {
            Debug.LogError("DiscardOrBuildKnifeRopeShovelOrMedicine optionChosen not set.");
            return;
        }
        // 0 = discard card, 1 = knife, 2 = rope, 3 = shovel, 4 = medicine
        if (optionChosen == 1)
        {
            EventGenerator.Singleton.RaiseBuildInventionSuccessEvent(Invention.Knife);
        }
        else if (optionChosen == 2)
        {
            EventGenerator.Singleton.RaiseBuildInventionSuccessEvent(Invention.Rope);
        }
        else if (optionChosen == 3)
        {
            EventGenerator.Singleton.RaiseBuildInventionSuccessEvent(Invention.Shovel);
        }
        else if (optionChosen == 4)
        {
            EventGenerator.Singleton.RaiseBuildInventionSuccessEvent(Invention.Medicine);
        }
    }
}
