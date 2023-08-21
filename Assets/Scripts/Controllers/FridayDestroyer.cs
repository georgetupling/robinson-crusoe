using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is attached to Friday's card and action pawn
// It deletes them when he dies :(

public class FridayDestroyer : MonoBehaviour
{
    void Awake()
    {
        EventGenerator.Singleton.AddListenerToFridayDiesEvent(OnFridayDiesEvent);
    }
    void OnFridayDiesEvent()
    {
        StartCoroutine(WaitForAnimationThenDelete());
    }

    IEnumerator WaitForAnimationThenDelete()
    {
        yield return new WaitForSeconds(GameSettings.AnimationDuration);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
