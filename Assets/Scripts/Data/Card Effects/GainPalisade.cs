using UnityEngine;
public class GainPalisade : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainPalisadeEvent(1);
    }
}
