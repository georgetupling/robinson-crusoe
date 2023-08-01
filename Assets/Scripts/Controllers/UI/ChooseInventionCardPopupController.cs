using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseInventionCardPopupController : MonoBehaviour
{
    [SerializeField] Image cardImagePrefab;
    [SerializeField] Transform cardArea;
    [SerializeField] RectTransform background;

    List<Image> cardImages = new List<Image>();
    List<InventionCard> inventionCards;

    void Awake() {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
    }

    public void Initialize(List<InventionCard> inventionCards) {
        this.inventionCards = inventionCards;
        // Expands the background to fit the number of cards
        float cardWidth = cardImagePrefab.rectTransform.rect.width;
        float spaceBetweenCards = 15f;
        background.sizeDelta = new Vector2(background.sizeDelta.x + ((inventionCards.Count - 1) * cardWidth) + ((inventionCards.Count - 2) * spaceBetweenCards), background.sizeDelta.y);

        for (int i = 0; i < inventionCards.Count; i++) {
            Image cardImage = Instantiate(cardImagePrefab, cardArea, false);
            cardImages.Add(cardImage);
            if (inventionCards[i].cardSprite == null) {
                Debug.LogError($"Invention card {inventionCards[i].invention} sprite is null.");
            } else {
                cardImage.sprite = inventionCards[i].cardSprite;
            }
            // Positions the card image
            // Card images are anchored to the centre-left, so increasing the x value positions them rightward in a line
            RectTransform cardImageRectTransform = cardImage.rectTransform;
            cardImageRectTransform.anchoredPosition = new Vector3(i * (cardWidth + spaceBetweenCards), 0f, 0f);

            // Adds the listener to the button
            int index = i;
            Button cardImageButton = cardImage.GetComponent<Button>();
            cardImageButton.onClick.AddListener(() => {
                EventGenerator.Singleton.RaiseInventionCardChosenFromSelectionEvent(this.inventionCards, index);
                EventGenerator.Singleton.RaiseEnableMainUIEvent();
                Destroy(gameObject);
            });
        }
    }
}
