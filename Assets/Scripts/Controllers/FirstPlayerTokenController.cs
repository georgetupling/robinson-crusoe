using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPlayerTokenController : ComponentController
{
    [SerializeField] Transform position0;
    [SerializeField] Transform position1;
    [SerializeField] Transform position2;
    [SerializeField] Transform position3;
    List<Transform> positions;

    protected override void Awake()
    {
        base.Awake();
        positions = new List<Transform>() { position0, position1, position2, position3 };
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
    }

    void OnTurnStartEvent(int turnStarted)
    {
        Transform nextPosition = positions[turnStarted % GameSettings.PlayerCount];
        MoveToTransform(nextPosition, MoveStyle.Default);
    }
}
