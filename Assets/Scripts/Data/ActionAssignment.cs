using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionAssignment
{
    public ActionType Type;
    public List<int> playerIds;
    public int numberOfActions;
    public bool mustRoll;
    public List<ResourceCost> resourceCosts;
    public List<int> pawnComponentIds;

    // Gather only
    public IslandTile islandTile;
    public bool isRightSource;

    // Explore only
    public int locationId;

    // Build only
    bool buildingWithHide;
    public InventionCard inventionCard;

    // Threat only
    public EventCard eventCard;
    public bool isTwoActionThreat;
    public int eventCardControllerComponentId;
}
