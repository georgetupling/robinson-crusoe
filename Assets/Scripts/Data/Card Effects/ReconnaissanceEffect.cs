using UnityEngine;

public class ReconnaissanceEffect : CardEffect
{
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseReconnaissanceEvent();
    }
}
