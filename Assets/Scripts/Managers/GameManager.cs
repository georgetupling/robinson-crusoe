using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }

    public void IssueGameLoss() {
        // TODO
    }
}
