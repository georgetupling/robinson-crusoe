using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WoodpilePopupController : MonoBehaviour
{
    [SerializeField] Button plusButton;
    [SerializeField] Button minusButton;
    [SerializeField] TMP_Text valueDisplay;
    [SerializeField] Button confirmButton;

    int value;
    int woodLimit;
    void Awake()
    {
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
        SetUpButtons();
    }
    void SetUpButtons()
    {
        plusButton.onClick.AddListener(() =>
        {
            value = Mathf.Clamp(++value, 0, woodLimit);
            valueDisplay.text = value.ToString();
        });
        minusButton.onClick.AddListener(() =>
        {
            value = Mathf.Clamp(--value, 0, woodLimit);
            valueDisplay.text = value.ToString();
        });
        confirmButton.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseAddWoodToPileEvent(value);
            EventGenerator.Singleton.RaiseLoseWoodEvent(value);
            EventGenerator.Singleton.RaiseEnableMainUIEvent();
            Destroy(gameObject);
        });
    }
    public void SetWoodLimit(int woodLimit)
    {
        this.woodLimit = woodLimit;
    }
}
