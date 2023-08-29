using UnityEngine;
public class IfPossibleLose2Weapon : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseIfPossibleLoseWeaponEvent(2);
    }
}
