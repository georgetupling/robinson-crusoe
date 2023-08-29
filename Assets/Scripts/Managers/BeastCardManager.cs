using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;

public class BeastCardManager : MonoBehaviour
{
    private BeastCardManager singleton;

    private List<BeastCard> beastCards = new List<BeastCard>();

    private Stack<BeastCardController> beastDeck = new Stack<BeastCardController>();
    private Stack<BeastCardController> huntingDeck = new Stack<BeastCardController>();

    [SerializeField] private Transform beastDeckArea;
    [SerializeField] private Transform huntingDeckArea;

    [SerializeField] private BeastCardController beastCardPrefab;

    private float CardThickness = 0.005f;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeBeastCards();
        SpawnBeastDeck();
        EventGenerator.Singleton.AddListenerToDrawCardEvent(OnDrawCardEvent);
        EventGenerator.Singleton.AddListenerToTrackingEvent(OnTrackingEvent);
        EventGenerator.Singleton.AddListenerToResolveHuntingActionEvent(OnResolveHuntingActionEvent);
        EventGenerator.Singleton.AddListenerToIfPossibleDiscardBeastCardFromHuntingDeckEvent(OnIfPossibleDiscardBeastCardFromHuntingDeckEvent);
    }

    // Listeners

    void OnDrawCardEvent(Deck deck)
    {
        if (deck == Deck.Beast)
        {
            DrawCardFromBeastDeck();
        }
    }

    void OnTrackingEvent()
    {
        if (huntingDeck.Count == 0)
        {
            Debug.LogError("Hunting deck is empty. Failed to action tracking ability.");
            return;
        }
        BeastCardController topCard = huntingDeck.Pop();
        EventGenerator.Singleton.RaiseSpawnTrackingPopupEvent(topCard, huntingDeck);
    }

    void OnResolveHuntingActionEvent(int playerId)
    {
        if (huntingDeck.Count == 0)
        {
            Debug.LogError("Hunting deck is empty.");
            return;
        }
        BeastCardController drawnCard = huntingDeck.Pop();

        float height = 0.3f;
        float duration = GameSettings.AnimationDuration / 2f;
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        drawnCard.transform.DOMoveZ(transform.position.z - height, duration)
            .OnKill(() =>
            {
                drawnCard.transform.DORotate(Vector3.zero, duration)
                    .OnKill(() =>
                    {
                        EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                        EventGenerator.Singleton.RaiseSpawnCombatPopupEvent(playerId, drawnCard.ComponentId, drawnCard.data);
                    });
            });
    }

    void OnIfPossibleDiscardBeastCardFromHuntingDeckEvent()
    {
        if (huntingDeck.Count > 0)
        {
            DiscardCardFromHuntingDeck();
        }
    }

    // Methods for initializing beast cards and spawning the beast deck

    void InitializeBeastCards()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.Combine("Data", "beast-cards"));
        string jsonString = jsonTextAsset.text;
        List<string> jsonStrings = Json.ToList(jsonString);
        foreach (string str in jsonStrings)
        {
            BeastCardUnprocessedData data = JsonUtility.FromJson<BeastCardUnprocessedData>(str);
            BeastCard beastCard = new BeastCard(data);
            beastCards.Add(beastCard);
        }
    }

    void SpawnBeastDeck()
    {
        foreach (BeastCard beastCard in beastCards)
        {
            BeastCardController newCard = Instantiate(beastCardPrefab, beastDeckArea, false);
            Vector3 localPosition = new Vector3(0, 0, (-1) * beastDeck.Count * CardThickness);
            EventGenerator.Singleton.RaiseMoveComponentEvent(newCard.ComponentId, localPosition, MoveStyle.Instant);
            newCard.transform.eulerAngles = new Vector3(0, 180, 0);
            newCard.InitializeCard(beastCard);
            beastDeck.Push(newCard);
        }
        DeckShuffler.Singleton.ShuffleDeck(beastDeck, CardThickness);
    }

    // Methods for actioning events

    void DrawCardFromBeastDeck()
    {
        BeastCardController drawnCard = beastDeck.Pop();
        if (huntingDeck.Count == 0)
        {
            EventGenerator.Singleton.RaiseEnableHuntingActionSpaceEvent(true);
        }
        huntingDeck.Push(drawnCard);
        drawnCard.transform.SetParent(huntingDeckArea, true);
        Vector3 localPosition = new Vector3(0, 0, (-1) * huntingDeck.Count * CardThickness);
        float duration = GameSettings.AnimationDuration;
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        drawnCard.transform.DOLocalMoveX(localPosition.x, duration);
        drawnCard.transform.DOLocalMoveY(localPosition.y, duration);
        float height = -0.05f;
        float totalHeight = Mathf.Min(drawnCard.transform.position.z + height, localPosition.z + height);
        drawnCard.transform.DOLocalMoveZ(totalHeight, duration / 2)
                .OnKill(() =>
                {
                    drawnCard.transform.DOLocalMoveZ(localPosition.z, duration / 2)
                        .OnKill(() =>
                        {
                            EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                            DeckShuffler.Singleton.ShuffleDeck(huntingDeck, CardThickness);
                        });
                });
    }

    void DiscardCardFromHuntingDeck()
    {
        BeastCardController cardToDiscard = huntingDeck.Pop();
        Destroy(cardToDiscard);
    }
}
