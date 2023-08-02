using UnityEngine;

public class TrackingEffect : CardEffect
{
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseTrackingEvent();
    }
}
