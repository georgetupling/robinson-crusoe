using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager singleton;

    public List<Player> players = new List<Player>();

    private int playerCount;
    private int currentFirstPlayer;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
        playerCount = GameSettings.PlayerCount;
        InitializePlayers();
        EventGenerator.Singleton.AddListenerToHealthEvent(OnHealthEvent);
        EventGenerator.Singleton.AddListenerToDeterminationEvent(OnDeterminationEvent);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
        EventGenerator.Singleton.AddListenerToGetFirstPlayerEvent(OnGetFirstPlayerEvent);
        EventGenerator.Singleton.AddListenerToGetDeterminationEvent(OnGetDeterminationEvent);
    }

    void InitializePlayers() {
        for (int i = 0; i < playerCount; i++) {
            CharacterType characterType = GameSettings.PlayerCharacters[i];
            Character character = CharacterFactory.CreateCharacter(characterType);
            string playerName = GameSettings.PlayerNames[i];
            Player newPlayer = new Player(i, playerName, character);
            players.Add(newPlayer);
        }
    }

    void OnHealthEvent(string eventType, int playerId, int amount) {
        if (eventType == HealthEvent.LoseHealth) {
            amount *= -1;
        }
        if (playerId == HealthEvent.AllPlayers) {
            for (int i = 0; i < playerCount; i++) {
                ModifyHealth(i, amount);
            }
        } else if (playerId == HealthEvent.FirstPlayer) {
            ModifyHealth(currentFirstPlayer, amount);
        } else {
            ModifyHealth(playerId, amount);
        }
    }

    void OnDeterminationEvent(string eventType, int playerId, int amount) {
        if (eventType == DeterminationEvent.LoseDetermination) {
            amount *= -1;
        }
        if (playerId == DeterminationEvent.AllPlayers) {
            for (int i = 0; i < playerCount; i++) {
                ModifyDetermination(i, amount);
            }
        } else if (playerId == DeterminationEvent.FirstPlayer) {
            ModifyDetermination(currentFirstPlayer, amount);
        } else {
            ModifyDetermination(playerId, amount);
        }
    }

    void OnTurnStartEvent(int turnNumber) {
        currentFirstPlayer = (currentFirstPlayer + 1) % playerCount;
    }

    void OnGetFirstPlayerEvent(string eventType, int playerId) {
        if (eventType == GetFirstPlayerEvent.Query) {
            EventGenerator.Singleton.RaiseGetFirstPlayerResponseEvent(currentFirstPlayer);
        }
    }

    void OnGetDeterminationEvent(int playerId) {
        Player foundPlayer = players.Find(x => x.id == playerId);
        if (foundPlayer != null) {
            EventGenerator.Singleton.RaiseGetDeterminationResponseEvent(playerId, foundPlayer.determination);
        }
    }

    Player GetPlayer(int playerId) {
        if (playerId < 0 || playerId >= playerCount) {
            Debug.LogError("Invalid player ID.");
            return null;
        }
        return players.Find(player => player.id == playerId);
    }

    void ModifyDetermination(int playerId, int amount) {
        Player player = GetPlayer(playerId);
        if (player != null) {
            player.ModifyDetermination(amount);
        }
    }

    void ModifyHealth(int playerId, int amount) {
        Player player = GetPlayer(playerId);
        if (player != null) {
            player.ModifyHealth(amount);
        }
    }
}
