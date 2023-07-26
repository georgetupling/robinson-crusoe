using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    private float movementSpeed = 5.0f;
    private float zoomSpeed = 350.0f;

    private float maximumPanY = 3.0f;
    private float maximumPanX = 6.0f;

    private float maximumZoomIn = 4.0f;
    private float maximumZoomOut = 2.5f;

    private Vector3 initialPosition;

    private bool cameraLocked;
    [SerializeField] Transform islandTileArea;

    void Awake() {
        EventGenerator.Singleton.AddListenerToChooseAdjacentTileEvent(OnChooseAdjacentTileEvent);
    }

    void Start() {
        initialPosition = transform.position;
    }

    void Update() {
        if (cameraLocked) {
            return;
        }
        HandleMovementInput();
        HandleZoomInput();
        HandleResetCamera();
    }

    void HandleMovementInput() {
        float horizontal = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float fractionZoomedIn = 1 - (transform.position.z - initialPosition.z + maximumZoomOut) / (maximumZoomIn + maximumZoomOut); // 1 if fully zoomed in, 0 if fully zoomed out
        float maximumPanXAdjustedForZoom = maximumPanX * Mathf.Lerp(1.0f, 0.1f, fractionZoomedIn);
        float maximumPanYAdjustedForZoom = maximumPanY * Mathf.Lerp(1.0f, 0.1f, fractionZoomedIn);

        float newXPosition = Mathf.Clamp(transform.position.x + horizontal, initialPosition.x - maximumPanXAdjustedForZoom, initialPosition.x + maximumPanXAdjustedForZoom);
        float newYPosition = Mathf.Clamp(transform.position.y + vertical, initialPosition.y - maximumPanYAdjustedForZoom, initialPosition.y + maximumPanYAdjustedForZoom);
        transform.position = new Vector3(newXPosition, newYPosition, transform.position.z);
    }

    void HandleZoomInput() {
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        float newZPosition = Mathf.Clamp(transform.position.z + zoom, initialPosition.z - maximumZoomOut, initialPosition.z + maximumZoomIn);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);
    }

    void HandleResetCamera() {
        if (Input.GetKeyDown("r")) {
            cameraLocked = true;
            float duration = 0.75f;
            transform.DOLocalMove(initialPosition, duration);
            cameraLocked = false;
        }
    }

    // This code locks the camera over the island tile area when selecting an island tile!

    void OnChooseAdjacentTileEvent(bool isActive) {
        float duration = 0.75f;
        if (!isActive) {
            transform.DOLocalMove(initialPosition, duration);
            cameraLocked = false;
            return;
        }
        float hoverDistance = 2.5f;
        Vector3 lockedPosition = new Vector3(islandTileArea.localPosition.x, islandTileArea.localPosition.y, islandTileArea.localPosition.z - hoverDistance);
        transform.DOLocalMove(lockedPosition, duration);
        cameraLocked = true;
    }
}
