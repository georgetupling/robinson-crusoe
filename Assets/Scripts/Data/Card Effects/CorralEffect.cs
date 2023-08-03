using UnityEngine;

public class CorralEffect : CardEffect
{   
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseGetIslandTileInputEvent(true, InputType.Corral);
    }
}
