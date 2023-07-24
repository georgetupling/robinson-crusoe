using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Validates whether item build requirements are met. Currently duplicated in the BuildActionHandler, but I will move that code to here.
*/

public class RequirementChecker : MonoBehaviour
{
    public static RequirementChecker Singleton { get; private set; }

    private bool isWaitingForHideAvailable = false;
    private bool isWaitingForWoodAvailable = false;
    private bool isWaitingForWeaponLevel = false;
    private int hideAvailable = 0;
    private int woodAvailable = 0;
    private int weaponLevel = 0;
    private int roofLevel = 0;
    private int palisadeLevel = 0;
    private bool shelterIsBuilt;

    private List<Terrain> terrainRequirementsMet = new List<Terrain>();
    private List<Invention> itemRequirementsMet = new List<Invention>();

    // Building costs for the roof, palisade, and shelter

    private Dictionary<int, ResourceCost> buildingCostsByPlayerCount = new Dictionary<int, ResourceCost>() {
        { 1, ResourceCost.TwoWoodOrHide },
        { 2, ResourceCost.TwoWoodOrHide },
        { 3, ResourceCost.ThreeWoodOrTwoHide },
        { 4, ResourceCost.FourWoodOrThreeHide }
    };

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
        }
        EventGenerator.Singleton.AddListenerToGetResourceEvent(OnGetResourceEvent);
        EventGenerator.Singleton.AddListenerToGetWeaponLevelEvent(OnGetWeaponLevelEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
        EventGenerator.Singleton.AddListenerToTerrainRequirementMetEvent(OnTerrainRequirementMetEvent);
        EventGenerator.Singleton.AddListenerToShelterIsBuiltResponseEvent(OnShelterIsBuiltResponseEvent);
        EventGenerator.Singleton.AddListenerToGetRoofLevelResponseEvent(OnGetRoofLevelResponseEvent);
        EventGenerator.Singleton.AddListenerToGetPalisadeLevelResponseEvent(OnGetPalisadeLevelResponseEvent);
    }

    // Listeners

    void OnGetResourceEvent(string eventType, int amount) {
        if (eventType == GetResourceEvent.GetHideResponse) {
            hideAvailable = amount;
            isWaitingForHideAvailable = false;
        } else if (eventType == GetResourceEvent.GetWoodResponse) {
            woodAvailable = amount;
            isWaitingForWoodAvailable = false;
        }
    }

    void OnGetWeaponLevelEvent(string eventType, int response) {
        if (eventType == GetWeaponLevelEvent.Response && isWaitingForWeaponLevel) {
            weaponLevel = response;
            isWaitingForWeaponLevel =  false;
        }
    }

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt) {
        if (isBuilt) {
            itemRequirementsMet.Add(invention);
        } else {
            itemRequirementsMet.Remove(invention);
        }
    }

    void OnTerrainRequirementMetEvent(Terrain terrainType, bool requirementMet) {
        if (requirementMet) {
            terrainRequirementsMet.Add(terrainType);
        } else {
            terrainRequirementsMet.Remove(terrainType);
        }
    }

    void OnShelterIsBuiltResponseEvent(bool shelterIsBuilt) {
        this.shelterIsBuilt = shelterIsBuilt;
    }

    void OnGetRoofLevelResponseEvent(int roofLevel) {
        this.roofLevel = roofLevel;
    }

    void OnGetPalisadeLevelResponseEvent(int palisadeLevel) {
        this.palisadeLevel = palisadeLevel;
    }

    // Public methods

    public bool InventionRequirementsMet(InventionCard inventionCard) {
        if (inventionCard == null) {
            Debug.LogError("Parameter inventionCard is null.");
            return false;
        }
        if (inventionCard.terrainTypeRequirement != Terrain.None && !terrainRequirementsMet.Contains(inventionCard.terrainTypeRequirement)) {
            return false;
        }
        foreach (Invention itemRequirement in inventionCard.itemRequirements) {
            if (!itemRequirementsMet.Contains(itemRequirement)) {
                return false;
            }
        }
        StartCoroutine(UpdateResources());
        if (!SufficientResourcesAvailable(inventionCard.resourceCosts)) {
            return false;
        }
        return true;
    }

    public bool BuildRequirementsMet(ActionType actionType) {
        // TODO: check for additional wood token
        ResourceCost resourceCost;
        if (actionType == ActionType.BuildWeapon) {
            resourceCost = ResourceCost.Wood;
        } else {
            resourceCost = buildingCostsByPlayerCount[GameSettings.PlayerCount];
        }
        StartCoroutine(UpdateResources());
        if (!SufficientResourceAvailable(resourceCost)) {
            return false;
        }

        // Checks if the shelter is already built or the roof/palisade/Weapon is at maximum
        if (actionType == ActionType.BuildShelter) {
            EventGenerator.Singleton.RaiseShelterIsBuiltEvent();
            if (shelterIsBuilt) {
                return false;
            }
        } else if (actionType == ActionType.BuildRoof) {
            EventGenerator.Singleton.RaiseGetRoofLevelEvent();
            const int maximumRoofLevel = 4;
            if (roofLevel >= maximumRoofLevel) {
                return false;
            }
        } else if (actionType == ActionType.BuildPalisade) {
            EventGenerator.Singleton.RaiseGetPalisadeLevelEvent();
            const int maximumPalisadeLevel = 4;
            if (palisadeLevel >= maximumPalisadeLevel) {
                return false;
            }
        } else if (actionType == ActionType.BuildWeapon) {
            StartCoroutine(UpdateWeaponLevel());
            const int maximumWeaponLevel = 10;
            if (weaponLevel >= maximumWeaponLevel) {
                return false;
            }
        }
        return true;
    }

    public bool ThreatRequirementsMet(EventCard eventCard) {
        if (eventCard == null) {
            Debug.LogError("Parameter eventCard is null.");
            return false;
        }
        foreach (Invention itemRequirement in eventCard.threatItemRequirements) {
            if (!itemRequirementsMet.Contains(itemRequirement)) {
                return false;
            }
        }
        StartCoroutine(UpdateWeaponLevel());
        if (weaponLevel < eventCard.threatWeaponRequirement) {
            return false;
        }
        StartCoroutine(UpdateResources());
        if (!SufficientResourcesAvailable(eventCard.threatResourceCosts)) {
            return false;
        }
        return true;
    }

    // Helper methods

    IEnumerator UpdateResources() {
        isWaitingForHideAvailable = true;
        isWaitingForWoodAvailable = true;
        EventGenerator.Singleton.RaiseGetHideEvent();
        EventGenerator.Singleton.RaiseGetWoodEvent();
        while (isWaitingForHideAvailable || isWaitingForWoodAvailable) {
            yield return null;
        }
    }

    IEnumerator UpdateWeaponLevel() {
        isWaitingForWeaponLevel = true;
        EventGenerator.Singleton.RaiseGetWeaponLevelEvent();
        while (isWaitingForWeaponLevel) {
            yield return null;
        }
    }

    bool SufficientResourcesAvailable(List<ResourceCost> resourceCosts) {
        foreach (ResourceCost resourceCost in resourceCosts) {
            if (!SufficientResourceAvailable(resourceCost)) {
                return false;
            }
        }
        return true;
    }

    bool SufficientResourceAvailable(ResourceCost resourceCost) {
        switch(resourceCost) {
            case ResourceCost.Wood: if (woodAvailable < 1) return false; break;
            case ResourceCost.Hide: if (hideAvailable < 1) return false; break;
            case ResourceCost.TwoWood: if (woodAvailable < 2) return false; break;
            case ResourceCost.ThreeWood: if (woodAvailable < 3) return false; break;
            case ResourceCost.FourWood: if (woodAvailable < 4) return false; break;
            case ResourceCost.TwoHide: if (hideAvailable < 2) return false; break;
            case ResourceCost.ThreeHide: if (hideAvailable < 3) return false; break;
            case ResourceCost.TwoWoodOrHide: if (woodAvailable < 2 && hideAvailable < 1) return false; break;
            case ResourceCost.ThreeWoodOrTwoHide: if (woodAvailable < 3 && hideAvailable < 2) return false; break;
            case ResourceCost.FourWoodOrThreeHide: if (woodAvailable < 4 && hideAvailable < 3) return false; break;
            case ResourceCost.WoodUnlessShovel: if (!itemRequirementsMet.Contains(Invention.Shovel) && woodAvailable < 1) return false; break;
            default: Debug.LogError($"No method for validating resource cost {resourceCost}."); return false;
        }
        return true;
    }



}
