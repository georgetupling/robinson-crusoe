using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenPositioner
{
    const float initialTemperature = 5000f;
    const float coolingFactor = 0.95f;
    const float minimumTemperature = 0.001f;

    const float smallTokenRadius = 0.10f;
    const float largeTokenRadius = 0.25f;
    const float areaBoundaryX = 0.3f;
    const float areaBoundaryY = 0.175f;

    const float stepSize = 0.03f;

    static readonly List<TokenType> largeTokenTypes = new List<TokenType> { 
        TokenType.Discovery, TokenType.Storm, TokenType.RainCloud, 
        TokenType.SnowCloud, TokenType.OrangeWeatherDie, TokenType.RedWeatherDie, TokenType.OrangeWeatherDie };
    static readonly List<TokenType> flatTokenTypes = new List<TokenType> { TokenType.Discovery, TokenType.Storm, TokenType.RainCloud, TokenType.SnowCloud };

    public static void PositionTokens(Transform tokenArea) {
        // Generates parallel lists of token positions and token types
        List<GameObject> childObjects = new List<GameObject>();
        for (int i = 0; i < tokenArea.childCount; i++) {
            childObjects.Add(tokenArea.GetChild(i).gameObject);
        }
        List<Vector3> positions = new List<Vector3>();
        List<TokenType> tokenTypes = new List<TokenType>();
        List<int> componentIds = new List<int>();
        foreach (GameObject childObject in childObjects) {
            if (childObject.GetComponent<TokenController>() == null) {
                continue;
            }
            positions.Add(childObject.transform.localPosition);
            tokenTypes.Add(childObject.GetComponent<TokenController>().tokenType);
            componentIds.Add(childObject.GetComponent<TokenController>().ComponentId);
        }

        // Simulated annealing algorithm
        float temperature = initialTemperature;
        float energy = ComputeEnergy(positions, tokenTypes);
        while (energy > 0 && temperature > minimumTemperature) {
            List<Vector3> newPositions = MakeRandomAdjustment(positions, tokenTypes);
            float newEnergy = ComputeEnergy(newPositions, tokenTypes);
            if (newEnergy < energy) {
                positions = newPositions;
                energy = newEnergy;
            } else {
                if (Random.Range(0f, 1f) < Mathf.Exp((energy - newEnergy) / temperature)) {
                    positions = newPositions;
                    energy = newEnergy;
                }
            }
            temperature *= coolingFactor;
        }

        // Raises events to move the tokens to their new positions
        for (int i = 0; i < positions.Count; i++) {
            EventGenerator.Singleton.RaiseMoveComponentEvent(componentIds[i], positions[i]);
        }
    }

    // Helper methods

    static float ComputeEnergy(List<Vector3> positions, List<TokenType> tokenTypes) {
        float energy = 0;
        for (int i = 0; i < positions.Count; i++) {
            for (int j = 0; j < positions.Count; j++) {
                if (i == j) {
                    continue;
                }
                // Applies penalty for tokens outside the area boundaries
                if (Mathf.Abs(positions[i].x) > areaBoundaryX) {
                    energy += 2.5f * (Mathf.Abs(positions[i].x) - areaBoundaryX);
                }
                if (Mathf.Abs(positions[i].y) > areaBoundaryY) {
                    energy += 2.5f * (Mathf.Abs(positions[i].y) - areaBoundaryY);
                }
                // Sets the minimum distance between the two tokens and applies penalty if they're too close
                float minimumDistance = SetMinimumDistance(positions, tokenTypes, i , j);
                float distanceX = Mathf.Abs(positions[i].x - positions[j].x);
                float distanceY = Mathf.Abs(positions[i].y - positions[j].y);
                float euclideanDistance = Mathf.Sqrt(Mathf.Pow(distanceX, 2f) + Mathf.Pow(distanceY, 2f));
                if (euclideanDistance < minimumDistance) {
                    energy += minimumDistance - euclideanDistance;
                    continue;
                }
            }
        }
        return energy;
    }

    static float SetMinimumDistance(List<Vector3> positions, List<TokenType> tokenTypes, int i, int j) {
        float minimumDistance;
        if (largeTokenTypes.Contains(tokenTypes[i]) && largeTokenTypes.Contains(tokenTypes[j])) {
            minimumDistance = largeTokenRadius * 2;
        } else if (!largeTokenTypes.Contains(tokenTypes[i]) && !largeTokenTypes.Contains(tokenTypes[j])) {
            minimumDistance = smallTokenRadius * 2;
        } else {
            minimumDistance = smallTokenRadius + largeTokenRadius;
        }
        // Tokens can half overlap if they are both flat and only one is raised
        if (flatTokenTypes.Contains(tokenTypes[i]) && flatTokenTypes.Contains(tokenTypes[j]) && positions[i].z != positions[j].z) {
            minimumDistance *= 0.5f;
        }
        return minimumDistance;
    }

    static List<Vector3> MakeRandomAdjustment(List<Vector3> positions, List<TokenType> tokenTypes) {
        List<Vector3> newPositions = new List<Vector3>(positions);
        int randomIndex = Random.Range(0, newPositions.Count);
        float randomNumber = Random.Range(0f, 1f);
        int adjustmentFactor = Random.Range(0f, 1f) < 0.5f ? 1 : -1;
        Vector3 newPosition = newPositions[randomIndex];
        TokenType tokenType = tokenTypes[randomIndex];
        if (flatTokenTypes.Contains(tokenType)) {
            if (randomNumber < 0.1) { 
                if (newPosition.z == 0f || newPosition.z == -0.5f * ComponentDimensions.GetHeight(tokenType)) {
                    newPosition = new Vector3(newPosition.x, newPosition.y, -1.5f * ComponentDimensions.GetHeight(tokenType));
                } else if (newPosition.z == -1.5f * ComponentDimensions.GetHeight(tokenType)) {
                    newPosition = new Vector3(newPosition.x, newPosition.y, -ComponentDimensions.GetHeight(tokenType));
                } else {
                    newPosition = new Vector3(newPosition.x, newPosition.y, -0.5f * ComponentDimensions.GetHeight(tokenType));
                }
            } else if (randomNumber < 0.55) {
                newPosition = new Vector3(newPosition.x + stepSize * adjustmentFactor, newPosition.y, newPosition.z);
            } else {
                newPosition = new Vector3(newPosition.x, newPosition.y + stepSize * adjustmentFactor, newPosition.z);
            }
        } else {
            if (randomNumber < 0.5) {
                newPosition = new Vector3(newPosition.x + stepSize * adjustmentFactor, newPosition.y, newPosition.z);
            } else {
                newPosition = new Vector3(newPosition.x, newPosition.y + stepSize * adjustmentFactor, newPosition.z);
            }
        }
        newPositions[randomIndex] = newPosition;
        return newPositions;
    }

}
