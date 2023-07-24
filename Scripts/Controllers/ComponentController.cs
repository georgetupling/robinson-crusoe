using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ComponentController : MonoBehaviour
{
    public int ComponentId { get; protected set; }
    private static int MostRecentComponentId = 0;
    
    protected virtual void Awake() {
        ComponentId = MostRecentComponentId++;
        EventGenerator.Singleton.AddListenerToMoveComponentEvent(OnMoveComponentEvent);
        EventGenerator.Singleton.AddListenerToShakeComponentEvent(OnShakeComponentEvent);
        EventGenerator.Singleton.AddListenerToDestroyComponentEvent(OnDestroyComponentEvent);
    }

    protected virtual void Start() { }
        
    protected void OnMoveComponentEvent(int componentId, Transform transform, Vector3 localPosition, MoveStyle moveStyle) {
        if (this.ComponentId == componentId) {
            MoveToTransform(transform, localPosition, moveStyle);
        }
    }

    protected void OnShakeComponentEvent(int componentId) {
        if (this.ComponentId == componentId) {
            Shake();
        }
    }

    protected void OnDestroyComponentEvent(int componentId) {
        if (this.ComponentId == componentId) {
            Destroy(gameObject);
        }
    }

    // Movement

    protected void MoveToLocalPosition(Vector3 localPosition, MoveStyle moveStyle) {
        float duration = 0.75f;
        float height = -0.05f;
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        switch(moveStyle) {
            case MoveStyle.Default:
                transform.DOLocalMove(localPosition, duration)
                    .OnComplete(() => {
                        EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                    });
                break;
            case MoveStyle.Slow:
                transform.DOLocalMove(localPosition, duration * 1.5f)
                    .OnComplete(() => {
                            EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                        });
                break;
            case MoveStyle.LiftUp:
                transform.DOLocalMoveX(localPosition.x, duration);
                transform.DOLocalMoveY(localPosition.y, duration);
                float totalHeight = Mathf.Min(transform.position.z + height, localPosition.z + height);
                transform.DOLocalMoveZ(totalHeight, duration / 2)
                    .OnComplete(() => {
                        transform.DOLocalMoveZ(localPosition.z, duration / 2)
                            .OnComplete(() => {
                                EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                            });
                    });
                break;
            case MoveStyle.Instant:
                transform.localPosition = localPosition;
                EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                break;
            case MoveStyle.Fast:
                transform.DOLocalMove(localPosition, duration * 0.5f)
                    .OnComplete(() => {
                            EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                        });
                break;
            case MoveStyle.ReturnPawn:
                height = -0.3f;
                transform.DOLocalMoveZ(height, duration / 3f)
                    .OnComplete(() => {
                        transform.DOLocalMoveX(localPosition.x, duration * 1.5f);
                        transform.DOLocalMoveY(localPosition.y, duration * 1.5f)
                            .OnComplete(() => {
                                transform.DOLocalMoveZ(localPosition.z, duration / 3f)
                                    .OnComplete(() => {
                                        EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                                    });
                            });
                    });                
                break;
            default:
                Debug.LogError($"Invalid move style.");
                EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                break;
        }
    }

    protected void MoveToLocalPosition(Vector3 localPosition) {
        MoveToLocalPosition(localPosition, MoveStyle.Default);
    }

    protected void MoveToTransform(Transform transform, Vector3 localPosition, MoveStyle moveStyle) {
        if (transform != null) {
            this.transform.SetParent(transform, true);
        }
        MoveToLocalPosition(localPosition, moveStyle);
    }

    protected void MoveToTransform(Transform transform, Vector3 localPosition) {
        MoveToTransform(transform, localPosition, MoveStyle.Default);
    }

    protected void MoveToTransform(Transform transform, MoveStyle moveStyle) {
        MoveToTransform(transform, Vector3.zero, moveStyle);
    }

    protected void MoveToTransform(Transform transform) {
        MoveToTransform(transform, Vector3.zero, MoveStyle.Default);
    }

    // Turning face up and face down

    protected void TurnFaceDown() {
        TurnFace(180, false);
    }

    protected void TurnFaceUp() {
        TurnFace(0, false);
    }

    protected void TurnFaceDown(bool withAnimation) {
        TurnFace(180, withAnimation);
    }

    protected void TurnFaceUp(bool withAnimation) {
        TurnFace(0, withAnimation);
    }

    protected void TurnFace(float angle, bool withAnimation) {
        if (!withAnimation) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);
            return;
        }
        EventGenerator.Singleton.RaiseAnimationInProgressEvent(true);
        float height = -0.2f;
        float duration = 0.75f;
        float initialZPosition = transform.localPosition.z;
        transform.DOLocalMoveZ(initialZPosition + height, duration / 3)
            .OnComplete(() => {
                transform.DORotate(new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z), duration / 3)
                    .OnComplete(() => {
                        transform.DOLocalMoveZ(initialZPosition, duration / 3);
                        EventGenerator.Singleton.RaiseAnimationInProgressEvent(false);
                    });
            });
    }

    // Shaking

    protected void Shake() {
        float duration = 0.2f;
        Vector3 strength = new Vector3(0.03f, 0.03f, 0f);
        int vibrato = 20;
        float randomness = 30f;
        bool snapping = false;
        transform.DOShakePosition(duration, strength, vibrato, randomness, snapping);
    }
}
