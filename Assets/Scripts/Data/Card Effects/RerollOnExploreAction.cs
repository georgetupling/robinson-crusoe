using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollOnExploreAction : CardEffect
{    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("RerollOnExploreAction effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnTokenOnDeckEvent(Deck.ExploreAdventure, TokenType.Reroll);
        hasBeenApplied = true;
    }
}
