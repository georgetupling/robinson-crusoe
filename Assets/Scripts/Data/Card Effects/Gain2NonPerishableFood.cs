using UnityEngine;
public class Gain2NonPerishableFood : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainNonPerishableFoodEvent(2);
    }
}
