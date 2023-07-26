using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreActionAreaController : MonoBehaviour
{
    [SerializeField] int locationId;
    
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
        EventGenerator.Singleton.AddListenerToGetDistanceFromCampToLocationEvent(HandleQueryResponse);
        EventGenerator.Singleton.AddListenerToEnableExploreActionAreaEvent(OnEnableExploreActionAreaEvent);
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

    // Listeners

    void OnCampMovedEvent() {
        if (gameObject.activeInHierarchy) {
            StartCoroutine(UpdateCampLocation());
        }
    }

    void HandleQueryResponse(string eventType, int locationId, int response) {
        if (waitingOnDistanceQuery && eventType == GetDistanceFromCampToLocationEvent.Response && locationId == this.locationId) {
            distanceFromCamp = response;
            waitingOnDistanceQuery = false;
        }
    }

    void OnEnableExploreActionAreaEvent(int locationId, bool enable) {
        if (locationId == this.locationId) {
            gameObject.SetActive(enable);
            if (enable) {
                StartCoroutine(UpdateCampLocation());
            }
        }
    }

    IEnumerator UpdateCampLocation() {
        waitingOnDistanceQuery = true;
        EventGenerator.Singleton.RaiseGetDistanceFromCampToLocationEvent(locationId);
        while (waitingOnDistanceQuery) {
            yield return null;
        }
        UpdateActivePositions();
    }

    void UpdateActivePositions() {
        int maximumActionPawns = distanceFromCamp == 0 ? 2 : 1 + distanceFromCamp;
        for (int index = 0; index < positions.Count; index++) {
                positions[index].gameObject.SetActive(index < maximumActionPawns);
        }
    }

    public int GetLocationId() {
        return locationId;
    }
}   
