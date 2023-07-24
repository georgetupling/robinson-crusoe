using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SpawnTokenOnDeckEvent : UnityEvent<Deck, TokenType> { }

