using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ShelterEvent : UnityEvent<string>
{
    public const string LoseShelter = "LoseShelter";
    public const string GainShelter = "GainShelter";
}
