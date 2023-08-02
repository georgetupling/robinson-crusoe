using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DiscoveryTokenController : TokenController, IPointerClickHandler
{
    public DiscoveryToken data { get; private set; }
    private bool isInitialized;

    private MeshRenderer meshRenderer;
    
    protected override void Awake() {
        base.Awake();
        TurnFaceDown();
        this.tokenType = TokenType.Discovery;
        meshRenderer = GetComponent<MeshRenderer>();
        EventGenerator.Singleton.AddListenerToInitializeDiscoveryTokenEvent(OnInitializeDiscoveryTokenEvent);
        EventGenerator.Singleton.AddListenerToDiscoveryTokenDrawnEvent(OnDiscoveryTokenDrawnEvent);
        EventGenerator.Singleton.AddListenerToSpritePopupClosedEvent(OnSpritePopupClosedEvent);
        EventGenerator.Singleton.AddListenerToActivateDiscoveryTokenEvent(OnActivateDiscoveryTokenEvent);
    }

    protected override void Start() {
        base.Start();
    }

    // Initializes the discovery token controller

    void OnInitializeDiscoveryTokenEvent(int componentId, DiscoveryToken data) {
        if (componentId != this.ComponentId) {
            return;
        }
        if (data == null) {
            Debug.LogError("Discovery token data is null.");
            return;
        }
        if (isInitialized) {
            Debug.LogError("Discovery token is already initialized.");
            return;
        }
        this.data = data;
        isInitialized = true;
        meshRenderer.material = data.tokenMaterial;
    }

    // Reveals the discovery token with an animation when it is drawn

    void OnDiscoveryTokenDrawnEvent(int componentId) {
        if (componentId == this.ComponentId) {
            RevealDiscoveryToken();
        }
    }

    void RevealDiscoveryToken() {
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        transform.DOMoveZ(transform.position.z - 0.15f, 0.5f)
            .OnComplete(() => {
                transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f)
                    .OnKill(() => 
                    {
                    EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                    EventGenerator.Singleton.RaiseDiscoveryTokenRevealedEvent(ComponentId, data.tokenSprite);
                    });
            });
    }

    // Moves the discovery token to the future resources area when the reveal popup is closed

    void OnSpritePopupClosedEvent(int sourceComponentId) {
        if (sourceComponentId != this.ComponentId) {
            return;
        }
        foreach (CardEffect tokenEffect in data.effectsOnDraw) {
            tokenEffect.ApplyEffect();
        }
        if (data.canBeActivated) {
            EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
            transform.DOMove(futureResourcesArea.position, 0.75f)
                .OnComplete(() => {
                    transform.SetParent(futureResourcesArea, true);
                    TokenPositioner.PositionTokens(futureResourcesArea);
                    EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                });
        } else {
            Destroy(gameObject);
        }
    }

    // Opens the confirm activation pop when clicked while in the available resources area

    public void OnPointerClick(PointerEventData pointerEventData) {
        if (transform.parent != availableResourcesArea) {
            if (transform.parent == futureResourcesArea) {
                Shake();
            }
            return;
        }
        EventGenerator.Singleton.RaiseSpawnDiscoveryTokenActivationPopupEvent(this.ComponentId, data);
    }

    // Activates the discovery token

    public void OnActivateDiscoveryTokenEvent(int sourceComponentId, int selectedPlayerId) {
        if (sourceComponentId != this.ComponentId) {
            return;
        }
        foreach (CardEffect activationEffect in data.effectsOnActivation) {
            if (activationEffect.targetType == TargetType.Player) {
                activationEffect.SetTarget(selectedPlayerId);
            }
            activationEffect.ApplyEffect();
        }
        TokenPositioner.PositionTokens(availableResourcesArea);
        Destroy(gameObject);
    }
}
