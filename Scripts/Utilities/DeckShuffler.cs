using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeckShuffler
{
    public static void ShuffleDeck(Stack<IslandTileController> deck, float islandTileThickness) {
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
    }

    public static void ShuffleDeck(Stack<CardController> deck, float cardThickness) {
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
    }

    public static void ShuffleDeck(Stack<DiscoveryTokenController> deck, float tokenThickness) {
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
    }

    public static void ShuffleDeck(Stack<InventionCard> deck) {
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

    public static void ShuffleDeck(Stack<BeastCardController> deck, float cardThickness) {
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
            deck.Push(card);
        }
    }
}
