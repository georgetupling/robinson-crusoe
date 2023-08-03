using UnityEngine;
public class LoseWeapon : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseWeaponEvent(1);
    }
}
