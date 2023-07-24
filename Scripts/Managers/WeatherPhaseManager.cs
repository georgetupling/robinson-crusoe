using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherPhaseManager : MonoBehaviour
{
    static WeatherPhaseManager singleton;
    
    [SerializeField] Transform weatherArea;
    List<TokenController> weatherTokens = new List<TokenController>();
    List<TokenController> weatherDice = new List<TokenController>();

    int roofLevel;
    bool isWaitingOnRoofLevel;

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            return;
        }
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToGetRoofLevelResponseEvent(OnGetRoofLevelResponseEvent);
    }

    void OnPhaseStartEvent(Phase phaseStarted) {
        if (phaseStarted != Phase.Weather) {
            return;
        }
        StartCoroutine(ApplyWeatherPhase());
    }

    IEnumerator ApplyWeatherPhase() {
        isWaitingOnRoofLevel = true;
        EventGenerator.Singleton.RaiseGetRoofLevelEvent();
        while (isWaitingOnRoofLevel) {
            yield return null;
        }
        GetWeatherTokens();
        GetWeatherDice();
        // numberOfClouds[0] is the number of rain clouds, numberOfClouds[1] is the number of snow clouds
        List<int> numberOfClouds = DiceRoller.RollWeatherDice(weatherDice);
        int numberOfRainClouds = numberOfClouds[0];
        int numberOfSnowClouds = numberOfClouds[1];
        foreach (TokenController token in weatherTokens) {
            if (token.tokenType == TokenType.Storm) {
                EventGenerator.Singleton.RaiseLosePalisadeEvent(1);
            } else if (token.tokenType == TokenType.RainCloud) {
                numberOfRainClouds += 1;
            } else if (token.tokenType == TokenType.SnowCloud) {
                numberOfSnowClouds += 1;
            }
        }
        EventGenerator.Singleton.RaiseLoseWoodEvent(numberOfSnowClouds);
        numberOfRainClouds += numberOfSnowClouds;
        numberOfRainClouds -= roofLevel;
        if (numberOfRainClouds > 0) {
            EventGenerator.Singleton.RaiseLoseFoodEvent(numberOfRainClouds);
            EventGenerator.Singleton.RaiseLoseWoodEvent(numberOfRainClouds);
        }
        ClearWeatherArea();
        yield return new WaitForSeconds(1.5f);
        EventGenerator.Singleton.RaiseEndPhaseEvent(Phase.Weather);
    }

    void OnGetRoofLevelResponseEvent(int roofLevel) {
        if (isWaitingOnRoofLevel) {
            this.roofLevel = roofLevel;
            isWaitingOnRoofLevel = false;
        }
    }

    // Helper methods

    void GetWeatherTokens() {
        for (int i = 0; i < weatherArea.childCount; i++) {
            Transform childTransform = weatherArea.GetChild(i);
            TokenController token = childTransform.GetComponent<TokenController>();
            if (token != null) {
                weatherTokens.Add(token);
            }
        }
    }

    void GetWeatherDice() {
        foreach (TokenController token in weatherTokens) {
            if (token.tokenType == TokenType.RedWeatherDie || token.tokenType == TokenType.WhiteWeatherDie || token.tokenType == TokenType.OrangeWeatherDie) {
                weatherDice.Add(token);
            }
        }
    }

    void ClearWeatherArea() {
        for (int i = 0; i < weatherArea.childCount; i++) {
            Transform childTransform = weatherArea.GetChild(i);
            Destroy(childTransform.gameObject);
        }
        weatherTokens.Clear();
        weatherDice.Clear();
    }
}
