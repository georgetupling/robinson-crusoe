using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InventionCardController : CardController
{
    [SerializeField] private InventionCard data;
    [SerializeField] private InventionCardTokenSpawner tokenSpawner;
    [SerializeField] private InventionBuildActionSpaceController actionSpaceController;
    [SerializeField] private NonPlayerPawnSpawner nonPlayerPawnSpawner;

    public bool IsBuilt { get; private set; }
    private int playerId = -1; // For personal inventions only
    
    protected override void Awake() {
        base.Awake();
        IsBuilt = false;
        EventGenerator.Singleton.AddListenerToBuildInventionSuccessEvent(OnBuildInventionSuccessEvent);
        EventGenerator.Singleton.AddListenerToItemEvent(OnItemEvent);
    }

    void OnBuildInventionSuccessEvent(Invention invention) {
        if (!isInitialized || invention != data.invention) {
            return;
        }
        if (IsBuilt) {
            Debug.LogError($"{data.invention} is already built.");
            return;
        }
        IsBuilt = true;
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        float height = -0.2f;
        float duration = GameSettings.AnimationDuration;
        float initialZPosition = transform.localPosition.z;
        transform.DOLocalMoveZ(initialZPosition + height, duration / 3)
            .OnKill(() => {
                transform.DORotate(new Vector3(0, 180, 0), duration / 3)
                    .OnKill(() => {
                        transform.DOLocalMoveZ(initialZPosition, duration / 3);
                        foreach (CardEffect cardEffect in data.effectsOnBuild) {
                            cardEffect.ApplyEffect();
                        }
                        EventGenerator.Singleton.RaiseUpdateBuiltInventionsEvent(data.invention, true);
                        EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                    });
            });
    }

    void OnItemEvent(string eventType, Invention invention) {
        if (!isInitialized) {
            return;
        }
        if (eventType == ItemEvent.LoseItem && invention == data.invention) {
            HandleLoseItemEvent();
        } else if (eventType == ItemEvent.DiscardInventionCard && invention == data.invention) {
            // TODO
        }
    }

    // Methods for setting the InventionCard (and playerId for personal inventions)

    public void InitializeCard(InventionCard inventionCard) {
        if (!isInitialized) {
            data = inventionCard;
            isInitialized = true;
            meshRenderer.material = data.cardMaterial;
            tokenSpawner.Initialize(inventionCard);
            actionSpaceController.SetInvention(inventionCard.invention);
            nonPlayerPawnSpawner.SetInvention(inventionCard.invention);
        } else {
            Debug.Log("EventCardController already initialized.");
        }
    }

    public void InitializePersonalInvention(int playerId, Invention invention) {
        this.playerId = playerId;
        EventGenerator.Singleton.RaisePersonalInventionSpawnedEvent(this, invention);
    }
    
    // Handles lose item events

    void HandleLoseItemEvent() {
        if (!IsBuilt) {
            Debug.LogError($"{data.invention} is not built.");
            return;
        }
        IsBuilt = false;
        TurnFaceUp(true);
        foreach (CardEffect cardEffect in data.effectsOnBuild) {
            if (cardEffect is OngoingCardEffect) {
                OngoingCardEffect ongoingCardEffect = cardEffect as OngoingCardEffect;
                ongoingCardEffect.EndEffect();
            }
        }
        foreach (CardEffect cardEffect in data.effectsOnLoss) {
            cardEffect.ApplyEffect();
        }
        EventGenerator.Singleton.RaiseUpdateBuiltInventionsEvent(data.invention, false);
    }

    // Getters

    public int GetPlayerId() {
        if (playerId == -1) {
            Debug.LogError("Invention card playerId not set.");
        }
        return playerId;
    }

    public Invention GetInvention() {
        return data.invention;
    }

    public InventionCard GetInventionCard() {
        return data;
        // Used by the ActionPawnController when deciding whether an action assignment is valid
    }
}
