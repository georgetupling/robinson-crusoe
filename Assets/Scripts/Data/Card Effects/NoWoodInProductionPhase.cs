using UnityEngine;
public class NoWoodInProductionPhase : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseNoResourceInProductionPhaseEvent(ResourceType.Wood);
    }
}
