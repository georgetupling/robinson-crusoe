using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeCampCardEnabler : MonoBehaviour
{
    void Awake()
    {
        if (GameSettings.PlayerCount < 4)
        {
            gameObject.SetActive(false);
        }
    }
}
