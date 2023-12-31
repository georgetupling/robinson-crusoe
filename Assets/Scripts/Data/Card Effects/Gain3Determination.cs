using UnityEngine;

public class Gain3Determination : CardEffect
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
            Debug.LogError("Gain3Determination target not set.");
            return;
        }
        EventGenerator.Singleton.RaiseGainDeterminationEvent(targetId, 3);
        hasBeenApplied = true;
    }
}
