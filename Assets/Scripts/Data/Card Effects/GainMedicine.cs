using UnityEngine;

public class GainMedicine : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseBuildInventionSuccessEvent(Invention.Medicine);
    }
}
