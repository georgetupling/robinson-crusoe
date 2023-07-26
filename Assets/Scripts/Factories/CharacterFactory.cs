using System.Collections;
using System.Collections.Generic;

public static class CharacterFactory 
{
    private static Dictionary<CharacterType, Character> characters = new Dictionary<CharacterType, Character> {
        { CharacterType.Carpenter, new Character(CharacterType.Carpenter, "Carpenter", 13, new List<int> { 8, 5, 3 }) },
        { CharacterType.Cook, new Character(CharacterType.Cook, "Cook", 13, new List<int> { 9, 6, 4, 2 }) },
        { CharacterType.Explorer, new Character(CharacterType.Explorer, "Explorer", 12, new List<int> { 6, 1 }) },
        { CharacterType.Soldier, new Character(CharacterType.Soldier, "Soldier", 12, new List<int> { 7, 3 }) }
    };

    public static Character CreateCharacter(CharacterType characterType) {
        return characters[characterType];
    }
}
