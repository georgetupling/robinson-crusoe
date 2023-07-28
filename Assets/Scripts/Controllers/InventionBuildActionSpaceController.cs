using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventionBuildActionSpaceController : MonoBehaviour
{
    Invention invention;
    bool inventionSet;

    void Awake() {
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
    }

    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt) {
        if (inventionSet && invention == this.invention) {
            gameObject.SetActive(!isBuilt);
        }
    }

    public void SetInvention(Invention invention) {
        this.invention = invention;
        inventionSet = true;
    }
}
