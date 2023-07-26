using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleRaytraceBlockerController : MonoBehaviour
{
    CanvasGroup canvasGroup;
    
    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        EventGenerator.Singleton.AddListenerToEnableMainUIEvent(OnEnableMainUIEvent);
    }

    void OnEnableMainUIEvent(bool isEnabled) {
        canvasGroup.blocksRaycasts = isEnabled ? false : true;
    }

}
