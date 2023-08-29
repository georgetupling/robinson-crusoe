using UnityEngine;
public class AdventureOnBuildAction : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseSpawnTokenOnDeckEvent(Deck.BuildAdventure, TokenType.BuildAdventure);
    }
}
