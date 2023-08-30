using UnityEngine;

public class Lose2HealthUnlessMedicine : CardEffect
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
            Debug.LogError("Lose2HealthUnlessMedicine target not set.");
            return;
        }
        if (!medicineBuilt)
        {
            EventGenerator.Singleton.RaiseLoseHealthEvent(targetId, 2);
        }
    }
}
