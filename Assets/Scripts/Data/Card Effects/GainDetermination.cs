using UnityEngine;

public class GainDetermination : CardEffect
{
    protected override void Initialize()
    {
        base.Initialize();
        targetType = TargetType.Player;
    }

    public override void ApplyEffect()
    {
        if (targetId == -1)
        {
            Debug.LogError("GainDetermination target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseGainDeterminationEvent(targetId, 1);
    }
}
