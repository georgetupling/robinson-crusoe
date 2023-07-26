using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HealthEvent : UnityEvent<string, int, int>
{
    public const string GainHealth = "GainHealth";
    public const string LoseHealth = "LoseHealth";

    public const int Player1 = 0;
    public const int Player2 = 1;
    public const int Player3 = 2;
    public const int Player4 = 3;
    public const int AllPlayers = 5;
    public const int FirstPlayer = 6;
}
