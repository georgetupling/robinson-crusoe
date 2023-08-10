using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameEndScreenController : MonoBehaviour
{
    [SerializeField] private CanvasGroup background;
    [SerializeField] private TMP_Text message;
    [SerializeField] private TMP_Text submessage;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    const float fadeDuration = 1f;

    void Awake() {
        EventGenerator.Singleton.AddListenerToGameEndEvent(OnGameEndEvent);
        SetUpButtons();
    }
    void SetUpButtons() {
        restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainScene");
        });
        mainMenuButton.onClick.AddListener(() => {
            SceneManager.LoadScene("StartMenu");
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }
    void OnGameEndEvent(bool playersWin) {
        if (playersWin) {
            message.text = "Congratulations!";
            submessage.text = "You have braved the horrors of the island and emerged triumphant.";
        } else {
            message.text = "Defeat";
            submessage.text = "As the sun sets on your adventure, remember, every end is a new beginning.";
        }
        StartCoroutine(FadeInEffect());
    }
    private IEnumerator FadeInEffect() {
        background.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration) {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            background.alpha = alpha;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        background.alpha = 1f;
        message.gameObject.SetActive(true);
        submessage.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }
}
