using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideDiscardFoodOrShuffleIntoEventDeck : CardEffect
{
    public override void ApplyEffect()
    {
        if (optionChosen == -1)
        {
            Debug.LogError("DecideFoodOrShuffleIntoEventDeck optionChosen not set.");
            return;
        }
        // 0 = discard food, 1 = nothing happens (and the card egts shuffled into the event deck)
        if (optionChosen == 0)
        {
            EventGenerator.Singleton.RaiseLoseFoodEvent(1);
        }
    }
}
