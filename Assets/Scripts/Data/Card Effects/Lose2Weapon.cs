using UnityEngine;

public class Lose2Weapon : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Lose2Weapon effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseWeaponEvent(2);
        hasBeenApplied = true;
    }
}
