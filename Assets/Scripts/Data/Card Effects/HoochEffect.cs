using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoochEffect : CardEffect
{   
    public override void ApplyEffect() {
        if (optionChosen == -1) {
            Debug.LogError("HoochEffect optionChosen not set.");
            return;
        }
        // 0 = cancel, 1 = cancel rain cloud, 2 = convert rain to snow
        if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseCancelRainCloudEvent();
        } else if (optionChosen == 2) {
            EventGenerator.Singleton.RaiseConvertSnowToRainEvent();
        }
    }
}
