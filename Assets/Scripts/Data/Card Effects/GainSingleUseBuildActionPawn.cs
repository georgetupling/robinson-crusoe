using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainSingleUseBuildActionPawn : CardEffect
{
    protected override void Initialize() {
        base.Initialize();
        targetType = TargetType.Player;
    }
    
    public override void ApplyEffect() {
        if (hasBeenApplied) {
            Debug.LogError("GainSingleUseBuildActionPawn effect has already been applied.");
            return;
        }
        if (targetId == -1) {
            // If no player is specified, the script searches the scene to find a player with a free action pawn space
            SingleUsePawnSpawner[] pawnSpawners = GameObject.FindObjectsOfType(typeof(SingleUsePawnSpawner)) as SingleUsePawnSpawner[];
            for (int i = 0; i < pawnSpawners.Length; i++) {
                if (pawnSpawners[i].transform.childCount == 0) {
                    targetId = pawnSpawners[i].GetPlayerId();
                }
            }
        }
        if (targetId == -1) {
            Debug.LogError("Failed to find unoccupied location to spawn build action pawn.");
            return;
        }
        EventGenerator.Singleton.RaiseSpawnSingleUsePawnEvent(targetId, PawnType.Build);
        hasBeenApplied = true;
    }
}
