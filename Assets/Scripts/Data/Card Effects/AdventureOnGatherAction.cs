using UnityEngine;
public class AdventureOnGatherAction : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseSpawnTokenOnDeckEvent(Deck.GatherAdventure, TokenType.GatherAdventure);
    }
}
