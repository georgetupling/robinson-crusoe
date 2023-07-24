using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MoraleEvent : UnityEvent<string, int>
{
    public const string GainMorale = "GainMorale";
    public const string LoseMorale = "LoseMorale";
}
