using UnityEngine;
public class EndDrumsEffect : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(DrumsEffect));
    }
}
