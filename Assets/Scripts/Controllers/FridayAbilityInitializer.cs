using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridayAbilityInitializer : MonoBehaviour
{
    [SerializeField] private AbilityAreaClickHandler clickHandler;

    const int FridaysPlayerId = 4;
    void Awake()
    {
        // TODO?
        // Friday's ability is a reroll so no need to ever activate except to get the closeup...
    }
}
