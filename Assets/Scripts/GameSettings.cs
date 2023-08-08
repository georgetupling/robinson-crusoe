using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public static int PlayerCount = 2;
    public static Scenario CurrentScenario = Scenario.Castaways;
    public static float AnimationDuration = 1f;

    public static Dictionary<int, string> PlayerNames = new Dictionary<int, string> {
        { 0, "Bob" },
        { 1, "Alice" },
        { 2, "Daniel" },
        { 3, "Rose" }
    };
    public static Dictionary<int, CharacterType> PlayerCharacters = new Dictionary<int, CharacterType> {
        { 0, CharacterType.Soldier },
        { 1, CharacterType.Carpenter },
        { 2, CharacterType.Explorer },
        { 3, CharacterType.Cook }
    };
    public static Dictionary<int, Gender> PlayerGenders = new Dictionary<int, Gender> {
        { 0, Gender.Male },
        { 1, Gender.Female },
        { 2, Gender.Male },
        { 3, Gender.Female }
    };
}
