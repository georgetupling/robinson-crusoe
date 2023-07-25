using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceFactory : MonoBehaviour
{
    public static DiceFactory Singleton;

    Dictionary<DieType, Die> dice = new Dictionary<DieType, Die>();

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
        } else {
            Debug.LogError("Scene contains duplicate DiceFactory.");
            return;
        }
        LoadBuildDice();
        LoadGatherDice();
        LoadExploreDice();
        LoadWeatherDice();
    }

    void LoadBuildDice() {
        Die buildSuccessDie = new Die(
            DieType.BuildSuccess,
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildSuccessDieDetermination"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildSuccessDieDetermination"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildSuccessDieSuccess")
        );
        dice[DieType.BuildSuccess] = buildSuccessDie;

        Die buildAdventureDie = new Die(
            DieType.BuildAdventure,
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildDieBlank")
        );
        dice[DieType.BuildAdventure] = buildAdventureDie;

        Die buildDamageDie = new Die(
            DieType.BuildDamage,
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/BuildDieBlank")
        );
        dice[DieType.BuildDamage] = buildDamageDie;
    }

    void LoadGatherDice() {
        Die gatherSuccessDie = new Die(
            DieType.GatherSuccess,
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherSuccessDieDetermination"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherSuccessDieSuccess")
        );
        dice[DieType.GatherSuccess] = gatherSuccessDie;

        Die gatherAdventureDie = new Die(
            DieType.GatherAdventure,
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherDieBlank")
        );
        dice[DieType.GatherAdventure] = gatherAdventureDie;

        Die gatherDamageDie = new Die(
            DieType.GatherDamage,
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/GatherDieBlank")
        );
        dice[DieType.GatherDamage] = gatherDamageDie;
    }

    void LoadExploreDice() {
        Die exploreSuccessDie = new Die(
            DieType.ExploreSuccess,
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreSuccessDieDetermination"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreSuccessDieSuccess")
        );
        dice[DieType.ExploreSuccess] = exploreSuccessDie;

        Die exploreAdventureDie = new Die(
            DieType.ExploreAdventure,
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreDieBlank")
        );
        dice[DieType.ExploreAdventure] = exploreAdventureDie;

        Die exploreDamageDie = new Die(
            DieType.ExploreDamage,
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/ExploreDieBlank")
        );
        dice[DieType.ExploreDamage] = exploreDamageDie;
    }

    void LoadWeatherDice() {
        Die whiteWeatherDie = new Die(
            DieType.WhiteWeather,
            Resources.Load<Sprite>("Sprites/Dice Faces/WhiteWeatherDieSnow"),
            Resources.Load<Sprite>("Sprites/Dice Faces/WhiteWeatherDieSnow"),
            Resources.Load<Sprite>("Sprites/Dice Faces/WhiteWeatherDieTwoSnow"),
            Resources.Load<Sprite>("Sprites/Dice Faces/WhiteWeatherDieTwoSnow"),
            Resources.Load<Sprite>("Sprites/Dice Faces/WhiteWeatherDieTwoRain"),
            Resources.Load<Sprite>("Sprites/Dice Faces/WhiteWeatherDieTwoRain")
        );
        dice[DieType.WhiteWeather] = whiteWeatherDie;

        Die orangeWeatherDie = new Die(
            DieType.OrangeWeather,
            Resources.Load<Sprite>("Sprites/Dice Faces/OrangeWeatherDieSnow"),
            Resources.Load<Sprite>("Sprites/Dice Faces/OrangeWeatherDieRain"),
            Resources.Load<Sprite>("Sprites/Dice Faces/OrangeWeatherDieRain"),
            Resources.Load<Sprite>("Sprites/Dice Faces/OrangeWeatherDieRain"),
            Resources.Load<Sprite>("Sprites/Dice Faces/OrangeWeatherDieTwoRain"),
            Resources.Load<Sprite>("Sprites/Dice Faces/OrangeWeatherDieTwoRain")
        );
        dice[DieType.OrangeWeather] = orangeWeatherDie;

        Die redWeatherDie = new Die(
            DieType.RedWeather,
            Resources.Load<Sprite>("Sprites/Dice Faces/RedWeatherDieFood"),
            Resources.Load<Sprite>("Sprites/Dice Faces/RedWeatherDiePalisade"),
            Resources.Load<Sprite>("Sprites/Dice Faces/RedWeatherDiePalisade"),
            Resources.Load<Sprite>("Sprites/Dice Faces/RedWeatherDieCombat"),
            Resources.Load<Sprite>("Sprites/Dice Faces/RedWeatherDieBlank"),
            Resources.Load<Sprite>("Sprites/Dice Faces/RedWeatherDieBlank")
        );
        dice[DieType.RedWeather] = redWeatherDie;
    }

    public Die GetDie(DieType dieType) {
        if (!dice.ContainsKey(dieType) || dice[dieType] == null) {
            Debug.LogError($"Failed to load {dieType} die.");
            return null;
        }
        return dice[dieType];
    }
}
