using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentDimensions
{
    static Dictionary<TokenType, float> tokenHeights = new Dictionary<TokenType, float> {
        { TokenType.BlackMarker, 0.025f },
        { TokenType.Storm, 0.01f },
        { TokenType.SnowCloud, 0.01f },
        { TokenType.RainCloud, 0.01f },
        { TokenType.Discovery, 0.01f },
        { TokenType.OrangeWeatherDie, 0.15f },
        { TokenType.RedWeatherDie, 0.15f },
        { TokenType.WhiteWeatherDie, 0.15f },
        { TokenType.Camp, 0.01f },
        { TokenType.Shortcut, 0.01f },
        { TokenType.Wood, 0.03f }
    };

    public static float GetHeight(TokenType tokenType)
    {
        if (!tokenHeights.ContainsKey(tokenType))
        {
            Debug.Log($"{tokenType} token height not set.");
            return 0f;
        }
        return tokenHeights[tokenType];
    }
}
