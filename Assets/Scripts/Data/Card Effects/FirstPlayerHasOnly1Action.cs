using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPlayerHasOnly1Action : CardEffect
{
    public override void ApplyEffect()
    {
        int firstPlayerId = 6;
        EventGenerator.Singleton.RaisePlayerHasOnly1ActionThisTurnEvent(firstPlayerId);
    }
}
