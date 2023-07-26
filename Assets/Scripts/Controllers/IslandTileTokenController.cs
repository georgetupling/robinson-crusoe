using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TokenController;

public class IslandTileTokenController : TokenController
{
    public enum Position { leftSource, rightSource, terrain, misc1, misc2, misc3, misc4, misc5, misc6, None  }

    private Dictionary<Position, Vector3> positions = new Dictionary<Position, Vector3>();

    protected override void Awake() {
        base.Awake();
        InitializePositions();
        EventGenerator.Singleton.AddListenerToIslandTileTokenEvent(OnIslandTileTokenEvent);
    }

    void InitializePositions() {
        string[] transformNames = new string[] { "leftSourceArea", "rightSourceArea", "terrainArea", "misc1Area", "misc2Area", "misc3Area", "misc4Area", "misc5Area", "misc6Area" };
        for (int i = 0; i < transformNames.Length; i++) {
            Transform positionTransform = transform.parent.Find(transformNames[i]);
            if (positionTransform == null) {
                Debug.LogError($"{transformNames[i]} not found as child of IslandTileController.");
                return;
            }
            positions.Add((Position) i, positionTransform.localPosition);
        }
    }

    void OnIslandTileTokenEvent(string eventType, int componentId, TokenType tokenType, Position position) {
        if (eventType == IslandTileTokenEvent.SetTokenPositionById && componentId == ComponentId) {
            MoveToIslandTilePosition(position);
        } else if (this.tokenType == TokenType.Camp && eventType == IslandTileTokenEvent.TurnCampTokenFaceDown) {
            TurnFaceDown();
        } else if (this.tokenType == TokenType.Camp && eventType == IslandTileTokenEvent.TurnCampTokenFaceUp) {
            TurnFaceUp();
        }
    }

    void MoveToIslandTilePosition(Position position) {
        if (!positions.ContainsKey(position)) {
            Debug.LogError($"position dictionary does not contain key {position}.");
        }
        MoveToLocalPosition(positions[position], MoveStyle.Instant);
    }
}
