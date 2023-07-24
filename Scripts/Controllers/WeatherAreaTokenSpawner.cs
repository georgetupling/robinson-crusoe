using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherAreaTokenSpawner : MonoBehaviour
{
    [SerializeField] Transform weatherArea;

    void Awake() {
        EventGenerator.Singleton.AddListenerToSpawnTokenInWeatherAreaEvent(OnSpawnTokenInWeatherAreaEvent);
    }

    void OnSpawnTokenInWeatherAreaEvent(TokenType tokenType) {
        TokenController prefab = PrefabLoader.Singleton.GetPrefab(tokenType);
        TokenController newToken = Instantiate(prefab, weatherArea, false);
        float randX = Random.Range(-0.3f, 0.3f);
        float randY = Random.Range(-0.2f, 0.2f);
        float halfTokenHeight = ComponentDimensions.GetHeight(tokenType) / 2f;
        newToken.transform.localPosition = new Vector3(randX, randY, -halfTokenHeight);
        TokenPositioner.PositionTokens(weatherArea);
    }
}
