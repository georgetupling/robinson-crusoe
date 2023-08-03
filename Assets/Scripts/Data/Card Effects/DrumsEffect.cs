using UnityEngine;
public class DrumsEffect : OngoingCardEffect
{
    protected override void Initialize()
    {
        base.Initialize();
        endTrigger = Trigger.None;
        effectTrigger = Trigger.MoralePhase;
    }

    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
    }
    public override void ApplyEffectTrigger()
    {
        EventGenerator.Singleton.RaiseGainDeterminationEvent(DeterminationEvent.FirstPlayer, 2);
    }
}
