using UnityEngine;

public class FirstPlayerCanOnlyBuildArrangeCampOrRest : CardEffect
{
    public override void ApplyEffect()
    {
        int firstPlayerId = 6;
        EventGenerator.Singleton.RaisePlayerCanOnlyRestBuildOrMakeCampThisTurnEvent(firstPlayerId);
    }
}
