using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfPossibleDiscardBeastCardFromHuntingDeck : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseIfPossibleDiscardBeastCardFromHuntingDeckEvent();
    }
}
