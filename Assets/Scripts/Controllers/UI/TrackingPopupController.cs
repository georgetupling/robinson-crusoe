using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackingPopupController : MonoBehaviour
{
    [SerializeField] Image cardImage;
    [SerializeField] Button topButton;
    [SerializeField] Button bottomButton;

    const float CardThickness = 0.005f;

    void Awake() {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
    }

    public void Initialize(BeastCardController beastCard, Stack<BeastCardController> huntingDeck) {
        cardImage.sprite = beastCard.data.cardSprite;

        topButton.onClick.AddListener(() => {
            PutOnTop(beastCard, huntingDeck);
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });

        bottomButton.onClick.AddListener(() => {
            PutOnBottom(beastCard, huntingDeck);
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }

    void PutOnTop(BeastCardController beastCard, Stack<BeastCardController> huntingDeck) {
        huntingDeck.Push(beastCard);
        beastCard.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    void PutOnBottom(BeastCardController beastCard, Stack<BeastCardController> huntingDeck) {
        // Turns the deck into a List and inserts the card at index 0
        List<BeastCardController> deckList = new List<BeastCardController>(huntingDeck);
        deckList.Insert(0, beastCard);
        // Reconstructs the deck
        huntingDeck.Clear();
        foreach (BeastCardController card in deckList) {
            huntingDeck.Push(card);
            card.transform.localPosition = new Vector3 (0, 0, (-1) * huntingDeck.Count * CardThickness);
        }
    }
}
