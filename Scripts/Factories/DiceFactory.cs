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
    }

    void LoadBuildDice() {
        Die buildSuccessDie = new Die(
            DieType.BuildSuccess,
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildSuccessDieDetermination"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildSuccessDieDetermination"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildSuccessDieSuccess"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildSuccessDieSuccess")
        );
        dice[DieType.BuildSuccess] = buildSuccessDie;

        Die buildAdventureDie = new Die(
            DieType.BuildAdventure,
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildAdventureDieAdventure"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildDieBlank"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildDieBlank"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildDieBlank")
        );
        dice[DieType.BuildAdventure] = buildAdventureDie;

        Die buildDamageDie = new Die(
            DieType.BuildDamage,
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildDamageDieDamage"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildDieBlank"),
            Resources.Load<Sprite>("Sprites/DiceFaces/BuildDieBlank")
        );
        dice[DieType.BuildDamage] = buildDamageDie;
    }

    public Die GetDie(DieType dieType) {
        if (!dice.ContainsKey(dieType) || dice[dieType] == null) {
            Debug.LogError($"Failed to load {dieType} die.");
            return null;
        }
        return dice[dieType];
    }
}
