using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoutingPopupController : MonoBehaviour
{
    [SerializeField] Image image0;
    [SerializeField] Image image1;

    void Awake() {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
    }

    public void Initialize(DiscoveryTokenController token1, DiscoveryTokenController token2) {
        // Sets the image sprites and adds listeners to their buttons
        image0.sprite = token1.data.tokenSprite;
        image1.sprite = token2.data.tokenSprite;

        Button button0 =  image0.GetComponent<Button>();
        button0.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseSpritePopupClosedEvent(token1.ComponentId);
            Destroy(token2.gameObject);
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });

        Button button1 =  image1.GetComponent<Button>();
        button1.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseSpritePopupClosedEvent(token2.ComponentId);
            Destroy(token1.gameObject);
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }
}
