using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollOnGatherAction : CardEffect
{    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("RerollOnGatherAction effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnTokenOnDeckEvent(Deck.GatherAdventure, TokenType.Reroll);
        hasBeenApplied = true;
    }
}
