using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TrackerTokenEvent : UnityEvent<string, int>
{
    public const string SetMoraleTracker = "SetMoraleTracker";
    public const string SetPalisadeTracker = "SetPalisadeTracker";
    public const string SetRoofTracker = "SetRoofTracker";
    public const string SetWeaponTracker = "SetWeaponTracker";
    public const string SetTurnTracker = "SetTurnTracker";
    public const string SetWoodpileTracker = "SetWoodpileTracker";
    public const string SetPlayer0HealthTracker = "SetPlayer0HealthTracker";
    public const string SetPlayer1HealthTracker = "SetPlayer1HealthTracker";
    public const string SetPlayer2HealthTracker = "SetPlayer2HealthTracker";
    public const string SetPlayer3HealthTracker = "SetPlayer3HealthTracker";
}
