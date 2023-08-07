using UnityEngine;

public class MastEffect : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseAddWoodToPileEvent(3);
    }
}
