using UnityEngine;
public class Gain3Weapon : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainWeaponEvent(3);
    }
}
