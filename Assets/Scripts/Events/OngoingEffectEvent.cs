using UnityEngine;
using UnityEngine.Events;
using static OngoingCardEffect;

[System.Serializable]
public class OngoingEffectEvent : UnityEvent<string, OngoingCardEffect, int, Trigger>
{
    public const string StartOngoingEffect = "StartOngoingEffect";
    public const string EndOngoingEffect = "EndOngoingEffect";
    public const string ApplyEndTrigger = "ApplyEndTrigger";
    public const string ApplyEffectTrigger = "ApplyEffectTrigger";
}
