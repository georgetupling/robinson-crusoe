using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotivationalSpeechEffect : CardEffect
{ 
    public override void ApplyEffect() {
        EventGenerator.Singleton.RaiseGainMoraleEvent(1);
    }
}
