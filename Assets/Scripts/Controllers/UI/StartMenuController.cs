using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private TMP_Text playerCountDisplay;
    [SerializeField] private Transform startMenu;
    [SerializeField] private ChooseCharacterMenuController chooseCharacterMenu;

    private int playerCount = 1;

    void Awake()
    {
        SetUpButtons();
    }

    void SetUpButtons()
    {
        playButton.onClick.AddListener(() =>
        {
            GameSettings.PlayerCount = playerCount;
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
            playerCountDisplay.text = playerCount + " Player";
        });
        minusButton.onClick.AddListener(() =>
        {
            playerCount = Mathf.Clamp(--playerCount, 1, 4);
            playerCountDisplay.text = playerCount + " Player";
        });
    }
}
