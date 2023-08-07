using UnityEngine;
public class LineSymbolEffect : CardEffect
{
    protected override void Initialize()
    {
        base.Initialize();
        if (GameSettings.CurrentScenario == Scenario.Castaways)
        {
            targetType = TargetType.Player;
        }
    }

    public override void ApplyEffect()
    {
        if (targetType == TargetType.Player && targetId == -1)
        {
            Debug.LogError("LineSymbolEffect target not set.");
            return;
        }
        switch (GameSettings.CurrentScenario)
        {
            case Scenario.Castaways:
                EventGenerator.Singleton.RaiseGainDeterminationEvent(targetId, 3);
                break;
            default:
                Debug.LogError($"LineSymbolEffect not set for {GameSettings.CurrentScenario} scenario.");
                break;
        }
    }
}
