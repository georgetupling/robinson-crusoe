using UnityEngine;
public class AtStartOfNextRoundGain2Wood : OngoingCardEffect
{
    protected override void Initialize()
    {
        base.Initialize();
        effectTrigger = Trigger.StartTurn;
    }

    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseStartOngoingEffectEvent(this);
    }
    public override void ApplyEffectTrigger()
    {
        EventGenerator.Singleton.RaiseGainWoodEvent(2);
        EventGenerator.Singleton.RaiseEndOngoingEffectEvent(this.CardEffectId);
    }
}
