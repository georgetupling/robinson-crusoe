using UnityEngine;

public class IfPossible1LessFoodInProductionPhase : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseIfPossibleLessResourceInProductionPhaseEvent(ResourceType.Food, 1);
    }
}
