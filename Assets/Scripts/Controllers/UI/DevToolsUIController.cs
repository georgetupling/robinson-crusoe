using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevToolsUIController : MonoBehaviour
{
    public static DevToolsUIController singleton;

    [SerializeField] private Button gainWood;
    [SerializeField] private Button loseWood;
    [SerializeField] private Button moveWood;
    [SerializeField] private Button gainMorale;
    [SerializeField] private Button loseMorale;
    [SerializeField] private Button gainHide;

    [SerializeField] private Button drawTile;

    [SerializeField] private Button gainRoof;
    [SerializeField] private Button loseRoof;

    [SerializeField] private Button drawBeastCard;

    [SerializeField] private Button drawEventCard;

    [SerializeField] private Button gainDetermination;
    [SerializeField] private Button loseDetermination;

    [SerializeField] private Button drawDiscoveryToken;

    [SerializeField] private Button gainWeapon;
    [SerializeField] private Button spawnWeather;
    
    void Awake() {
        if (singleton == null) {
            singleton = this;
        } else {
            Destroy(gameObject);
            return;
        }
        SetUpButtons();
    }

    void SetUpButtons() {
        gainWood.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseGainWoodEvent(1);
        });
        loseWood.onClick.AddListener(() => {
           EventGenerator.Singleton.RaiseLoseWoodEvent(1);
        });
        moveWood.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseMakeResourcesAvailableEvent();
        });
        gainMorale.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseGainMoraleEvent(1);
        });
        loseMorale.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseLoseMoraleEvent(1);
        });

        drawTile.onClick.AddListener(() => {
            int rand = Random.Range(0, 10);
            EventGenerator.Singleton.RaiseDrawIslandTileEvent(rand);
        });

        gainHide.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseGainHideEvent(1);
        });

        gainRoof.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseGainRoofEvent(1);
        });
        loseRoof.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseLoseRoofEvent(1);
        });

        drawBeastCard.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Beast);
        });

        drawEventCard.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Event);
        });

        gainDetermination.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseGainDeterminationEvent(0, 1);
        });
        loseDetermination.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseLoseDeterminationEvent(0, 1);
        });

        drawDiscoveryToken.onClick.AddListener(() => {
            int rand = Random.Range(1,4);
            EventGenerator.Singleton.RaiseDrawDiscoveryTokenEvent(rand);
        });

        gainWeapon.onClick.AddListener(() => {
            EventGenerator.Singleton.RaiseGainWeaponEvent(1);
        });
        spawnWeather.onClick.AddListener(() => {
            List<TokenType> weatherTokens = new List<TokenType> { TokenType.Storm, TokenType.SnowCloud, TokenType.RainCloud, 
                TokenType.OrangeWeatherDie, TokenType.WhiteWeatherDie, TokenType.RedWeatherDie };
            int randomIndex = Random.Range(0, weatherTokens.Count);
            EventGenerator.Singleton.RaiseSpawnTokenInWeatherAreaEvent(weatherTokens[randomIndex]);
        });
    }

}
