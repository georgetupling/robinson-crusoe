using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodpileManager : MonoBehaviour
{
    static WoodpileManager singleton;

    [SerializeField] Transform stage0;
    [SerializeField] Transform stage1;
    [SerializeField] Transform stage2;
    [SerializeField] Transform stage3;
    [SerializeField] Transform stage4;
    List<Transform> stages;

    List<int> maximumWoodByStage = new List<int> { 1, 2, 3, 4, 5 };

    public int woodAvailable = 0;
    public int woodInPile = 0; // Set to private later, only public for testing
    public int currentStage = 0;
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
        stages = new List<Transform> { stage0, stage1, stage2, stage3, stage4 };
        EventGenerator.Singleton.AddListenerToAddWoodToPileEvent(OnAddWoodToPileEvent);
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToGetResourceEvent(OnGetResourceEvent);
    }
    void OnAddWoodToPileEvent(int amount)
    {
        int amountToAdd = Mathf.Clamp(amount, 0, maximumWoodByStage[currentStage]);
        woodInPile += amountToAdd;
        SpawnWoodTokens(amountToAdd);
        if (woodInPile == maximumWoodByStage[currentStage])
        {
            currentStage++;
            woodInPile = 0;
            if (currentStage == 5)
            {
                EventGenerator.Singleton.RaiseWoodpileCompletedEvent();
            }
        }

    }
    void OnPhaseStartEvent(Phase phaseStarted)
    {
        if (phaseStarted != Phase.Action || currentStage == 5)
        {
            return;
        }
        EventGenerator.Singleton.RaiseGetWoodEvent();
        if (woodAvailable > 0)
        {
            int woodRequiredForNextStage = maximumWoodByStage[currentStage] - woodInPile;
            int woodLimit = Mathf.Min(woodAvailable, woodRequiredForNextStage);
            EventGenerator.Singleton.RaiseSpawnWoodpilePopupEvent(woodLimit);
        }
    }
    void OnGetResourceEvent(string eventType, int amount)
    {
        if (eventType == GetResourceEvent.GetWoodResponse)
        {
            woodAvailable = amount;
        }
    }

    // Helper methods

    void SpawnWoodTokens(int amountToAdd)
    {
        Transform stage = stages[currentStage];
        TokenController woodTokenPrefab = PrefabLoader.Singleton.GetPrefab(TokenType.Wood);
        float zPosition = ComponentDimensions.GetHeight(TokenType.Wood) / 2f;
        float spaceBetweenTokens = 0.06f;
        for (int i = 0; i < amountToAdd; i++)
        {
            TokenController newToken = Instantiate(woodTokenPrefab, stage, false);
            float yPosition = (stage.childCount - 1) * spaceBetweenTokens;
            newToken.transform.localPosition = new Vector3(0f, yPosition, zPosition);
            float zRotation = 30f + Random.Range(-5f, 5f);
            newToken.transform.eulerAngles = new Vector3(0f, 0f, zRotation);
        }
    }
}
