using UnityEngine;
using UnityEngine.Events;

public class GetDistanceFromCampEvent : UnityEvent<string, int, int>
{
    public const string Query = "Query";
    public const string Response = "Response";
}
