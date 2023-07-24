using UnityEngine;
using UnityEngine.Events;
using static IslandTileTokenController;
using static TokenController;

[System.Serializable]
public class IslandTileTokenEvent : UnityEvent<string, int, TokenType, Position>
{
    public const string SetTokenPositionById = "SetTokenPositionById";
    public const string TurnCampTokenFaceDown = "TurnCampTokenFaceDown";
    public const string TurnCampTokenFaceUp = "TurnCampTokenFaceUp";
}
