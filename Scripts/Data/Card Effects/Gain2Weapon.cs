using UnityEngine;

public class Gain2Weapon : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Gain2Weapon effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainWeaponEvent(2);
        hasBeenApplied = true;
    }
}
