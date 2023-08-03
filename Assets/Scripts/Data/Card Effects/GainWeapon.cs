using UnityEngine;
public class GainWeapon : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGainWeaponEvent(1);
    }
}
