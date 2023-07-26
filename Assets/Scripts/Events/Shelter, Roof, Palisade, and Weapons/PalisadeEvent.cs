using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PalisadeEvent : UnityEvent<string, int>
{
    public const string GainPalisade = "GainPalisade";
    public const string LosePalisade = "LosePalisade";
    public const string LoseHalfPalisade = "LoseHalfPalisade";
}
