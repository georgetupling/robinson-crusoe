using UnityEngine;
using UnityEngine.Events;

public class GetDistanceFromCampToLocationEvent : UnityEvent<string, int, int>
{
    public const string Query = "Query";
    public const string Response = "Response";
}
