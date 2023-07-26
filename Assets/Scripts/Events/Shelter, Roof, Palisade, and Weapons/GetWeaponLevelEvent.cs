using UnityEngine;
using UnityEngine.Events;

public class GetWeaponLevelEvent : UnityEvent<string, int>
{
    public const string Query = "Query";
    public const string Response = "Response";
}
