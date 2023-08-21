using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridayInitializer : MonoBehaviour
{
    void Start()
    {
        ActionPawnController actionPawn = GetComponent<ActionPawnController>();
        if (actionPawn == null)
        {
            Debug.LogError("Friday action pawn is null.");
            return;
        }
        actionPawn.InitializeNonPlayerPawn(4, false, PawnType.Friday);
    }
}