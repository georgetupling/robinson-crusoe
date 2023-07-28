using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungryPredatorEffect : CardEffect
{   
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("HungryPredatorEffect effect has already been applied.");
            return;
        }
        if (optionChosen == -1) {
            Debug.LogError("HungryPredatorEffect optionChosen not set.");
            return;
        }
        if (targetId == -1) {
            Debug.LogError("HungryPredatorEffect target not set.");
            return;
        }
        // 0 = slay the beast, 1 = shelter in camp
        if (optionChosen == 0) {
            EventGenerator.Singleton.RaiseLoseHealthEvent(targetId, 2);
            EventGenerator.Singleton.RaiseGainFoodEvent(2);
            EventGenerator.Singleton.RaiseGainHideEvent(1);
        } else if (optionChosen == 1) {
            EventGenerator.Singleton.RaiseLoseFoodEvent(1);
            EventGenerator.Singleton.RaiseLosePalisadeEvent(1);
        }
        hasBeenApplied = true;
    }
}
