using UnityEngine;
public class SkipProductionPhase : OngoingCardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseSkipProductionPhaseEvent();
    }
}
