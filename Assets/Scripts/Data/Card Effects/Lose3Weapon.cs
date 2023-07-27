using UnityEngine;

public class Lose3Weapon : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Lose3Weapon effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseLoseWeaponEvent(3);
        hasBeenApplied = true;
    }
}
