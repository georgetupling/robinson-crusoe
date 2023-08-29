using UnityEngine;
public class NoFoodInProductionPhase : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseNoResourceInProductionPhaseEvent(ResourceType.Food);
    }
}
