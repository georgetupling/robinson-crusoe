using UnityEngine;
public class HalfFoodInProductionPhase : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseHalfResourceInProductionPhaseEvent(ResourceType.Food);
    }
}
