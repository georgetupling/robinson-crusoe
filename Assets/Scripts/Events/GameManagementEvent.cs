using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameManagementEvent : UnityEvent<string, bool>
{
    public const string EndGame = "EndGame";
    
    public const bool Victory = true;
    public const bool Defeat = false;
}
