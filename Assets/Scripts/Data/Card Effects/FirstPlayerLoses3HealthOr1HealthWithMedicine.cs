using UnityEngine;

public class FirstPlayerLoses3HealthOr1HealthWithMedicine : OngoingCardEffect
{
    public override void ApplyEffect()
    {
        if (medicineBuilt)
        {
            EventGenerator.Singleton.RaiseLoseHealthEvent(HealthEvent.FirstPlayer, 1);
        }
        else
        {
            EventGenerator.Singleton.RaiseLoseHealthEvent(HealthEvent.FirstPlayer, 3);
        }
    }
}
