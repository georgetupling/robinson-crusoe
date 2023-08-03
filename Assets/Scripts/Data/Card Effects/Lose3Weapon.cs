using UnityEngine;
public class Lose3Weapon : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseWeaponEvent(3);
    }
}
