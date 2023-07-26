using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static ResourceType;
using static TokenType;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager singleton;

    [SerializeField] private Transform availableResourcesArea;
    [SerializeField] private Transform futureResourcesArea;

    private Dictionary<ResourceType, int> availableResources = new Dictionary<ResourceType, int>();
    private Dictionary<ResourceType, int> futureResources = new Dictionary<ResourceType, int>();

    private List<TokenController> availableResourceTokens = new List<TokenController>();
    private List<TokenController> futureResourceTokens = new List<TokenController>();

    private bool isActionPhase;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
        InitializeResources();
        EventGenerator.Singleton.AddListenerToResourceEvent(OnResourceEvent);
        EventGenerator.Singleton.AddListenerToGetResourceEvent(OnGetResourceEvent);
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
    }

    void InitializeResources() {
        availableResources.Add(ResourceType.Food, 0);
        availableResources.Add(ResourceType.Hide, 0);
        availableResources.Add(ResourceType.NonPerishableFood, 0);
        availableResources.Add(ResourceType.Wood, 0);

        futureResources.Add(ResourceType.Food, 0);
        futureResources.Add(ResourceType.Hide, 0);
        futureResources.Add(ResourceType.NonPerishableFood, 0);
        futureResources.Add(ResourceType.Wood, 0);
    }

    void OnResourceEvent(string eventType, int amount) {
        switch (eventType) {
            case ResourceEvent.GainFood: GainResource(ResourceType.Food, amount); break;
            case ResourceEvent.LoseFood: LoseResource(ResourceType.Food, amount); break;
            case ResourceEvent.GainHide: GainResource(ResourceType.Hide, amount); break;
            case ResourceEvent.LoseHide: LoseResource(ResourceType.Hide, amount); break;
            case ResourceEvent.GainNonPerishableFood: GainResource(ResourceType.NonPerishableFood, amount); break;
            case ResourceEvent.LoseNonPerishableFood: LoseResource(ResourceType.NonPerishableFood, amount); break;
            case ResourceEvent.GainWood: GainResource(ResourceType.Wood, amount); break;
            case ResourceEvent.LoseWood: LoseResource(ResourceType.Wood, amount); break;
            case ResourceEvent.MakeResourcesAvailable: MakeResourcesAvailable(); break;
        }
    }

    void OnGetResourceEvent(string eventType, int amount) {
        switch (eventType) {
            case GetResourceEvent.GetHide: EventGenerator.Singleton.RaiseGetHideResponseEvent(availableResources[ResourceType.Hide]); break;
            case GetResourceEvent.GetFood: EventGenerator.Singleton.RaiseGetFoodResponseEvent(availableResources[ResourceType.Food]); break;
            case GetResourceEvent.GetNonPerishableFood: EventGenerator.Singleton.RaiseGetNonPerishableFoodResponseEvent(availableResources[ResourceType.NonPerishableFood]); break;
            case GetResourceEvent.GetWood: EventGenerator.Singleton.RaiseGetWoodResponseEvent(availableResources[ResourceType.Wood]); break;
        }
    }

    void OnPhaseStartEvent(Phase phase) {
        isActionPhase = phase == Phase.Action;
    }

    void GainResource(ResourceType resourceType, int amount) {
        if (isActionPhase) {
            futureResources[resourceType] += amount;
            SpawnTokens(futureResourcesArea, PrefabLoader.Singleton.GetPrefab(resourceType), futureResourceTokens, amount);
        } else {
            availableResources[resourceType] += amount;
            SpawnTokens(availableResourcesArea, PrefabLoader.Singleton.GetPrefab(resourceType), availableResourceTokens, amount);
        }
    }

    void LoseResource(ResourceType resourceType, int amount) {
        availableResources[resourceType] -= amount;
        if (availableResources[resourceType] < 0) {
            EventGenerator.Singleton.RaiseAllLoseHealthEvent(-availableResources[resourceType]);
            availableResources[resourceType] = 0;
        }
        if (availableResourceTokens.Count > 0) {
            for (int i = 0; i < amount; i++) {
                DeleteTokenOfType(resourceType, availableResourceTokens);
            }
        }
    }

    void MakeResourcesAvailable() {
        foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType))) {
            availableResources[resourceType] += futureResources[resourceType];
            futureResources[resourceType] = 0;
        }
        foreach (TokenController token in futureResourceTokens) {
            availableResourceTokens.Add(token);
        }
        futureResourceTokens.RemoveAll(x => (x != null && x.transform.parent == availableResourcesArea));
    }

    void SpawnTokens(Transform resourcesArea, TokenController prefab, List<TokenController> listOfTokens, int amount) {
        for (int i= 0; i < amount; i++) {
            SpawnToken(resourcesArea, prefab, listOfTokens);
        }
    }

    private void SpawnToken(Transform resourcesArea, TokenController prefab, List<TokenController> listOfTokens) {
        if (prefab == null || resourcesArea == null) {
            Debug.Log("Prefab or resource area is null. Cannot spawn token.");
            return;
        }
        TokenController newToken = Instantiate(prefab, resourcesArea, false);
        // Randomizes the position of the token within the resource area
        float randX = Random.Range(-0.3f, 0.3f);
        float randY = Random.Range(-0.2f, 0.2f);
        float randRotation = Random.Range(0, 360);
        newToken.transform.localPosition = new Vector3(randX, randY, 0);
        newToken.transform.rotation = Quaternion.Euler(0, 0, randRotation);
        TokenPositioner.PositionTokens(resourcesArea);
        listOfTokens.Add(newToken);
    }

    private void DeleteTokenOfType(ResourceType resourceType, List<TokenController> listOfTokens) {
        TokenType tokenType;
        switch (resourceType) {
            case ResourceType.Food: tokenType = TokenType.Food; break;
            case ResourceType.Hide: tokenType = TokenType.Hide; break;
            case ResourceType.NonPerishableFood: tokenType = TokenType.NonPerishableFood; break;
            case ResourceType.Wood: tokenType = TokenType.Wood; break;
            default: return;
        }
        TokenController tokenToDelete = listOfTokens.Find(x => x.tokenType == tokenType);
        if (tokenToDelete == null) {
            Debug.Log("Failed to delete resource token.");
            return;
        }
        listOfTokens.Remove(tokenToDelete);
        Destroy(tokenToDelete.gameObject);
    }
}