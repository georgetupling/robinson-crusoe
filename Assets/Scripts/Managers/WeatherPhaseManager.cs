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

    int cookDetermination;
    bool isWaitingOnCookDetermination;

    int numberOfRainClouds = 0;
    int numberOfSnowClouds = 0;
    int foodLost = 0;
    int palisadeLost = 0;

    int cancelledRainClouds = 0;
    int snowCloudsConvertedToRain = 0;

    // Inventions
    bool furnaceIsBuilt;


    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            return;
        }
        EventGenerator.Singleton.AddListenerToPhaseStartEvent(OnPhaseStartEvent);
        EventGenerator.Singleton.AddListenerToTurnStartEvent(OnTurnStartEvent);
        EventGenerator.Singleton.AddListenerToGetRoofLevelResponseEvent(OnGetRoofLevelResponseEvent);
        EventGenerator.Singleton.AddListenerToDieRolledEvent(OnDieRolledEvent);
        EventGenerator.Singleton.AddListenerToGetDeterminationResponseEvent(OnGetDeterminationResponseEvent);
        EventGenerator.Singleton.AddListenerToCancelRainCloudEvent(OnCancelRainCloudEvent);
        EventGenerator.Singleton.AddListenerToConvertSnowToRainEvent(OnConvertSnowToRainEvent);
        EventGenerator.Singleton.AddListenerToUpdateBuiltInventionsEvent(OnUpdateBuiltInventionsEvent);
    }

    // Listeners

    void OnPhaseStartEvent(Phase phaseStarted)
    {
        if (phaseStarted != Phase.Weather)
        {
            return;
        }
        StartCoroutine(ApplyWeatherPhase());
    }

    void OnTurnStartEvent(int turnStarted)
    {
        SpawnWeatherForTurn(turnStarted);
    }

    void OnDieRolledEvent(DieType dieType, int faceRolled)
    {
        if (dieType == DieType.WhiteWeather)
        {
            if (faceRolled < 2)
            {
                numberOfSnowClouds += 1;
            }
            else if (faceRolled < 4)
            {
                numberOfSnowClouds += 2;
            }
            else
            {
                numberOfRainClouds += 2;
            }
        }
        else if (dieType == DieType.RedWeather)
        {
            if (faceRolled < 1)
            {
                foodLost++;
            }
            else if (faceRolled < 3)
            {
                palisadeLost++;
            }
            else if (faceRolled < 4)
            {
                // TODO: combat against strength 3 beast
            }
        }
        else if (dieType == DieType.OrangeWeather)
        {
            if (faceRolled < 1)
            {
                numberOfSnowClouds += 1;
            }
            else if (faceRolled < 4)
            {
                numberOfRainClouds += 1;
            }
            else
            {
                numberOfRainClouds += 2;
            }
        }
    }

    void OnGetDeterminationResponseEvent(int playerId, int determination)
    {
        if (isWaitingOnCookDetermination)
        {
            cookDetermination = determination;
            isWaitingOnCookDetermination = false;
        }
    }

    void OnGetRoofLevelResponseEvent(int roofLevel)
    {
        if (isWaitingOnRoofLevel)
        {
            this.roofLevel = roofLevel;
            isWaitingOnRoofLevel = false;
        }
    }

    void OnCancelRainCloudEvent()
    {
        cancelledRainClouds++;
    }

    void OnConvertSnowToRainEvent()
    {
        snowCloudsConvertedToRain++;
    }
    void OnUpdateBuiltInventionsEvent(Invention invention, bool isBuilt)
    {
        if (invention == Invention.Furnace)
        {
            furnaceIsBuilt = isBuilt;
        }
    }

    IEnumerator ApplyWeatherPhase()
    {
        isWaitingOnRoofLevel = true;
        EventGenerator.Singleton.RaiseGetRoofLevelEvent();
        while (isWaitingOnRoofLevel)
        {
            yield return null;
        }
        GetWeatherTokens();
        GetWeatherDice();

        // Rolls the weather dice and waits for the user to close the popup
        if (weatherDice.Count > 0)
        {
            EventGenerator.Singleton.RaiseSpawnDicePopupEvent(weatherDice);
        }
        while (popupArea.childCount > 0)
        {
            yield return null;
        }
        // Counts up tokens
        foreach (TokenController token in weatherTokens)
        {
            if (token.tokenType == TokenType.Storm)
            {
                EventGenerator.Singleton.RaiseLosePalisadeEvent(1);
            }
            else if (token.tokenType == TokenType.RainCloud)
            {
                numberOfRainClouds += 1;
            }
            else if (token.tokenType == TokenType.SnowCloud)
            {
                numberOfSnowClouds += 1;
            }
        }
        // Reduces snow by 1 if the furnace is built
        if (furnaceIsBuilt && numberOfSnowClouds > 0)
        {
            numberOfSnowClouds--;
        }
        // If there are one or more clouds, checks whether there is a cook with enough determination to use Hooch
        if (numberOfRainClouds > 0 || numberOfSnowClouds > 0)
        {
            for (int i = 0; i < GameSettings.PlayerCount; i++)
            {
                while (popupArea.childCount > 0)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(0.25f);
                if (GameSettings.PlayerCharacters[i] == CharacterType.Cook)
                {
                    isWaitingOnCookDetermination = true;
                    EventGenerator.Singleton.RaiseGetDeterminationEvent(i);
                    while (isWaitingOnCookDetermination)
                    {
                        yield return null;
                    }
                    int determinationRequired = 3;
                    if (cookDetermination >= determinationRequired)
                    {
                        Character cook = CharacterFactory.CreateCharacter(CharacterType.Cook);
                        Ability hooch = cook.abilities[3];
                        EventGenerator.Singleton.RaiseSpawnAbilityPopupEvent(i, hooch);
                    }
                }
            }
        }
        while (popupArea.childCount > 0)
        {
            yield return null;
        }

        // Applies any "cancel rain" or "convert snow to rain" effects
        numberOfRainClouds -= cancelledRainClouds;
        numberOfSnowClouds -= snowCloudsConvertedToRain;
        numberOfRainClouds += snowCloudsConvertedToRain;
        if (numberOfRainClouds < 0)
        {
            numberOfRainClouds = 0;
        }
        if (numberOfSnowClouds < 0)
        {
            numberOfSnowClouds = 0;
        }

        // Applies the effects
        EventGenerator.Singleton.RaiseLoseWoodEvent(numberOfSnowClouds);
        numberOfRainClouds += numberOfSnowClouds;
        numberOfRainClouds -= roofLevel;
        if (numberOfRainClouds > 0)
        {
            EventGenerator.Singleton.RaiseLoseFoodEvent(numberOfRainClouds);
            EventGenerator.Singleton.RaiseLoseWoodEvent(numberOfRainClouds);
        }
        EventGenerator.Singleton.RaiseLoseFoodEvent(foodLost);
        EventGenerator.Singleton.RaiseLosePalisadeEvent(palisadeLost);
        ClearWeatherArea();
        yield return new WaitForSeconds(1.5f);
        EventGenerator.Singleton.RaiseEndPhaseEvent(Phase.Weather);
    }

    // Helper methods

    void GetWeatherTokens()
    {
        for (int i = 0; i < weatherArea.childCount; i++)
        {
            Transform childTransform = weatherArea.GetChild(i);
            TokenController token = childTransform.GetComponent<TokenController>();
            if (token != null)
            {
                weatherTokens.Add(token);
            }
        }
    }

    void GetWeatherDice()
    {
        foreach (TokenController token in weatherTokens)
        {
            if (token.tokenType == TokenType.RedWeatherDie)
            {
                weatherDice.Add(DieType.RedWeather);
            }
            else if (token.tokenType == TokenType.WhiteWeatherDie)
            {
                weatherDice.Add(DieType.WhiteWeather);
            }
            else if (token.tokenType == TokenType.OrangeWeatherDie)
            {
                weatherDice.Add(DieType.OrangeWeather);
            }
        }
    }

    void ClearWeatherArea()
    {
        for (int i = 0; i < weatherArea.childCount; i++)
        {
            Transform childTransform = weatherArea.GetChild(i);
            Destroy(childTransform.gameObject);
        }
        weatherTokens.Clear();
        weatherDice.Clear();
        numberOfRainClouds = 0;
        numberOfSnowClouds = 0;
        foodLost = 0;
        palisadeLost = 0;
        cancelledRainClouds = 0;
        snowCloudsConvertedToRain = 0;
    }

    void SpawnWeatherForTurn(int turnStarted)
    {
        // This method spawns the weather for turn
        switch (GameSettings.CurrentScenario)
        {
            case Scenario.Castaways:
                if (turnStarted < 3)
                {
                    return;
                }
                else if (turnStarted < 6)
                {
                    EventGenerator.Singleton.RaiseSpawnTokenInWeatherAreaEvent(TokenType.OrangeWeatherDie);
                }
                else
                {
                    EventGenerator.Singleton.RaiseSpawnTokenInWeatherAreaEvent(TokenType.OrangeWeatherDie);
                    EventGenerator.Singleton.RaiseSpawnTokenInWeatherAreaEvent(TokenType.RedWeatherDie);
                    EventGenerator.Singleton.RaiseSpawnTokenInWeatherAreaEvent(TokenType.WhiteWeatherDie);
                }
                break;
            default:
                Debug.LogError($"Weather for {GameSettings.CurrentScenario} scenario not set.");
                break;
        }
    }
}
