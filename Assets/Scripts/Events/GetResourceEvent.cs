using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GetResourceEvent : UnityEvent<string, int>
{
    public const string GetHide = "GetHide";
    public const string GetFood = "GetFood";
    public const string GetNonPerishableFood = "GetNonPerishableFood";
    public const string GetWood = "GetWood";

    public const string GetHideResponse = "GetHideResponse";
    public const string GetFoodResponse = "GetFoodResponse";
    public const string GetNonPerishableFoodResponse = "GetNonPerishableFoodResponse";
    public const string GetWoodResponse = "GetWoodResponse";
}
