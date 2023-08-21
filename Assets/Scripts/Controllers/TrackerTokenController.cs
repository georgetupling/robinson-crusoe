using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TokenController;

/*
    This class controls the movement of a single tracker token.
    The morale, roof, palisade, weapon trackers, and turn trackers are initialized automatically at start-up.
    Health trackers must be initialized via an event.
*/

public class TrackerTokenController : TokenController
{
    private Dictionary<int, Vector3> positions = new Dictionary<int, Vector3>();

    private string eventTypeToListenFor;
    private int minimumValue;
    private int maximumValue;

    // These fields are for health trackers only
    private int playerId;
    private bool isInitialized;

    protected override void Awake()
    {
        base.Awake();
        if (this.tokenType == TokenType.HealthTracker)
        {
            EventGenerator.Singleton.AddListenerToInitializeHealthTrackerTokenEvent(OnInitializeHealthTrackerTokenEvent);
        }
        else
        {
            InitializeEventTypeToListenFor();
            InitializeMinAndMaxValues();
            InitializePositions();
        }
    }

    protected override void Start()
    {
        base.Start();
        EventGenerator.Singleton.AddListenerToTrackerTokenEvent(OnTrackerTokenEvent);
    }

    void InitializePositions()
    {
        for (int i = minimumValue; i <= maximumValue; i++)
        {
            int positionNumber = i - minimumValue; // if the minimum value is -3 => position 0 represents value -3, position 1 represents value -2, etc...
            string transformName = "Position" + positionNumber;
            Transform positionTransform = transform.parent.Find(transformName);
            if (positionTransform == null)
            {
                Debug.LogError($"{transformName} not found as child of parent transform.");
                return;
            }
            positions.Add(positionNumber, positionTransform.localPosition);
        }
    }

    void InitializeEventTypeToListenFor()
    {
        switch (this.tokenType)
        {
            case TokenType.MoraleTracker: eventTypeToListenFor = TrackerTokenEvent.SetMoraleTracker; break;
            case TokenType.PalisadeTracker: eventTypeToListenFor = TrackerTokenEvent.SetPalisadeTracker; break;
            case TokenType.RoofTracker: eventTypeToListenFor = TrackerTokenEvent.SetRoofTracker; break;
            case TokenType.WeaponTracker: eventTypeToListenFor = TrackerTokenEvent.SetWeaponTracker; break;
            case TokenType.TurnTracker: eventTypeToListenFor = TrackerTokenEvent.SetTurnTracker; break;
            case TokenType.FridayHealthTracker: eventTypeToListenFor = TrackerTokenEvent.SetFridayHealthTracker; break;
            case TokenType.HealthTracker:
                switch (playerId)
                {
                    case 0: eventTypeToListenFor = TrackerTokenEvent.SetPlayer0HealthTracker; break;
                    case 1: eventTypeToListenFor = TrackerTokenEvent.SetPlayer1HealthTracker; break;
                    case 2: eventTypeToListenFor = TrackerTokenEvent.SetPlayer2HealthTracker; break;
                    case 3: eventTypeToListenFor = TrackerTokenEvent.SetPlayer3HealthTracker; break;
                }
                break;
        }
    }

    void InitializeMinAndMaxValues()
    {
        switch (this.tokenType)
        {
            case TokenType.MoraleTracker: minimumValue = -3; maximumValue = 3; break;
            case TokenType.PalisadeTracker: minimumValue = 0; maximumValue = 4; break;
            case TokenType.RoofTracker: minimumValue = 0; maximumValue = 4; break;
            case TokenType.WeaponTracker: minimumValue = 0; maximumValue = 10; break;
            case TokenType.FridayHealthTracker: minimumValue = 0; maximumValue = 4; break;
            case TokenType.TurnTracker:
                minimumValue = 0;
                switch (GameSettings.CurrentScenario)
                {
                    case Scenario.Castaways: maximumValue = 11; break;
                    default: Debug.LogError($"No maximum turn value set for scenario {GameSettings.CurrentScenario}."); maximumValue = 11; break;
                }
                break;
        }
    }

    void OnInitializeHealthTrackerTokenEvent(int componentId, int playerId, int maximumHealth)
    {
        if (componentId == this.ComponentId)
        {
            InitializeHealthTrackerToken(playerId, maximumHealth);
        }
    }

    void InitializeHealthTrackerToken(int playerId, int maximumHealth)
    {
        if (isInitialized)
        {
            Debug.LogError("Health tracker token is already initialized.");
            return;
        }
        this.playerId = playerId;
        minimumValue = 0;
        maximumValue = maximumHealth;
        InitializeEventTypeToListenFor();
        InitializePositions();
        isInitialized = true;
    }

    void OnTrackerTokenEvent(string eventType, int newValue)
    {
        if (eventType == eventTypeToListenFor)
        {
            MoveToTrackerPosition(newValue);
        }
    }

    void MoveToTrackerPosition(int newValue)
    {
        int positionNumber = newValue - minimumValue;
        if (!positions.ContainsKey(positionNumber))
        {
            Debug.LogError($"positions dictionary does not contain key {positionNumber}.");
            return;
        }
        MoveToLocalPosition(positions[positionNumber]);
    }

}
