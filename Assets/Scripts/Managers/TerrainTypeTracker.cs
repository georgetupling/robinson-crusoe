using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTypeTracker : MonoBehaviour
{
    private static TerrainTypeTracker singleton;

    List<Terrain> revealedTerrainTypes = new List<Terrain>();

    void Awake() {
        if (singleton == null) {
            singleton = this;
        }
        EventGenerator.Singleton.AddListenerToTerrainRevealedEvent(OnTerrainRevealedEvent);
    }

    void OnTerrainRevealedEvent(Terrain terrainType, bool isRevealed) {
        if (isRevealed) {
            if (!revealedTerrainTypes.Contains(terrainType)) {
                EventGenerator.Singleton.RaiseTerrainRequirementMetEvent(terrainType, true);
            }
            revealedTerrainTypes.Add(terrainType);
        } else {
            revealedTerrainTypes.Remove(terrainType);
            if (!revealedTerrainTypes.Contains(terrainType)) {
                EventGenerator.Singleton.RaiseTerrainRequirementMetEvent(terrainType, false);
            }
        }
    }
}
