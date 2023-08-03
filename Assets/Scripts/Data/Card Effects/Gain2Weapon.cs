using UnityEngine;

public class Gain2Weapon : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainWeaponEvent(2);
    }
}
