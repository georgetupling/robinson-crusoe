using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscardAllThreatsAndApplyFailureEffects : CardEffect
{
    public override void ApplyEffect()
    {
        List<EventCardController> threats = new List<EventCardController>();
        Transform leftThreatArea = GameObject.Find("leftThreatArea").transform;
        Transform rightThreatArea = GameObject.Find("rightThreatArea").transform;
        if (leftThreatArea.childCount > 0 && leftThreatArea.GetChild(0) != null)
        {
            EventCardController eventCard = leftThreatArea.GetChild(0).GetComponent<EventCardController>();
            if (eventCard != null)
            {
                threats.Add(eventCard);
            }
        }
        if (rightThreatArea.childCount > 0 && rightThreatArea.GetChild(0) != null)
        {
            EventCardController eventCard = rightThreatArea.GetChild(0).GetComponent<EventCardController>();
            if (eventCard != null)
            {
                threats.Add(eventCard);
            }
        }
        foreach (EventCardController threat in threats)
        {
            EventCard eventCard = threat.GetEventCard();
            foreach (CardEffect failureEffect in eventCard.failureEffects)
            {
                failureEffect.ApplyEffect();
            }
        }
        foreach (EventCardController threat in threats)
        {
            Destroy(threat.gameObject);
        }
    }
}
