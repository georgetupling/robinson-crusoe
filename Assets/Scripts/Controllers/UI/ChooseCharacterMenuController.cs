using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ChooseCharacterMenuController : MonoBehaviour
{
    [SerializeField] private Transform menuBackground;
    [SerializeField] private Button backButton;
    [SerializeField] private Button startButton;

    [SerializeField] private Transform nameArea;
    [SerializeField] private Transform characterArea;
    [SerializeField] private Transform genderArea;

    List<CharacterType> chosenCharacters = new List<CharacterType> { CharacterType.Random, CharacterType.Random, CharacterType.Random, CharacterType.Random };
    List<CharacterType> availableCharacters = new List<CharacterType> { CharacterType.Random, CharacterType.Carpenter, CharacterType.Cook, CharacterType.Explorer, CharacterType.Soldier };

    List<Gender> chosenGenders = new List<Gender> { Gender.Random, Gender.Random, Gender.Random, Gender.Random };
    List<Gender> availableGenders = new List<Gender> { Gender.Random, Gender.Male, Gender.Female };

    void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            menuBackground.gameObject.SetActive(true);
            gameObject.SetActive(false);
        });
        startButton.onClick.AddListener(() =>
        {
            for (int i = 0; i < GameSettings.PlayerCount; i++) {
                if (chosenCharacters[i] == CharacterType.Random) {
                    // Sets the chosen character to a non-Random character not already chosen
                    List<CharacterType> possibleCharacters = new List<CharacterType>();
                    foreach (CharacterType character in availableCharacters) {
                        if (character != CharacterType.Random && !chosenCharacters.Contains(character)) {
                            possibleCharacters.Add(character);
                        }
                    }
                    int randomIndex = Random.Range(0, possibleCharacters.Count);
                    int playerId = i;
                    chosenCharacters.Add(possibleCharacters[randomIndex]);
                    GameSettings.PlayerCharacters[playerId] = possibleCharacters[randomIndex];
                }

                if (chosenGenders[i] == Gender.Random) {
                    Gender randomGender = Random.Range(0f, 1f) < 0.5f ? Gender.Male : Gender.Female;
                    int playerId = i;
                    GameSettings.PlayerGenders[playerId] = randomGender;
                }
            }
            SceneManager.LoadScene("MainScene");
        });
    }

    public void Initialize()
    {
        InitializeNameFields();
        InitializeCharacterFields();
        InitializeGenderFields();
    }
    void InitializeNameFields()
    {
        for (int i = 0; i < nameArea.childCount; i++)
        {
            Transform childTransform = nameArea.GetChild(i);
            TMP_InputField nameField = childTransform.GetComponent<TMP_InputField>();
            if (nameField == null)
            {
                continue;
            }
            if (i < GameSettings.PlayerCount)
            {
                nameField.interactable = true;
                int playerId = i;
                GameSettings.PlayerNames[playerId] = "Player" + (playerId + 1);
                nameField.onValueChanged.AddListener((string value) =>
                {
                    GameSettings.PlayerNames[playerId] = value;
                });
            }
            else
            {
                nameField.interactable = false;
            }
        }
    }
    void InitializeCharacterFields() {
        for (int i = 0; i < characterArea.childCount; i++) {
            Transform childTransform = characterArea.GetChild(i);
            TMP_Text characterDisplay = childTransform.GetComponent<TMP_Text>();
            if (characterDisplay == null) {
                continue;
            }
            Button[] buttons = characterDisplay.GetComponentsInChildren<Button>();
            if (buttons.Length < 2 || buttons[0] == null || buttons[1] == null) {
                continue;
            }
            Button leftButton = buttons[0];
            Button rightButton = buttons[1];
            // Adds listeners to the left and right character select buttons
            if (i < GameSettings.PlayerCount)
            {
                leftButton.interactable = true;
                rightButton.interactable = true;
                characterDisplay.color = Color.white;
                
                int playerId = i;
                leftButton.onClick.AddListener(() => {
                    // Gets the previous character type, looping until it reaches a character type not already chosen or "random"
                    do
                    {
                        int currentIndex = availableCharacters.IndexOf(chosenCharacters[playerId]);
                        int newIndex = --currentIndex % availableCharacters.Count;
                        if (newIndex < 0) {
                            newIndex = availableCharacters.Count + newIndex;
                        }
                        chosenCharacters[playerId] = availableCharacters[newIndex];
                    }
                    while (chosenCharacters.Where((c, index) => index < GameSettings.PlayerCount && index != playerId)
                        .Any(c => c != CharacterType.Random && c == chosenCharacters[playerId]));
                    characterDisplay.text = chosenCharacters[playerId].ToString();
                    GameSettings.PlayerCharacters[playerId] = chosenCharacters[playerId];
                });
                rightButton.onClick.AddListener(() => {
                    // Gets the next character type, looping until it reaches a character type not already chosen or "random"
                    do
                    {
                        int currentIndex = availableCharacters.IndexOf(chosenCharacters[playerId]);
                        int newIndex = ++currentIndex % availableCharacters.Count;
                        if (newIndex < 0) {
                            newIndex = availableCharacters.Count + newIndex;
                        }
                        chosenCharacters[playerId] = availableCharacters[newIndex];
                    }
                    while (chosenCharacters.Where((c, index) => index < GameSettings.PlayerCount && index != playerId)
                        .Any(c => c != CharacterType.Random && c == chosenCharacters[playerId]));
                    characterDisplay.text = chosenCharacters[playerId].ToString();
                    GameSettings.PlayerCharacters[playerId] = chosenCharacters[playerId];
                });
            }
            else
            {
                leftButton.interactable = false;
                rightButton.interactable = false;
                characterDisplay.color = Color.grey;
            }
        }
    }

    void InitializeGenderFields() {
        for (int i = 0; i < genderArea.childCount; i++) {
            Transform childTransform = genderArea.GetChild(i);
            TMP_Text genderDisplay = childTransform.GetComponent<TMP_Text>();
            if (genderDisplay == null) {
                continue;
            }
            Button[] buttons = genderDisplay.GetComponentsInChildren<Button>();
            if (buttons.Length < 2 || buttons[0] == null || buttons[1] == null) {
                continue;
            }
            Button leftButton = buttons[0];
            Button rightButton = buttons[1];
            
            // Adds listeners to the left and right character select buttons
            if (i < GameSettings.PlayerCount)
            {
                // Re-enables the buttons if necessary
                leftButton.interactable = true;
                rightButton.interactable = true;
                genderDisplay.color = Color.white;
                
                int playerId = i;
                leftButton.onClick.AddListener(() => {
                    // Gets the previous gender
                    int currentIndex = availableGenders.IndexOf(chosenGenders[playerId]);
                    int newIndex = --currentIndex % availableGenders.Count;
                    if (newIndex < 0) {
                        newIndex = availableGenders.Count + newIndex;
                    }
                    chosenGenders[playerId] = availableGenders[newIndex];
                    genderDisplay.text = chosenGenders[playerId].ToString();
                    GameSettings.PlayerGenders[playerId] = chosenGenders[playerId];
                });
                rightButton.onClick.AddListener(() => {
                    // Gets the next gender
                    int currentIndex = availableGenders.IndexOf(chosenGenders[playerId]);
                    int newIndex = ++currentIndex % availableGenders.Count;
                    if (newIndex < 0) {
                        newIndex = availableGenders.Count + newIndex;
                    }
                    chosenGenders[playerId] = availableGenders[newIndex];
                    genderDisplay.text = chosenGenders[playerId].ToString();
                    GameSettings.PlayerGenders[playerId] = chosenGenders[playerId];
                });
            }
            else
            {
                leftButton.interactable = false;
                rightButton.interactable = false;
                genderDisplay.color = Color.grey;
            }
        }
    }
}
