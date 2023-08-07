using UnityEngine;

public class ScenarioSheetSpawner : MonoBehaviour
{
    void Start()
    {
        Transform scenarioSheetPrefab = PrefabLoader.Singleton.GetPrefab(GameSettings.CurrentScenario);
        Instantiate(scenarioSheetPrefab, transform, false);
    }
}
