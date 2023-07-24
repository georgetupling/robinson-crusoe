using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class CardController : ComponentController
{
    protected bool isInitialized;
    protected MeshRenderer meshRenderer;
    
    protected override void Awake() {
        base.Awake();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    protected void RevealCard(Deck deckDrawnFrom, Card data) {
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        transform.DOMoveZ(transform.position.z - 0.3f, 0.5f)
            .OnComplete(() => {
                transform.DORotate(new Vector3(0f, 180f, 0f), 0.5f)
                    .OnComplete(() => {
                    EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                    EventGenerator.Singleton.RaiseCardRevealedEvent(deckDrawnFrom, data, this.ComponentId);
                    });
            });
    }
}
