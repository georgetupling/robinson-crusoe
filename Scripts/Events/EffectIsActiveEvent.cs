using UnityEngine;
using UnityEngine.Events;
using static System.Type;

[System.Serializable]
public class EffectIsActiveEvent : UnityEvent<string, int, System.Type, bool>
{
    public const string RequestById = "RequestById";
    public const string RequestByType = "RequestByType";
    public const string Response = "Response";
}
