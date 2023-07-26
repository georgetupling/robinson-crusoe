using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureCardController : CardController
{
    private AdventureCard data;

    protected override void Awake() {
        base.Awake();
        EventGenerator.Singleton.AddListenerToCardDrawnEvent(OnCardDrawnEvent);
    }
    
    protected override void Start() {
        base.Start();
    }

    void OnCardDrawnEvent(Deck deckType, int componentId) {
        if (componentId != this.ComponentId) {
            return;
        }
        RevealCard(deckType, data);
    }

    public void InitializeCard(AdventureCard adventureCard) {
        if (!isInitialized) {
            data = adventureCard;
            isInitialized = true;
            meshRenderer.material = data.cardMaterial;
        } else {
            Debug.Log("AdventureCardController already initialized.");
        }
    }
}
