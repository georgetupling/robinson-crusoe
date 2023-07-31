using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// This class also spawns/despawns the black marker tokens which indicate an ability has been used!

public class AbilityAreaClickHandler : MonoBehaviour, IPointerClickHandler
{
    int playerId;
    Ability ability;

    [SerializeField] Transform blackMarkerPosition;

    void Awake() {
        EventGenerator.Singleton.AddListenerToAbilityActivatedEvent(OnAbilityActivatedEvent);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
    }

    void OnAbilityActivatedEvent(int playerId, Ability ability) {
        if (playerId == this.playerId && ability == this.ability) {
            TokenController blackMarkerPrefab = PrefabLoader.Singleton.GetPrefab(TokenType.BlackMarker);
            TokenController spawnedToken = Instantiate(blackMarkerPrefab, blackMarkerPosition, false);
            float tokenHeight = ComponentDimensions.GetHeight(TokenType.BlackMarker);
            spawnedToken.transform.localPosition = new Vector3(0, 0, -tokenHeight / 2f);
        }
    }

    void OnTurnStartEvent(int turnStarted) {
        if (blackMarkerPosition.childCount > 0) {
            TokenController token = blackMarkerPosition.GetChild(0).GetComponent<TokenController>();
            if (token != null) {
                Destroy(token.gameObject);
            }
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        if (ability == null) {
            Debug.LogError("Ability is null.");
            return;
        }
        if (blackMarkerPosition.childCount > 0) {
            return;
        }
        EventGenerator.Singleton.RaiseSpawnAbilityPopupEvent(playerId, ability);
    }

    public void Initialize(int playerId, Ability ability) {
        this.playerId = playerId;
        this.ability = ability;
    }
}
