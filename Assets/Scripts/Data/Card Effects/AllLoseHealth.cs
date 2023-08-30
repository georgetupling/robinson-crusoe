using UnityEngine;

public class AllLoseHealth : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseHealthEvent(HealthEvent.AllPlayers, 1);
    }
}
