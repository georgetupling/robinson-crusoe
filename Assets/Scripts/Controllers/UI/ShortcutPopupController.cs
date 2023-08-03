using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutPopupController : MonoBehaviour
{
    [SerializeField] Button foodButton;
    [SerializeField] Button woodButton;

    void Awake()
    {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        SetUpButtons();
    }
    void SetUpButtons()
    {
        foodButton.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseGainFoodEvent(1);
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
        woodButton.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseGainWoodEvent(1);
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }
}
