using UnityEngine;

public class AllLoseHealthUnlessMedicine : CardEffect
{
    public override void ApplyEffect()
    {
        if (!medicineBuilt)
        {
            EventGenerator.Singleton.RaiseLoseHealthEvent(HealthEvent.AllPlayers, 1);
        }
    }
}
