using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogInitializer : MonoBehaviour
{
    void Start() {
        ActionPawnController actionPawn = GetComponent<ActionPawnController>();
        if (actionPawn == null) {
            Debug.LogError("Dog action pawn is null.");
            return;
        }
        actionPawn.InitializeNonPlayerPawn(6, false, PawnType.Dog);
    }
}