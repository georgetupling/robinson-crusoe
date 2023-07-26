using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingActionSpaceController : MonoBehaviour
{
    void Awake() {
        EventGenerator.Singleton.AddListenerToEnableHuntingActionSpaceEvent(OnEnableHuntingActionSpace);
        gameObject.SetActive(false);
    }

    void OnEnableHuntingActionSpace(bool enable) {
        gameObject.SetActive(enable);
    }
}
