using UnityEngine;
public class AtStartOfNextRoundDrawDiscoveryToken : OngoingCardEffect
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
        EventGenerator.Singleton.RaiseDrawDiscoveryTokenEvent(1);
        EventGenerator.Singleton.RaiseEndOngoingEffectEvent(this.CardEffectId);
    }
}
