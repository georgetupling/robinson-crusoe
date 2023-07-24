using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastCardController : CardController
{
    public BeastCard data { get; private set; }

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
    }

    public void InitializeCard(BeastCard beastCard) {
        if (!isInitialized) {
            data = beastCard;
            isInitialized = true;
            meshRenderer.material = data.cardMaterial;
        } else {
            Debug.Log("BeastCardController already initialized.");
        }
    }
}
