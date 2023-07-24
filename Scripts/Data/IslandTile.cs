using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class IslandTile
{
    public int Id { get; private set; }
    public Terrain TerrainType { get; private set; }
    public List<Source> Sources { get; private set; }
    public bool HasTotem { get; private set; }
    public bool HasBeastCard { get; private set; }
    public bool HasNaturalShelter { get; private set; }
    public int NumberOfDiscoveryTokens { get; private set; }
    public Material Material { get; private set; }
    public Material HighlightedMaterial { get; private set; }
    public Sprite Sprite { get; private set; }

    public IslandTile(IslandTileUnprocessedData tileData) {
        Id = tileData.Id;
        TerrainType = EnumParser.ParseTerrain(tileData.TerrainType);
        Sources = EnumParser.ParseSourceList(tileData.Sources);
        HasTotem = tileData.HasTotem;
        HasBeastCard = tileData.HasBeastCard;
        HasNaturalShelter = tileData.HasNaturalShelter;
        NumberOfDiscoveryTokens = tileData.NumberOfDiscoveryTokens;
        string materialName = "IslandTile" + tileData.Id + "Material"; 
        Material = Resources.Load<Material>(Path.Combine("Materials/Island Tiles", materialName));
        string highlightedMaterialName = "IslandTile" + tileData.Id + "HighlightedMaterial"; 
        HighlightedMaterial = Resources.Load<Material>(Path.Combine("Materials/Island Tiles", highlightedMaterialName));
        string spriteName = ResourceName.GetSpriteName("IslandTile" + tileData.Id);
        Sprite = Resources.Load<Sprite>(Path.Combine("Sprites/Island Tiles", spriteName));
    }
}
