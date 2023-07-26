using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card { 
    public Material cardMaterial { get; protected set; }
    public Sprite cardSprite { get; protected set; }
    public Sprite cardBackSprite { get; protected set; }
}

// This class exists to be the parent class for EventCard, AdventureCard, etc. so that they can be used interchangeably in certain contexts
// E.g. when creating a card popup you can pass any Card
// This saves me making separate card popup controllers for each type of card
