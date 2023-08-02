using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerPawnSpawner : MonoBehaviour
{
    Invention invention = Invention.None; // Which invention (if any) the script is listening for
    Transform actionPawnArea;

    int playerId = 6; // The first player "owns" the neutral pawns

    [SerializeField] ActionPawnController actionPawnPrefab;

    void Awake() {
        actionPawnArea = transform;
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
    }

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt) {
        if (invention == Invention.Map && this.invention == Invention.Map) {
            if (isBuilt  && actionPawnArea.childCount == 0) {
                ActionPawnController actionPawn = Instantiate(actionPawnPrefab, actionPawnArea, false);
                actionPawn.InitializeNonPlayerPawn(playerId, false, PawnType.Explore);
            } else if (!isBuilt && actionPawnArea.childCount > 0) {
                Destroy(actionPawnArea.GetChild(0).gameObject);
            }
        } else if (invention == Invention.Lantern && this.invention == Invention.Lantern && actionPawnArea.childCount == 0) {
            if (isBuilt  && actionPawnArea.childCount == 0) {
                ActionPawnController actionPawn = Instantiate(actionPawnPrefab, actionPawnArea, false);
                actionPawn.InitializeNonPlayerPawn(playerId, false, PawnType.Build);
            } else if (!isBuilt && actionPawnArea.childCount > 0) {
                Destroy(actionPawnArea.GetChild(0).gameObject);
            }
        } else if (invention == Invention.Shield && this.invention == Invention.Shield && actionPawnArea.childCount == 0) {
            if (isBuilt  && actionPawnArea.childCount == 0) {
                ActionPawnController actionPawn = Instantiate(actionPawnPrefab, actionPawnArea, false);
                actionPawn.InitializeNonPlayerPawn(playerId, false, PawnType.Hunting);
            } else if (!isBuilt && actionPawnArea.childCount > 0) {
                Destroy(actionPawnArea.GetChild(0).gameObject);
            }
        } else if (invention == Invention.Raft && this.invention == Invention.Raft && actionPawnArea.childCount == 0) {
            if (isBuilt  && actionPawnArea.childCount == 0) {
                ActionPawnController actionPawn = Instantiate(actionPawnPrefab, actionPawnArea, false);
                actionPawn.InitializeNonPlayerPawn(playerId, false, PawnType.GatherOrExplore);
            } else if (!isBuilt && actionPawnArea.childCount > 0) {
                Destroy(actionPawnArea.GetChild(0).gameObject);
            }
        } else if (invention == Invention.Belts && this.invention == Invention.Belts && actionPawnArea.childCount == 0) {
            if (isBuilt  && actionPawnArea.childCount == 0) {
                ActionPawnController actionPawn = Instantiate(actionPawnPrefab, actionPawnArea, false);
                actionPawn.InitializeNonPlayerPawn(playerId, false, PawnType.Gather);
            } else if (!isBuilt && actionPawnArea.childCount > 0) {
                Destroy(actionPawnArea.GetChild(0).gameObject);
            }
        }
    }

    public void SetInvention(Invention invention) {
        this.invention = invention;
    }

}
