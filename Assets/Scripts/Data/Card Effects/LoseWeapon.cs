using UnityEngine;

public class LoseWeapon : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("LoseWeapon effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseWeaponEvent(1);
        hasBeenApplied = true;
    }
}
