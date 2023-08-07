using UnityEngine;
public class CrossSymbolEffect : CardEffect
{
    protected override void Initialize()
    {
        base.Initialize();
    }

    public override void ApplyEffect()
    {
        if (targetType == TargetType.Player && targetId == -1)
        {
            Debug.LogError("CrossSymbolEffect target not set.");
            return;
        }
        switch (GameSettings.CurrentScenario)
        {
            case Scenario.Castaways:
                EventGenerator.Singleton.RaiseGainWeaponEvent(1);
                break;
            default:
                Debug.LogError($"CrossSymbolEffect not set for {GameSettings.CurrentScenario} scenario.");
                break;
        }
    }
}
