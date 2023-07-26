using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RoofEvent : UnityEvent<string, int>
{
    public const string GainRoof = "GainRoof";
    public const string LoseRoof = "LoseRoof";
    public const string LoseHalfRoof = "LoseHalfRoof";
}
