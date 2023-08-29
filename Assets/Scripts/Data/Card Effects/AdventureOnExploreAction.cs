using UnityEngine;
public class AdventureOnExploreAction : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseSpawnTokenOnDeckEvent(Deck.ExploreAdventure, TokenType.ExploreAdventure);
    }
}
