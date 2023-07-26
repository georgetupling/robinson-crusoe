using UnityEngine;
using UnityEngine.Events;

public class InventionIsBuiltEvent : UnityEvent<string, Invention, bool>
{
    public const string Query = "Query";
    public const string Response = "Response";
}
