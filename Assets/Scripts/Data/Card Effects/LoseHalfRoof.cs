using UnityEngine;

public class LoseHalfRoof : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseLoseHalfRoofEvent();
    }
}
