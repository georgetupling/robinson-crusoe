using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NightPhasePopupController : MonoBehaviour
{
    [SerializeField] Button confirmButton;
    [SerializeField] Image background;
    [SerializeField] TMP_Text instruction;

    [SerializeField] Toggle player0Toggle;
    [SerializeField] Toggle player1Toggle;
    [SerializeField] Toggle player2Toggle;
    [SerializeField] Toggle player3Toggle;
    List<Toggle> toggles;

    [SerializeField] TMP_Text player0Label;
    [SerializeField] TMP_Text player1Label;
    [SerializeField] TMP_Text player2Label;
    [SerializeField] TMP_Text player3Label;
    List<TMP_Text> labels;

    public List<int> playersEating = new List<int>();

    void Awake()
    {
        toggles = new List<Toggle> { player0Toggle, player1Toggle, player2Toggle, player3Toggle };
        labels = new List<TMP_Text> { player0Label, player1Label, player2Label, player3Label };
        SetUpButton();
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
    }

    void SetUpButton()
    {
        confirmButton.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaisePlayersEatingEvent(playersEating);
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }

    void SetUpToggles(int totalFoodAvailable)
    {
        for (int i = 0; i < GameSettings.PlayerCount; i++)
        {
            int playerId = i;
            toggles[i].onValueChanged.AddListener((bool isChecked) =>
            {
                if (isChecked)
                {
                    playersEating.Add(playerId);
                }
                else
                {
                    playersEating.Remove(playerId);
                }
                if (playersEating.Count == totalFoodAvailable)
                {
                    confirmButton.interactable = true;
                }
                else
                {
                    confirmButton.interactable = false;
                }
            });
            labels[i].text = GameSettings.PlayerNames[i];
        }
    }

    public void Initialize(int totalFoodAvailable)
    {
        if (totalFoodAvailable >= GameSettings.PlayerCount)
        {
            Debug.LogError("Enough food is available to feed everyone.");
            Destroy(gameObject);
            return;
        }
        if (totalFoodAvailable < 0)
        {
            Debug.LogError("Total food available can't be less than 0.");
            Destroy(gameObject);
            return;
        }
        // Deactivates unneeded toggles
        int maxPlayerCount = 4;
        for (int i = 0; i < maxPlayerCount; i++)
        {
            toggles[i].gameObject.SetActive(GameSettings.PlayerCount > i);
        }
        // Resizes the popup if there are fewer than four toggles to choose from
        float width = background.rectTransform.sizeDelta.x;
        float height = background.rectTransform.sizeDelta.y - (35f * (maxPlayerCount - GameSettings.PlayerCount));
        background.rectTransform.sizeDelta = new Vector2(width, height);
        // Modifies the instruction text
        string instructionText = "Who is eating tonight?\n(Choose " + totalFoodAvailable + ")";
        instruction.text = instructionText;
        // Adds listeners to the toggle
        SetUpToggles(totalFoodAvailable);
    }
}
