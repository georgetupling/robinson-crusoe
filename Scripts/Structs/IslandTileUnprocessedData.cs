using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct IslandTileUnprocessedData
{
    public int Id;
    public string TerrainType;
    public List<string> Sources;
    public bool HasTotem;
    public bool HasBeastCard;
    public bool HasNaturalShelter;
    public int NumberOfDiscoveryTokens;
}
