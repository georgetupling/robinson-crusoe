using UnityEngine;

public class ANewIdeaEffect : CardEffect
{   
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseDrawInventionCardsAndChooseOneEvent(5);
    }
}
