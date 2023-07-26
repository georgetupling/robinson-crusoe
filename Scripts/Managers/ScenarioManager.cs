using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Singleton;

    [SerializeField] private List<IScenario> scenarios;
    [SerializeField] private IScenario currentScenario; // Each scenario has its own script that implements IScenario

    private int turns;
    public int Turns => turns;

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
        turns = 12; // Set to 12 for testing purposes
    }
    
    public void InitializeScenario(int scenarioId) {
        // Sets the current scenario; method will be called by GameManager during start-up
        if (currentScenario == null) {
            currentScenario = scenarios[scenarioId];
            turns = currentScenario.turns;
        } else {
            Debug.Log("Scenario is not null.");
        }
    }

    public bool CheckWinCondition() {
        return currentScenario != null ? currentScenario.CheckWinCondition() : false;
    }

    public int GetStartingIslandTileId() {
        // TODO - implement properly
        return 8; // For testing purposes!
    }

    public int GetStartingIslandTileLocation() {
        // TODO - implement properly
        return 3; // For testing purposes!
    }
}
