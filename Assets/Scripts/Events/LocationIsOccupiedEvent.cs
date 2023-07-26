using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LocationIsOccupiedEvent : UnityEvent<string, int, bool>
{
    public const string Query = "Query";
    public const string Response = "Response";
}
