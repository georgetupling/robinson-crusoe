using UnityEngine;
using UnityEngine.Events;

public class ItemEvent : UnityEvent<string, Invention>
{
    public const string LoseItem = "LoseItem";
    public const string DiscardInventionCard = "DiscardInventionCard";
}
