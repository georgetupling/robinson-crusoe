using UnityEngine;
public class Lose2Palisade : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLosePalisadeEvent(2);
    }
}
