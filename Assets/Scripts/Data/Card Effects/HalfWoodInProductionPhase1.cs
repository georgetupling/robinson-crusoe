using UnityEngine;
public class HalfWoodInProductionPhase : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseHalfResourceInProductionPhaseEvent(ResourceType.Wood);
    }
}
