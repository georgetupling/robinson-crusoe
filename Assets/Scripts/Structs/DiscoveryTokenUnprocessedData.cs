using System.Collections;
using System.Collections.Generic;

public struct DiscoveryTokenUnprocessedData
{
    public string discovery;
    public string activationMessage;
    public List<string> effectsOnDraw;
    public bool canBeActivated;
    public bool activationRequiresWeapon;
    public bool activationRequiresPot;
    public List<string> effectsOnActivation;
}