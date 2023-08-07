using UnityEngine;
public class TwoLinesSymbolEffect : CardEffect
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
            Debug.LogError("TwoLinesSymbolEffect target not set.");
            return;
        }
        switch (GameSettings.CurrentScenario)
        {
            case Scenario.Castaways:
                EventGenerator.Singleton.RaiseGainHealthEvent(targetId, 1);
                break;
            default:
                Debug.LogError($"TwoLinesSymbolEffect not set for {GameSettings.CurrentScenario} scenario.");
                break;
        }
    }
}
