using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollOnBuildAction : CardEffect
{    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("RerollOnBuildAction effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnTokenOnDeckEvent(Deck.BuildAdventure, TokenType.Reroll);
        hasBeenApplied = true;
    }
}
