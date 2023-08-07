using UnityEngine;
public class SaltireSymbolEffect : CardEffect
{
    protected override void Initialize()
    {
        base.Initialize();
    }

    public override void ApplyEffect()
    {
        if (targetType == TargetType.Player && targetId == -1)
        {
            Debug.LogError("SaltireSymbolEffect target not set.");
            return;
        }
        switch (GameSettings.CurrentScenario)
        {
            case Scenario.Castaways:
                EventGenerator.Singleton.RaiseAddWoodToPileEvent(2);
                break;
            default:
                Debug.LogError($"SaltireSymbolEffect not set for {GameSettings.CurrentScenario} scenario.");
                break;
        }
    }
}
