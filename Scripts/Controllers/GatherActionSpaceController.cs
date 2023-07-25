using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherActionSpaceController : MonoBehaviour
{
    IslandTile islandTile;
    Source source;
    [SerializeField] bool isRightSource;
    
    [SerializeField] Transform position0;
    [SerializeField] Transform position1;
    [SerializeField] Transform position2;
    [SerializeField] Transform position3;
    [SerializeField] Transform position4;

    List<Transform> positions;

    int distanceFromCamp;
    bool waitingOnDistanceQuery;

    void Awake() {
        positions = new List<Transform> { position0, position1, position2, position3, position4 };
        SetPositions();
        EventGenerator.Singleton.AddListenerToCampMovedEvent(OnCampMovedEvent);
        EventGenerator.Singleton.AddListenerToGetDistanceFromCampEvent(HandleQueryResponse);
        EventGenerator.Singleton.AddListenerToEnableGatherActionAreaEvent(OnEnableGatherActionAreaEvent);
        gameObject.SetActive(false);
    }

    // Sets the positions so they're in a stack

    void SetPositions() {
        float nextZvalue = 0f;
        float pawnHeight = 0.1f;
        foreach (Transform position in positions) {
            float randomXOffset = Random.Range(-0.01f, 0.01f);
            float randomYOffset = Random.Range(-0.01f, 0.01f);
            position.localPosition = new Vector3(randomXOffset, randomYOffset, nextZvalue);
            nextZvalue -= pawnHeight;
        }
    }

    // Recalculates which positions are active when the camp moves

    void OnCampMovedEvent() {
        if (gameObject.activeInHierarchy) {
            StartCoroutine(UpdateCampLocation());
        }
    }

    void HandleQueryResponse(string eventType, int islandTileId, int response) {
        if (waitingOnDistanceQuery && eventType == GetDistanceFromCampEvent.Response && islandTileId == islandTile.Id) {
            distanceFromCamp = response;
            waitingOnDistanceQuery = false;
        }
    }

    void OnEnableGatherActionAreaEvent(int islandTileId, bool enable) {
            if (islandTileId != islandTile.Id) {
                return;
            }
            if (islandTile.Id == 3 && !isRightSource || islandTile.Sources.Count < 2 && isRightSource && islandTile.Id != 3) {
                // Usually if there is only one source it is on the left, but island tile 3 is the opposite way round
                return;
            }
            gameObject.SetActive(enable);
            if (enable) {
                StartCoroutine(UpdateCampLocation());
            }
        }

    // Helper methods

    IEnumerator UpdateCampLocation() {
        waitingOnDistanceQuery = true;
        EventGenerator.Singleton.RaiseGetDistanceFromCampEvent(islandTile.Id);
        while (waitingOnDistanceQuery) {
            yield return null;
        }
        UpdateActivePositions();
    }

    void UpdateActivePositions() {
        int maximumActionPawns = distanceFromCamp == 0 ? 0 : 1 + distanceFromCamp;
        for (int index = 0; index < positions.Count; index++) {
                positions[index].gameObject.SetActive(index < maximumActionPawns);
        }
    }

    // Public methods

    public void SetIslandTile (IslandTile islandTile) {
        this.islandTile = islandTile;
    }

    public bool GetIsRightSource() {
        return isRightSource;
    }

}
