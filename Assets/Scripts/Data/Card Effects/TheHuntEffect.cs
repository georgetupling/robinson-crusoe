using UnityEngine;

public class TheHuntEffect : CardEffect
{
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Beast);
    }
}
