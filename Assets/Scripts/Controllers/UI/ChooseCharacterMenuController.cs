using System.Collections;
using System.Collections.Generic;
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
    void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            menuBackground.gameObject.SetActive(true);
            gameObject.SetActive(false);
        });
        startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainScene");
        });
    }

    public void Initialize()
    {
        InitializeNameFields();
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
                int playerId = i;
                nameField.onValueChanged.AddListener((string value) =>
                {
                    Debug.Log(value);
                    GameSettings.PlayerNames[playerId] = value;
                });
            }
            else
            {
                nameField.interactable = false;
            }
        }
    }
}
