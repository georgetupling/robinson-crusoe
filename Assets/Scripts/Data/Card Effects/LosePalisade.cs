using UnityEngine;
public class LosePalisade : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLosePalisadeEvent(1);
    }
}
