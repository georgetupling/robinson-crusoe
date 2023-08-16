using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static Invention;
using static Terrain;
using static Source;
using static ResourceCost;

public static class EnumParser
{
    public static Equipment ParseEquipment(string equipmentString)
    {
        Equipment equipment;
        if (Enum.TryParse(equipmentString, out equipment))
        {
            return equipment;
        }
        else
        {
            Debug.LogError("Invalid equipment type: " + equipmentString);
            return Equipment.Biscuits; // Default value
        }
    }
    
    public static Invention ParseInvention(string inventionString)
    {
        Invention invention;
        if (Enum.TryParse(inventionString, out invention))
        {
            return invention;
        }
        else
        {
            Debug.LogError("Invalid invention type: " + inventionString);
            return Invention.Map; // Default value
        }
    }

    public static List<Invention> ParseInventionList(List<string> inventionStrings)
    {
        List<Invention> inventions = new List<Invention>();
        foreach (string inventionString in inventionStrings)
        {
            Invention invention;
            if (Enum.TryParse(inventionString, out invention))
            {
                inventions.Add(invention);
            }
            else
            {
                Debug.LogError("Invalid invention type: " + inventionString);
            }
        }
        return inventions;
    }

    public static Discovery ParseDiscovery(string discoveryString)
    {
        Discovery discovery;
        if (Enum.TryParse(discoveryString, out discovery))
        {
            return discovery;
        }
        else
        {
            Debug.LogError("Invalid discovery type: " + discoveryString);
            return Discovery.FailedToParse;
        }
    }

    public static List<ResourceCost> ParseResourceCostList(List<string> resourceCostStrings)
    {
        List<ResourceCost> resourceCosts = new List<ResourceCost>();
        foreach (string resourceCostString in resourceCostStrings)
        {
            ResourceCost resourceCost;
            string modifiedString = resourceCostString.Replace("2", "Two").Replace("3", "Three").Replace("4", "Four");
            if (Enum.TryParse(modifiedString, out resourceCost))
            {
                resourceCosts.Add(resourceCost);
            }
            else
            {
                Debug.LogError("Invalid resource cost type: " + resourceCostString);
            }
        }
        return resourceCosts;
    }

    public static Terrain ParseTerrain(string terrainString)
    {
        Terrain terrain;
        if (Enum.TryParse(terrainString, out terrain))
        {
            return terrain;
        }
        else
        {
            Debug.LogError("Invalid terrain type: " + terrainString);
            return Terrain.None; // Default value
        }
    }

    public static List<Source> ParseSourceList(List<string> sourceStrings)
    {
        List<Source> sources = new List<Source>();
        foreach (string sourceString in sourceStrings)
        {
            Source source;
            if (Enum.TryParse(sourceString, out source))
            {
                sources.Add(source);
            }
            else
            {
                Debug.LogError("Invalid source type: " + sourceString);
            }
        }
        return sources;
    }

    public static CardSymbol ParseCardSymbol(string cardSymbolString)
    {
        CardSymbol cardSymbol;
        if (CardSymbol.TryParse(cardSymbolString, out cardSymbol))
        {
            return cardSymbol;
        }
        else if (cardSymbolString == null || cardSymbolString == "")
        {
            Debug.Log($"No card symbol provided.");
        }
        else
        {
            Debug.Log($"{cardSymbolString} is not a valid card symbol.");
        }
        return CardSymbol.None; // Default value
    }

    public static AdventureType ParseAdventureType(string str)
    {
        AdventureType adventureType;
        if (AdventureType.TryParse(str, out adventureType))
        {
            return adventureType;
        }
        else if (str == null || str == "")
        {
            Debug.Log($"No adventure type provided.");
        }
        else
        {
            Debug.Log($"{str} is not a valid adventure type.");
        }
        return AdventureType.Build; // Default value
    }
}
