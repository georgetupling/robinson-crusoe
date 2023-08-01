using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUsePawnSpawner : MonoBehaviour
{
    int playerId = -1;

    [SerializeField] ActionPawnController prefab;
    Transform pawnPosition;

    void Awake() {
        pawnPosition = transform;
        EventGenerator.Singleton.AddListenerToSpawnSingleUsePawnEvent(OnSpawnSingleUsePawnEvent);
    }

    void OnSpawnSingleUsePawnEvent(int playerId, PawnType pawnType) {
        if (playerId != this.playerId) {
            return;
        }
        if (pawnType == PawnType.Player) {
            Debug.LogError("Single use pawns can't have pawn type Player.");
            return;
        }
        if (pawnPosition.childCount > 0) {
            Debug.LogError("Single use pawn position already occupied.");
            return;
        }
        ActionPawnController singleUsePawn = Instantiate(prefab, pawnPosition, false);
        singleUsePawn.InitializeNonPlayerPawn(playerId, true, pawnType);
    }

    public void SetPlayerId(int playerId) {
        this.playerId = playerId;
    }
    public int GetPlayerId() {
        return playerId;
    }
}
