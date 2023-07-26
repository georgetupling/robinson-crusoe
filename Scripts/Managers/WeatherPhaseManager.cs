using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherPhaseManager : MonoBehaviour
{
    static WeatherPhaseManager singleton;
    
    [SerializeField] Transform weatherArea;
    [SerializeField] Transform popupArea;
    List<TokenController> weatherTokens = new List<TokenController>();
    List<DieType> weatherDice = new List<DieType>();

    int roofLevel;
    bool isWaitingOnRoofLevel;

    int numberOfRainClouds = 0;
    int numberOfSnowClouds = 0;

    // numberOfClouds[0] is the number of rain clouds, numberOfClouds[1] is the number of snow clouds

    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            return;
        }
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToGetRoofLevelResponseEvent(OnGetRoofLevelResponseEvent);
        EventGenerator.Singleton.AddListenerToDieRolledEvent(OnDieRolledEvent);
    }

    void OnPhaseStartEvent(Phase phaseStarted) {
        if (phaseStarted != Phase.Weather) {
            return;
        }
        StartCoroutine(ApplyWeatherPhase());
    }

    void OnDieRolledEvent(DieType dieType, int faceRolled) {
        if (dieType == DieType.WhiteWeather) {
            if (faceRolled < 2) {
                numberOfSnowClouds += 1;
            } else if (faceRolled < 4) {
                numberOfSnowClouds += 2;
            } else {
                numberOfRainClouds += 2;
            }
        } else if (dieType == DieType.RedWeather) {
            if (faceRolled < 1) {
                EventGenerator.Singleton.RaiseLoseFoodEvent(1);
            } else if (faceRolled < 3) {
                EventGenerator.Singleton.RaiseLosePalisadeEvent(1);
            } else if (faceRolled < 4) {
                // TODO: combat against strength 3 beast
            }
        } else if (dieType == DieType.OrangeWeather) {
            if (faceRolled < 1) {
                numberOfSnowClouds += 1;
            } else if (faceRolled < 4) {
                numberOfRainClouds += 1;
            } else {
                numberOfRainClouds += 2;
            }
        }
    }

    IEnumerator ApplyWeatherPhase() {
        isWaitingOnRoofLevel = true;
        EventGenerator.Singleton.RaiseGetRoofLevelEvent();
        while (isWaitingOnRoofLevel) {
            yield return null;
        }
        GetWeatherTokens();
        GetWeatherDice();

        // Rolls the weather dice and waits for the user to close the popup
        if (weatherDice.Count > 0) {
            EventGenerator.Singleton.RaiseSpawnDicePopupEvent(weatherDice);
        }
        while (popupArea.childCount > 0) {
            yield return null;
        }
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
            if (token.tokenType == TokenType.RedWeatherDie) {
                weatherDice.Add(DieType.RedWeather);
            } else if (token.tokenType == TokenType.WhiteWeatherDie) {
                weatherDice.Add(DieType.WhiteWeather);
            } else if (token.tokenType == TokenType.OrangeWeatherDie) {
                weatherDice.Add(DieType.OrangeWeather);
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
        numberOfRainClouds = 0;
        numberOfSnowClouds = 0;
    }
}
