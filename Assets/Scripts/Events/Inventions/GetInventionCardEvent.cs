using UnityEngine;
using UnityEngine.Events;

public class GetInventionCardEvent : UnityEvent<string, Invention, InventionCard>
{
    public const string Query = "Query";
    public const string Response = "Response";
}
