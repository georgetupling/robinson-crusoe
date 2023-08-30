using UnityEngine;

public class DrawInventionCard : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Invention);
    }
}
