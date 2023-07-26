using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ResourceEvent : UnityEvent<string, int>
{
    public const string GainFood = "GainFood";
    public const string LoseFood = "LoseFood";
    public const string GainHide = "GainHide";
    public const string LoseHide = "LoseHide";
    public const string GainNonPerishableFood = "GainNonPerishableFood";
    public const string LoseNonPerishableFood = "LoseNonPerishableFood";
    public const string GainWood = "GainWood";
    public const string LoseWood = "LoseWood";
    public const string MakeResourcesAvailable = "MakeResourcesAvailable";
}