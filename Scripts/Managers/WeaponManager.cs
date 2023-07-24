using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager singleton;

    private int weaponLevel = 0;
    private const int minimumWeapon = 0;
    private const int maximumWeapon = 10;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            return;
        }
        EventGenerator.Singleton.AddListenerToGainWeaponEvent(OnGainWeaponEvent);
        EventGenerator.Singleton.AddListenerToGetWeaponLevelEvent(OnGetWeaponLevelEvent);
    }

    void OnGainWeaponEvent(int amount) {
        if (weaponLevel + amount < 0) {
            EventGenerator.Singleton.RaiseAllGainHealthEvent(weaponLevel + amount);
        }
        weaponLevel = Mathf.Clamp(weaponLevel + amount, minimumWeapon, maximumWeapon);
        EventGenerator.Singleton.RaiseSetWeaponTrackerEvent(weaponLevel);
    }

    void OnGetWeaponLevelEvent(string eventType, int response) {
        if (eventType == GetWeaponLevelEvent.Query) {
            EventGenerator.Singleton.RaiseGetWeaponLevelResponseEvent(weaponLevel);
        }
    }
}
