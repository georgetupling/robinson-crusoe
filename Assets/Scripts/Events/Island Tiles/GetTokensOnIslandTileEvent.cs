using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GetTokensOnIslandTileEvent : UnityEvent<string, int, List<TokenType>>
{
    public const string Query = "Query";
    public const string Response = "Response";
}
