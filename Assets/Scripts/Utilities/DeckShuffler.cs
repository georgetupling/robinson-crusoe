using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeckShuffler : MonoBehaviour
{
    public static DeckShuffler Singleton;

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
        } else {
            Debug.LogError("Scene contains duplicate DeckShuffler.");
        }
    }

    public void ShuffleDeck(Stack<IslandTileController> deck, float islandTileThickness) {
        IslandTileController[] array = deck.ToArray();
        deck.Clear();
        for (int i = 0; i < array.Length; i++) {
            int rand = Random.Range(0, array.Length);
            IslandTileController tmp = array[i];
            array[i] = array[rand];
            array[rand] = tmp;
        }
        for (int i = 0; i < array.Length; i++) {
            IslandTileController tile = array[i];
            tile.transform.localPosition = new Vector3(0, 0, (-1) * deck.Count * islandTileThickness);
            deck.Push(tile);
        }
        // Plays the animation
        List<ComponentController> componentList = new List<ComponentController>();
        for (int i = 0; i < array.Length; i++) {
            componentList.Add(array[i]);
        }
        StartCoroutine(PlayShuffleAnimation(componentList));
    }

    public void ShuffleDeck(Stack<CardController> deck, float cardThickness) {
        CardController[] array = deck.ToArray();
        deck.Clear();
        for (int i = 0; i < array.Length; i++) {
            int rand = Random.Range(0, array.Length);
            CardController tmp = array[i];
            array[i] = array[rand];
            array[rand] = tmp;
        }
        for (int i = 0; i < array.Length; i++) {
            CardController card = array[i];
            card.transform.localPosition = new Vector3(0, 0, (-1) * deck.Count * cardThickness);
            deck.Push(card);
        }
        // Plays the animation
        List<ComponentController> componentList = new List<ComponentController>();
        for (int i = 0; i < array.Length; i++) {
            componentList.Add(array[i]);
        }
        StartCoroutine(PlayShuffleAnimation(componentList));
    }

    public void ShuffleDeck(Stack<DiscoveryTokenController> deck, float tokenThickness) {
        DiscoveryTokenController[] array = deck.ToArray();
        deck.Clear();
        for (int i = 0; i < array.Length; i++) {
            int rand = Random.Range(0, array.Length);
            DiscoveryTokenController tmp = array[i];
            array[i] = array[rand];
            array[rand] = tmp;
        }
        for (int i = 0; i < array.Length; i++) {
            DiscoveryTokenController token = array[i];
            token.transform.localPosition = new Vector3(0, 0, (-1) * deck.Count * tokenThickness);
            deck.Push(token);
        }
        // Plays the animation
        List<ComponentController> componentList = new List<ComponentController>();
        for (int i = 0; i < array.Length; i++) {
            componentList.Add(array[i]);
        }
        StartCoroutine(PlayShuffleAnimation(componentList));
    }

    public void ShuffleDeck(Stack<InventionCard> deck) {
        InventionCard[] array = deck.ToArray();
        deck.Clear();
        for (int i = 0; i < array.Length; i++) {
            int rand = Random.Range(0, array.Length);
            InventionCard tmp = array[i];
            array[i] = array[rand];
            array[rand] = tmp;
        }
        for (int i = 0; i < array.Length; i++) {
            InventionCard card = array[i];
            deck.Push(card);
        }
    }

    public void ShuffleDeck(Stack<BeastCardController> deck, float cardThickness) {
        BeastCardController[] array = deck.ToArray();
        deck.Clear();
        for (int i = 0; i < array.Length; i++) {
            int rand = Random.Range(0, array.Length);
            BeastCardController tmp = array[i];
            array[i] = array[rand];
            array[rand] = tmp;
        }
        for (int i = 0; i < array.Length; i++) {
            BeastCardController card = array[i];
            card.transform.localPosition = new Vector3(0, 0, (-1) * deck.Count * cardThickness);
            card.transform.eulerAngles = new Vector3(0, 180, 0);
            deck.Push(card);
        }
        // Plays the animation
        List<ComponentController> componentList = new List<ComponentController>();
        for (int i = 0; i < array.Length; i++) {
            componentList.Add(array[i]);
        }
        StartCoroutine(PlayShuffleAnimation(componentList));
    }

    IEnumerator PlayShuffleAnimation(List<ComponentController> deck) {
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        ComponentController[] array = deck.ToArray();
        float shuffleDuration = GameSettings.AnimationDuration * 0.5f;
        float delayBetweenShakes = shuffleDuration / array.Length;

        float shakeDuration = 0.1f;
        Vector3 scaleShakeStrength = new Vector3(0.02f, 0.01f, 0f);
        Vector3 postionShakeStrength = new Vector3(0.005f, 0.005f, 0f);
        int vibrato = 20;
        int randomness = 30;
        bool snapping = false;
        for (int i = 0; i < array.Length; i++) {
            array[i].transform.DOShakeScale(shakeDuration, scaleShakeStrength, vibrato, randomness);
            array[i].transform.DOShakePosition(shakeDuration, postionShakeStrength, vibrato, randomness, snapping);
            yield return new WaitForSeconds(delayBetweenShakes);
        }
        for (int i = array.Length - 1; i >= 0; i--) {
            array[i].transform.DOShakeScale(shakeDuration, scaleShakeStrength, vibrato, randomness);
            array[i].transform.DOShakePosition(shakeDuration, postionShakeStrength, vibrato, randomness, snapping);
            yield return new WaitForSeconds(delayBetweenShakes);
        }
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
    }
}
