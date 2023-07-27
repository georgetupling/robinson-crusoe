using UnityEngine;

public class Gain3Weapon : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("Gain3Weapon effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainWeaponEvent(3);
        hasBeenApplied = true;
    }
}
