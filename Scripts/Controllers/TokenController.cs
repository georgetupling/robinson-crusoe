using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TokenController : ComponentController
{
    public TokenType tokenType;

    protected Transform futureResourcesArea;
    protected Transform availableResourcesArea;

    protected override void Awake() {
        base.Awake();
        futureResourcesArea = GameObject.Find("FutureResourcesArea").transform;
        availableResourcesArea = GameObject.Find("AvailableResourcesArea").transform;
        EventGenerator.Singleton.AddListenerToResourceEvent(OnResourceEvent);
    }

    // Moves tokens from the future resources area to the available resources area

    protected virtual void OnResourceEvent(string eventType, int amount) {
        if (eventType == ResourceEvent.MakeResourcesAvailable && transform.parent == futureResourcesArea) {
            Vector3 initialLocalPosition = transform.localPosition;
            EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
            transform.SetParent(availableResourcesArea, true);
            transform.DOLocalMove(initialLocalPosition, 0.5f)
                .OnComplete(() => {
                    TokenPositioner.PositionTokens(availableResourcesArea);
                    EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                });
        }
    }
}
