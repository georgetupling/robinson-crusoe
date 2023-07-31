using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DicePopupController : MonoBehaviour
{
    [SerializeField] RectTransform background;
    [SerializeField] TMP_Text instruction;
    [SerializeField] RectTransform diceArea;
    [SerializeField] Image imagePrefab;
    [SerializeField] Button okayButton;
    [SerializeField] Button rerollButton;
    
    List<Die> dice = new List<Die>();
    List<Image> images = new List<Image>();
    List<int> facesRolled = new List<int>();
    int playerId;

    const int iterations = 10;
    const float waitPerIteration = 0.15f;

    void Awake() {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        SetUpOkayButton();
        SetUpRerollButton();
    }

    void SetUpOkayButton() {
        okayButton.onClick.AddListener(() => {
            for (int i = 0; i < dice.Count; i++) {
                EventGenerator.Singleton.RaiseDieRolledEvent(dice[i].type, facesRolled[i]);
            }
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }

    void SetUpRerollButton() {
        rerollButton.onClick.AddListener(() => {
            instruction.gameObject.SetActive(true);
            okayButton.interactable = false;
            rerollButton.interactable = false;
            EventGenerator.Singleton.RaiseLoseDeterminationEvent(playerId, 2);
            // Pressing reroll adds listeners to and enables the die image buttons
            for (int i = 0; i < images.Count; i++) {
                int index = i;
                Button imageButton = images[i].GetComponent<Button>();
                imageButton.onClick.AddListener(() => {
                    DisableDieButtons();
                    StartCoroutine(RerollDie(index));
                });
                imageButton.enabled = true;
            }
        });
    }

    public void Initialize(List<DieType> dieTypes, int playerId, bool hasRerollAvailable) {
        this.playerId = playerId;
        // Converts the die types into dice using the DiceFactory
        foreach (DieType dieType in dieTypes) {
            Die die = DiceFactory.Singleton.GetDie(dieType);
            if (die == null) {
                continue;
            }
            dice.Add(die);
        }
        // Spawns the Images which will display the dice sprites
        for (int i = 0; i < dice.Count; i++) {
            Image image = Instantiate(imagePrefab, diceArea, false);

            float x, y;
            switch (dice.Count) {
                case 1:
                    x = diceArea.rect.width / 2f;
                    y = diceArea.rect.height / 2f;
                    break;
                case 2:
                    x = diceArea.rect.width * (i == 0 ? 0.33f : 0.67f);
                    y = diceArea.rect.height / 2f;
                    break;
                case 3:
                    x = diceArea.rect.width * (i == 0 ? 0.5f : i == 1 ? 0.33f : 0.67f);
                    y = diceArea.rect.height * (i == 0 ? 0.33f : 0.67f);
                    break;
                case 4:
                    x = diceArea.rect.width * (i < 2 ? 0.33f : 0.67f);
                    y = diceArea.rect.height * (i % 2 == 0 ? 0.33f : 0.67f);
                    break;
                default:
                Debug.LogError($"Unable to position dice.");
                    // Defaults to positioning the dice randomly -- will create overlaps!
                    x = Random.Range(diceArea.rect.width * 0.2f, diceArea.rect.width * 0.8f);
                    y = Random.Range(diceArea.rect.height * 0.2f, diceArea.rect.height * 0.8f);
                    break;
            }

            image.rectTransform.anchoredPosition = new Vector2(x, y);
            images.Add(image);
        }

        if (hasRerollAvailable) {
            rerollButton.gameObject.SetActive(true);
            float spaceBetweenButtons = 15f;
            background.sizeDelta = new Vector2(background.rect.width, background.rect.height + rerollButton.GetComponent<RectTransform>().rect.height + spaceBetweenButtons);
        }
        
        StartCoroutine(RollDice());
    }

    IEnumerator RollDice() {
        for (int i = 0; i < iterations; i++) {
            for (int j = 0; j < dice.Count; j++) {
                int randomDiceSide = Random.Range(0, 6);
                images[j].sprite = dice[j].GetFaceSprite(randomDiceSide);
                if (facesRolled.Count < j + 1) {
                    facesRolled.Add(randomDiceSide);
                } else {
                    facesRolled[j] = randomDiceSide;
                }
            }
            yield return new WaitForSeconds(waitPerIteration);
        }
        okayButton.interactable = true;
        rerollButton.interactable = true;
    }

    // Helper methods for rerolling dice

    void DisableDieButtons() {
        foreach (Image image in images) {
            Button imageButton = image.GetComponent<Button>();
            imageButton.enabled = false;
        }
    }

    IEnumerator RerollDie(int imageIndex) {
        for (int i = 0; i < iterations; i++) {
                int randomDiceSide = Random.Range(0, 6);
                images[imageIndex].sprite = dice[imageIndex].GetFaceSprite(randomDiceSide);
                facesRolled[imageIndex] = randomDiceSide;
            yield return new WaitForSeconds(waitPerIteration);
        }
        okayButton.interactable = true;
    }



}
