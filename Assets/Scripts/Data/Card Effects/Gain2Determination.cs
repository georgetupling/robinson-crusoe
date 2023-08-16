using UnityEngine;

public class Gain2Determination : CardEffect
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
            Debug.LogError("Gain2Determination target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseGainDeterminationEvent(targetId, 2);
        hasBeenApplied = true;
    }
}
