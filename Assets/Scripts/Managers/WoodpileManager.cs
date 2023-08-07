using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodpileManager : MonoBehaviour
{
    static WoodpileManager singleton;

    [SerializeField] TrackerTokenController trackerToken;

    public int woodpileLevel = 0; // Set to private later, only public for testing
    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogError("Scene contains duplicate WoodpileManager.");
            return;
        }
        EventGenerator.Singleton.AddListenerToGainWoodpileEvent(OnGainWoodpileEvent);
        EventGenerator.Singleton.AddListenerToGetWoodpileLevelEvent(OnGetWoodpileLevelEvent);
    }

    void OnGainWoodpileEvent()
    {
        woodpileLevel++;
        EventGenerator.Singleton.RaiseGetWoodpileLevelResponseEvent(woodpileLevel);
        if (trackerToken.gameObject.activeSelf == false)
        {
            trackerToken.gameObject.SetActive(true);
        }
        EventGenerator.Singleton.RaiseSetWoodpileTrackerEvent(woodpileLevel);
    }

    void OnGetWoodpileLevelEvent()
    {
        EventGenerator.Singleton.RaiseGetWoodpileLevelResponseEvent(woodpileLevel);
    }
}
