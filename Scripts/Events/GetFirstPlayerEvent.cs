using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GetFirstPlayerEvent : UnityEvent<string, int>
{
    public const string Query = "Query";
    public const string Response = "Response";
}
