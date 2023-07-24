using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpritePopupController : ComponentController
{
    [SerializeField] private Image image;
    [SerializeField] private Button okayButton;

    private int sourceComponentId;
    private Sprite sprite;

    protected override void Awake() {
        base.Awake();
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        EventGenerator.Singleton.AddListenerToInitializeSpritePopupEvent(OnInitializeSpritePopupEvent);
    }

    void OnInitializeSpritePopupEvent(int popupId, int sourceComponentId, Sprite sprite) {
        if (this.ComponentId == popupId) {
            this.sourceComponentId = sourceComponentId;
            this.sprite = sprite;
            SetImage();
            SetUpButton();
        }
    }

    void SetImage() {
        if (sprite == null) {
            Debug.LogError($"sprite is null. Failed to initialize sprite popup.");
            return;
        }
        image.sprite = sprite;
    }
    
    void SetUpButton() {
        okayButton.onClick.AddListener(() => {
            ClosePopup();
        });
    }

    void ClosePopup() {
        EventGenerator.Singleton.RaiseEnableMainUIEvent();
        EventGenerator.Singleton.RaiseSpritePopupClosedEvent(sourceComponentId);
        Destroy(gameObject);
    }

}
