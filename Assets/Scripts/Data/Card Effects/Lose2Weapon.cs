using UnityEngine;
public class Lose2Weapon : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseWeaponEvent(2);
    }
}
