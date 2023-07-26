using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureCardController : CardController
{
    private AdventureCard data;

    protected override void Awake() {
        base.Awake();
    }
    
    protected override void Start() {
        base.Start();
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
