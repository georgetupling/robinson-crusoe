using UnityEngine;

public class ShortcutEffect : CardEffect
{
    public override void ApplyEffect()
    {
        EventGenerator.Singleton.RaiseGetIslandTileInputEvent(true, InputType.Shortcut);
    }
}
