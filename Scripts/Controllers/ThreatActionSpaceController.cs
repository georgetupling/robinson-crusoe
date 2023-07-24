using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatActionSpaceController : MonoBehaviour
{
    public enum ThreatActionPosition { Left, Centre, Right };
    
    int componentId;
    EventCard eventCard;
    [SerializeField] EventCardController cardController;
    [SerializeField] ThreatActionPosition threatActionPosition;
    [SerializeField] Transform position0;
    [SerializeField] Transform position1;

    void Awake() {
        EventGenerator.Singleton.AddListenerToEnableThreatActionAreaEvent(OnEnableThreatActionAreaEvent);
        gameObject.SetActive(false);
    }

    void OnEnableThreatActionAreaEvent(int componentId, bool enable) {
        if (componentId != this.componentId) {
            return;
        }
        if (enable == gameObject.activeSelf) {
            return;
        }
        if (eventCard == null) {
            return;
        }
        if (eventCard.has1ActionThreat && eventCard.has2ActionThreat && threatActionPosition == ThreatActionPosition.Left) {
            position1.gameObject.SetActive(false);
            gameObject.SetActive(true);
        } else if (eventCard.has1ActionThreat && eventCard.has2ActionThreat && threatActionPosition == ThreatActionPosition.Right) {
            gameObject.SetActive(true);
        } else if (eventCard.has1ActionThreat && !eventCard.has2ActionThreat && threatActionPosition == ThreatActionPosition.Centre) {
            position1.gameObject.SetActive(false);
            gameObject.SetActive(true);
        } else if (!eventCard.has1ActionThreat && eventCard.has2ActionThreat && threatActionPosition == ThreatActionPosition.Centre) {
            gameObject.SetActive(true);
        }
    }

    public void Initialize(int componentId, EventCard eventCard) {
        this.componentId = componentId;
        this.eventCard = eventCard;
    }

    public bool GetIsTwoActionThreat() {
        return (
            (threatActionPosition == ThreatActionPosition.Right) || 
            (threatActionPosition == ThreatActionPosition.Centre && !eventCard.has1ActionThreat && eventCard.has2ActionThreat)
        );
    }
}
