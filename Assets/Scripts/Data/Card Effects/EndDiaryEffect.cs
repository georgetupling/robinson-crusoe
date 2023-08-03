using UnityEngine;
public class EndDiaryEffect : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseEndOngoingEffectByTypeEvent(typeof(DiaryEffect));
    }
}
