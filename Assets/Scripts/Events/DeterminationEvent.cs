using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DeterminationEvent : UnityEvent<string, int, int>
{
    public const string GainDetermination = "GainDetermination";
    public const string LoseDetermination = "LoseDetermination";

    public const int Player1 = 0;
    public const int Player2 = 1;
    public const int Player3 = 2;
    public const int Player4 = 3;
    public const int Friday = 4;
    public const int AllPlayers = 5;
    public const int FirstPlayer = 6;
}
