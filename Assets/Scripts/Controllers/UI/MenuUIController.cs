using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    void Awake()
    {
        SetUpButtons();
    }

    void SetUpButtons()
    {
        restartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainScene");
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("StartMenu");
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
