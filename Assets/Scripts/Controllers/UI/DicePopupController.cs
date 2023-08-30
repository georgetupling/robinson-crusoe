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

    // For resolving forced rerolls from reroll tokens
    bool forcedRerollOnSuccessfulAction;
    TokenController rerollToken;

    const int iterations = 10;
    const float waitPerIteration = 0.15f;

    void Awake()
    {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        SetUpOkayButton();
        SetUpRerollButton();
    }

    void SetUpOkayButton()
    {
        okayButton.onClick.AddListener(() =>
        {
            for (int i = 0; i < dice.Count; i++)
            {
                EventGenerator.Singleton.RaiseDieRolledEvent(dice[i].type, facesRolled[i]);
            }
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }

    void SetUpRerollButton()
    {
        rerollButton.onClick.AddListener(() =>
        {
            instruction.gameObject.SetActive(true);
            okayButton.interactable = false;
            rerollButton.interactable = false;
            EventGenerator.Singleton.RaiseLoseDeterminationEvent(playerId, 2);
            // Pressing reroll adds listeners to and enables the die image buttons
            for (int i = 0; i < images.Count; i++)
            {
                int index = i;
                Button imageButton = images[i].GetComponent<Button>();
                imageButton.onClick.AddListener(() =>
                {
                    DisableDieButtons();
                    StartCoroutine(RerollDie(index));
                });
                imageButton.enabled = true;
            }
        });
    }

    public void Initialize(List<DieType> dieTypes, int playerId, bool hasRerollAvailable)
    {
        this.playerId = playerId;
        // Converts the die types into dice using the DiceFactory
        foreach (DieType dieType in dieTypes)
        {
            Die die = DiceFactory.Singleton.GetDie(dieType);
            if (die == null)
            {
                continue;
            }
            dice.Add(die);
        }
        // Spawns the Images which will display the dice sprites
        for (int i = 0; i < dice.Count; i++)
        {
            Image image = Instantiate(imagePrefab, diceArea, false);

            float x, y;
            switch (dice.Count)
            {
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

        if (hasRerollAvailable)
        {
            rerollButton.gameObject.SetActive(true);
            float spaceBetweenButtons = 15f;
            background.sizeDelta = new Vector2(background.rect.width, background.rect.height + rerollButton.GetComponent<RectTransform>().rect.height + spaceBetweenButtons);
        }

        forcedRerollOnSuccessfulAction = CheckForForcedReroll(dieTypes);
        StartCoroutine(RollDice());
    }

    IEnumerator RollDice()
    {
        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < dice.Count; j++)
            {
                int randomDiceSide = Random.Range(0, 6);
                images[j].sprite = dice[j].GetFaceSprite(randomDiceSide);
                if (facesRolled.Count < j + 1)
                {
                    facesRolled.Add(randomDiceSide);
                }
                else
                {
                    facesRolled[j] = randomDiceSide;
                }
            }
            yield return new WaitForSeconds(waitPerIteration);
        }

        // Reroll a success if required by a reroll token
        if (forcedRerollOnSuccessfulAction)
        {
            int indexOfSuccessDie = -1;
            for (int i = 0; i < dice.Count; i++)
            {
                Die die = dice[i];
                if (die.type == DieType.BuildSuccess || die.type == DieType.GatherSuccess || die.type == DieType.ExploreSuccess)
                {
                    indexOfSuccessDie = i;
                    break;
                }
            }
            if (indexOfSuccessDie != -1)
            {
                Die successDie = dice[indexOfSuccessDie];
                int faceRolled = facesRolled[indexOfSuccessDie];
                bool rolledSuccess = (
                    (successDie.type == DieType.BuildSuccess && faceRolled > 1) ||
                    (successDie.type == DieType.GatherSuccess && faceRolled > 0) ||
                    (successDie.type == DieType.ExploreSuccess && faceRolled > 0)
                );
                if (rolledSuccess)
                {
                    StartCoroutine(RerollDie(indexOfSuccessDie));
                    Destroy(rerollToken.gameObject);
                }
                yield return new WaitForSeconds(iterations * waitPerIteration);
            }
        }


        okayButton.interactable = true;
        rerollButton.interactable = true;
    }

    // Helper methods for rerolling dice

    void DisableDieButtons()
    {
        foreach (Image image in images)
        {
            Button imageButton = image.GetComponent<Button>();
            imageButton.enabled = false;
        }
    }

    IEnumerator RerollDie(int imageIndex)
    {
        for (int i = 0; i < iterations; i++)
        {
            int randomDiceSide = Random.Range(0, 6);
            images[imageIndex].sprite = dice[imageIndex].GetFaceSprite(randomDiceSide);
            facesRolled[imageIndex] = randomDiceSide;
            yield return new WaitForSeconds(waitPerIteration);
        }
        okayButton.interactable = true;
    }

    bool CheckForForcedReroll(List<DieType> dieTypes)
    {
        // Build
        if (dieTypes.Contains(DieType.BuildSuccess))
        {
            Transform buildAdventureDeckTokenArea = GameObject.Find("BuildAdventureDeckTokenArea").transform;
            for (int i = 0; i < buildAdventureDeckTokenArea.childCount; i++)
            {
                Transform tokenPosition = buildAdventureDeckTokenArea.GetChild(i);
                if (tokenPosition != null && tokenPosition.childCount > 0 && tokenPosition.GetChild(0) != null)
                {
                    TokenController token = tokenPosition.GetChild(0).GetComponent<TokenController>();
                    if (token != null && token.tokenType == TokenType.Reroll)
                    {
                        rerollToken = token;
                        return true;
                    }
                }
            }
        }
        // Gather
        if (dieTypes.Contains(DieType.GatherSuccess))
        {
            Transform gatherAdventureDeckTokenArea = GameObject.Find("GatherAdventureDeckTokenArea").transform;
            for (int i = 0; i < gatherAdventureDeckTokenArea.childCount; i++)
            {
                Transform tokenPosition = gatherAdventureDeckTokenArea.GetChild(i);
                if (tokenPosition != null && tokenPosition.childCount > 0 && tokenPosition.GetChild(0) != null)
                {
                    TokenController token = tokenPosition.GetChild(0).GetComponent<TokenController>();
                    if (token != null && token.tokenType == TokenType.Reroll)
                    {
                        rerollToken = token;
                        return true;
                    }
                }
            }
        }
        // Explore
        if (dieTypes.Contains(DieType.ExploreSuccess))
        {
            Transform exploreAdventureDeckTokenArea = GameObject.Find("ExploreAdventureDeckTokenArea").transform;
            for (int i = 0; i < exploreAdventureDeckTokenArea.childCount; i++)
            {
                Transform tokenPosition = exploreAdventureDeckTokenArea.GetChild(i);
                if (tokenPosition != null && tokenPosition.childCount > 0 && tokenPosition.GetChild(0) != null)
                {
                    TokenController token = tokenPosition.GetChild(0).GetComponent<TokenController>();
                    if (token != null && token.tokenType == TokenType.Reroll)
                    {
                        rerollToken = token;
                        return true;
                    }
                }
            }
        }
        return false;
    }



}
