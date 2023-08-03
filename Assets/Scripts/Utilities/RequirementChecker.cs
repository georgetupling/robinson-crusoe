using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Validates whether item build requirements are met. Currently duplicated in the BuildActionHandler, but I will move that code to here.
*/

public class RequirementChecker : MonoBehaviour
{
    public static RequirementChecker Singleton { get; private set; }

    private bool isWaitingForWeaponLevel = false;
    private int weaponLevel = 0;
    private int roofLevel = 0;
    private int palisadeLevel = 0;
    private bool shelterIsBuilt;
    private Dictionary<int, int> playerDetermination = new Dictionary<int, int>(); // Maps playerId to determination
    public bool sufficientResourcesAvailable;
    bool waitingOnAreSufficientResourcesAvailable;

    private List<Terrain> terrainRequirementsMet = new List<Terrain>();
    private List<Invention> itemRequirementsMet = new List<Invention>();

    // Building costs for the roof, palisade, and shelter

    private Dictionary<int, ResourceCost> buildingCostsByPlayerCount = new Dictionary<int, ResourceCost>() {
        { 1, ResourceCost.TwoWoodOrHide },
        { 2, ResourceCost.TwoWoodOrHide },
        { 3, ResourceCost.ThreeWoodOrTwoHide },
        { 4, ResourceCost.FourWoodOrThreeHide }
    };

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        EventGenerator.Singleton.AddListenerToGetWeaponLevelEvent(OnGetWeaponLevelEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
        EventGenerator.Singleton.AddListenerToTerrainRequirementMetEvent(OnTerrainRequirementMetEvent);
        EventGenerator.Singleton.AddListenerToShelterIsBuiltResponseEvent(OnShelterIsBuiltResponseEvent);
        EventGenerator.Singleton.AddListenerToGetRoofLevelResponseEvent(OnGetRoofLevelResponseEvent);
        EventGenerator.Singleton.AddListenerToGetPalisadeLevelResponseEvent(OnGetPalisadeLevelResponseEvent);
        EventGenerator.Singleton.AddListenerToGetDeterminationResponseEvent(OnGetDeterminationResponseEvent);
        EventGenerator.Singleton.AddListenerToAreSufficientResourcesAvailableResponseEvent(OnAreSufficentResourcesAvailableResponseEvent);
    }

    // Listeners

    void OnAreSufficentResourcesAvailableResponseEvent(bool response)
    {
        if (waitingOnAreSufficientResourcesAvailable)
        {
            sufficientResourcesAvailable = response;
            waitingOnAreSufficientResourcesAvailable = false;
        }
    }

    void OnGetWeaponLevelEvent(string eventType, int response)
    {
        if (eventType == GetWeaponLevelEvent.Response && isWaitingForWeaponLevel)
        {
            weaponLevel = response;
            isWaitingForWeaponLevel = false;
        }
    }

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt)
    {
        if (isBuilt)
        {
            itemRequirementsMet.Add(invention);
        }
        else
        {
            itemRequirementsMet.Remove(invention);
        }
    }

    void OnTerrainRequirementMetEvent(Terrain terrainType, bool requirementMet)
    {
        if (requirementMet)
        {
            terrainRequirementsMet.Add(terrainType);
        }
        else
        {
            terrainRequirementsMet.Remove(terrainType);
        }
    }

    void OnShelterIsBuiltResponseEvent(bool shelterIsBuilt)
    {
        this.shelterIsBuilt = shelterIsBuilt;
    }

    void OnGetRoofLevelResponseEvent(int roofLevel)
    {
        this.roofLevel = roofLevel;
    }

    void OnGetPalisadeLevelResponseEvent(int palisadeLevel)
    {
        this.palisadeLevel = palisadeLevel;
    }

    void OnGetDeterminationResponseEvent(int playerId, int determination)
    {
        if (playerDetermination.ContainsKey(playerId))
        {
            playerDetermination[playerId] = determination;
        }
        else
        {
            playerDetermination.Add(playerId, determination);
        }
    }

    // Public methods

    public bool InventionRequirementsMet(int playerId, InventionCardController inventionCardController)
    {
        InventionCard inventionCard = inventionCardController.GetInventionCard();
        if (inventionCard == null)
        {
            Debug.LogError("Parameter inventionCard is null.");
            return false;
        }
        if (inventionCard.terrainTypeRequirement != Terrain.None && !terrainRequirementsMet.Contains(inventionCard.terrainTypeRequirement))
        {
            return false;
        }
        foreach (Invention itemRequirement in inventionCard.itemRequirements)
        {
            if (!itemRequirementsMet.Contains(itemRequirement))
            {
                return false;
            }
        }
        waitingOnAreSufficientResourcesAvailable = true;
        EventGenerator.Singleton.RaiseAreSufficientResourcesAvailableEvent();
        if (!sufficientResourcesAvailable)
        {
            return false;
        }
        if (inventionCard.isPersonalInvention)
        {
            if (playerId != inventionCardController.GetPlayerId())
            {
                return false;
            }
            EventGenerator.Singleton.RaiseGetDeterminationEvent(playerId);
            if (playerDetermination[playerId] < 2)
            {
                return false;
            }
        }
        return true;
    }

    public bool BuildRequirementsMet(ActionType actionType)
    {
        // TODO: check for additional wood token
        ResourceCost resourceCost;
        if (actionType == ActionType.BuildWeapon)
        {
            resourceCost = ResourceCost.Wood;
        }
        else
        {
            resourceCost = buildingCostsByPlayerCount[GameSettings.PlayerCount];
        }
        waitingOnAreSufficientResourcesAvailable = true;
        EventGenerator.Singleton.RaiseAreSufficientResourcesAvailableEvent();
        while (waitingOnAreSufficientResourcesAvailable)
        {
            // Do nothing
        }
        if (!sufficientResourcesAvailable)
        {
            return false;
        }

        // Checks if the shelter is already built or the roof/palisade/Weapon is at maximum
        if (actionType == ActionType.BuildShelter)
        {
            EventGenerator.Singleton.RaiseShelterIsBuiltEvent();
            if (shelterIsBuilt)
            {
                return false;
            }
        }
        else if (actionType == ActionType.BuildRoof)
        {
            EventGenerator.Singleton.RaiseGetRoofLevelEvent();
            const int maximumRoofLevel = 4;
            if (roofLevel >= maximumRoofLevel)
            {
                return false;
            }
        }
        else if (actionType == ActionType.BuildPalisade)
        {
            EventGenerator.Singleton.RaiseGetPalisadeLevelEvent();
            const int maximumPalisadeLevel = 4;
            if (palisadeLevel >= maximumPalisadeLevel)
            {
                return false;
            }
        }
        else if (actionType == ActionType.BuildWeapon)
        {
            StartCoroutine(UpdateWeaponLevel());
            const int maximumWeaponLevel = 10;
            if (weaponLevel >= maximumWeaponLevel)
            {
                return false;
            }
        }
        return true;
    }

    public bool ThreatRequirementsMet(EventCard eventCard)
    {
        if (eventCard == null)
        {
            Debug.LogError("Parameter eventCard is null.");
            return false;
        }
        foreach (Invention itemRequirement in eventCard.threatItemRequirements)
        {
            if (!itemRequirementsMet.Contains(itemRequirement))
            {
                return false;
            }
        }
        StartCoroutine(UpdateWeaponLevel());
        if (weaponLevel < eventCard.threatWeaponRequirement)
        {
            return false;
        }
        waitingOnAreSufficientResourcesAvailable = true;
        EventGenerator.Singleton.RaiseAreSufficientResourcesAvailableEvent();
        while (waitingOnAreSufficientResourcesAvailable)
        {
            // Do nothing
        }
        if (!sufficientResourcesAvailable)
        {
            return false;
        }
        return true;
    }

    // Helper methods

    IEnumerator UpdateWeaponLevel()
    {
        isWaitingForWeaponLevel = true;
        EventGenerator.Singleton.RaiseGetWeaponLevelEvent();
        while (isWaitingForWeaponLevel)
        {
            yield return null;
        }
    }

}
