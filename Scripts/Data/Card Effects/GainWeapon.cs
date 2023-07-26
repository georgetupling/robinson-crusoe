using UnityEngine;

public class GainWeapon : CardEffect
{
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("GainWeapon effect has already been applied.");
            return;
        }
        EventGenerator.Singleton.RaiseGainWeaponEvent(1);
        hasBeenApplied = true;
    }
}
