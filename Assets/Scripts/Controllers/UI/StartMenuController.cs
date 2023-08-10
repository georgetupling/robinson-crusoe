using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenuController : MonoBehaviour
{
    // Play and exit buttons
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    // Fields for selecting player count
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private TMP_Text playerCountDisplay;
    private int playerCount = 1;

    // Fields for selecting scenario
    [SerializeField] private Button nextScenarioButton;
    [SerializeField] private Button prevScenarioButton;
    [SerializeField] private TMP_Text scenarioDisplay;
    private List<Scenario> scenarios = new List<Scenario>();
    private Scenario selectedScenario = Scenario.Random; 

    // Menus
    [SerializeField] private Transform startMenu;
    [SerializeField] private ChooseCharacterMenuController chooseCharacterMenu;


    void Awake()
    {
        foreach (Scenario value in Enum.GetValues(typeof(Scenario))) {
            scenarios.Add(value);
        }
        SetUpButtons();
    }

    void SetUpButtons()
    {
        playButton.onClick.AddListener(() =>
        {
            GameSettings.PlayerCount = playerCount;
            if (selectedScenario == Scenario.Random) {
                List<Scenario> possibleScenarios = new List<Scenario>(scenarios);
                possibleScenarios.Remove(Scenario.Random);
                int randomIndex = UnityEngine.Random.Range(0, possibleScenarios.Count);
                selectedScenario = possibleScenarios[randomIndex];
            }
            GameSettings.CurrentScenario = selectedScenario;
            startMenu.gameObject.SetActive(false);
            chooseCharacterMenu.Initialize();
            chooseCharacterMenu.gameObject.SetActive(true);
        });
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        plusButton.onClick.AddListener(() =>
        {
            playerCount = Mathf.Clamp(++playerCount, 1, 4);
            playerCountDisplay.text = playerCount.ToString();
        });
        minusButton.onClick.AddListener(() =>
        {
            playerCount = Mathf.Clamp(--playerCount, 1, 4);
            playerCountDisplay.text = playerCount.ToString();
        });
        nextScenarioButton.onClick.AddListener(() => {
            int currentIndex = scenarios.IndexOf(selectedScenario);
            int nextIndex = ++currentIndex % scenarios.Count;
            selectedScenario = scenarios[nextIndex];
            scenarioDisplay.text = scenarios[nextIndex].ToString();
        });
        prevScenarioButton.onClick.AddListener(() => {
            int currentIndex = scenarios.IndexOf(selectedScenario);
            int nextIndex = --currentIndex % scenarios.Count;
            if (currentIndex < 0) {
                nextIndex = scenarios.Count + nextIndex;
            }
            selectedScenario = scenarios[nextIndex];
            scenarioDisplay.text = scenarios[nextIndex].ToString();
        });
    }
}
