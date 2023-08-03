using System.Collections;
using System.Collections.Generic;

public static class CharacterFactory
{
    static Ability carpenterAbility0 = new Ability("Economical Construction", 2, 0, true, CardEffectFactory.CreateCardEffect("EconomicalConstructionEffect"));
    static Ability carpenterAbility1 = new Ability("Craftsmanship", 2, 0, false, CardEffectFactory.CreateCardEffect("CraftsmanshipEffect"));
    static Ability carpenterAbility2 = new Ability("A New Idea", 3, 0, true, CardEffectFactory.CreateCardEffect("ANewIdeaEffect"));
    static Ability carpenterAbility3 = new Ability("Handyman", 3, 0, true, CardEffectFactory.CreateCardEffect("HandymanEffect"));
    static List<Ability> carpenterAbilities = new List<Ability> { carpenterAbility0, carpenterAbility1, carpenterAbility2, carpenterAbility3 };

    static Ability cookAbility0 = new Ability("Grandmas Recipe", 2, 1, true, CardEffectFactory.CreateCardEffect("GrandmasRecipeEffect"));
    static Ability cookAbility1 = new Ability("Scrounger", 2, 0, false, CardEffectFactory.CreateCardEffect("ScroungerEffect"));
    static Ability cookAbility2 = new Ability("Stone Soup", 3, 0, true, CardEffectFactory.CreateCardEffect("StoneSoupEffect"));
    static Ability cookAbility3 = new Ability("Hooch", 3, 0, true, new List<string> { "Cancel", "Cancel rain", "Snow to rain" }, CardEffectFactory.CreateCardEffect("HoochEffect"));
    static List<Ability> cookAbilities = new List<Ability> { cookAbility0, cookAbility1, cookAbility2, cookAbility3 };

    static Ability explorerAbility0 = new Ability("Lucky", 2, 0, false, CardEffectFactory.CreateCardEffect("LuckyEffect"));
    static Ability explorerAbility1 = new Ability("Reconnaissance", 2, 0, true, CardEffectFactory.CreateCardEffect("ReconnaissanceEffect"));
    static Ability explorerAbility2 = new Ability("Motivational Speech", 3, 0, true, CardEffectFactory.CreateCardEffect("MotivationalSpeechEffect"));
    static Ability explorerAbility3 = new Ability("Scouting", 3, 0, true, CardEffectFactory.CreateCardEffect("ScoutingEffect"));
    static List<Ability> explorerAbilities = new List<Ability> { explorerAbility0, explorerAbility1, explorerAbility2, explorerAbility3 };

    static Ability soldierAbility0 = new Ability("Tracking", 2, 0, true, CardEffectFactory.CreateCardEffect("TrackingEffect"));
    static Ability soldierAbility1 = new Ability("Defensive Plan", 2, 0, true, new List<string> { "Cancel", "Increase palisade", "Increase weapon level" }, CardEffectFactory.CreateCardEffect("DefensivePlanEffect"));
    static Ability soldierAbility2 = new Ability("Frenzy", 3, 0, false, CardEffectFactory.CreateCardEffect("FrenzyEffect"));
    static Ability soldierAbility3 = new Ability("The Hunt", 4, 0, true, CardEffectFactory.CreateCardEffect("TheHuntEffect"));
    static List<Ability> soldierAbilities = new List<Ability> { soldierAbility0, soldierAbility1, soldierAbility2, soldierAbility3 };

    private static Dictionary<CharacterType, Character> characters = new Dictionary<CharacterType, Character> {
        { CharacterType.Carpenter, new Character(CharacterType.Carpenter, "Carpenter", 13, new List<int> { 8, 5, 3 }, Invention.Snare, carpenterAbilities) },
        { CharacterType.Cook, new Character(CharacterType.Cook, "Cook", 13, new List<int> { 9, 6, 4, 2 }, Invention.Fireplace, cookAbilities) },
        { CharacterType.Explorer, new Character(CharacterType.Explorer, "Explorer", 12, new List<int> { 6, 1 }, Invention.Shortcut, explorerAbilities) },
        { CharacterType.Soldier, new Character(CharacterType.Soldier, "Soldier", 12, new List<int> { 7, 3 }, Invention.Spear, soldierAbilities) }
    };

    public static Character CreateCharacter(CharacterType characterType)
    {
        return characters[characterType];
    }
}
