using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventionCardController : CardController
{
    [SerializeField] private InventionCard data;
    [SerializeField] private InventionCardTokenSpawner tokenSpawner;

    public bool IsBuilt { get; private set; }
    
    protected override void Awake() {
        base.Awake();
        IsBuilt = false;
    }

    protected override void Start() {
        base.Start();
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
        TurnFaceDown(true);
        foreach (CardEffect cardEffect in data.effectsOnBuild) {
            cardEffect.ApplyEffect();
        }
        Debug.Log($"{data.invention} built successfully."); // For testing purposes, delete this print later!!
        EventGenerator.Singleton.RaiseUpdateBuiltInventionsEvent(data.invention, true);
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

    public void InitializeCard(InventionCard inventionCard) {
        if (!isInitialized) {
            data = inventionCard;
            isInitialized = true;
            meshRenderer.material = data.cardMaterial;
            tokenSpawner.Initialize(inventionCard);
        } else {
            Debug.Log("EventCardController already initialized.");
        }
    }

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

    public Invention GetInvention() {
        return data.invention;
    }

    public InventionCard GetInventionCard() {
        return data;
        // Used by the ActionPawnController when deciding whether an action assignment is valid
    }
}
