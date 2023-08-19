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
    [SerializeField] private Button spawnWound;
    [SerializeField] private Button gainHide;

    [SerializeField] private Button drawTile;

    [SerializeField] private Button canOnlyRest;

    [SerializeField] private Button gainRoof;
    [SerializeField] private Button loseRoof;

    [SerializeField] private Button drawBeastCard;

    [SerializeField] private Button drawEventCard;

    [SerializeField] private Button gainDetermination;
    [SerializeField] private Button loseDetermination;

    [SerializeField] private Button drawDiscoveryToken;

    [SerializeField] private Button gainWeapon;
    [SerializeField] private Button spawnWeather;
    [SerializeField] private Button exhaustFish;
    [SerializeField] private Button drawEquipment;

    bool fishExhausted;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SetUpButtons();
    }

    void SetUpButtons()
    {
        gainWood.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseGainWoodEvent(1);
        });
        loseWood.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseLoseWoodEvent(1);
        });
        moveWood.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseMakeResourcesAvailableEvent();
        });
        gainMorale.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseGainMoraleEvent(1);
        });
        loseMorale.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseLoseMoraleEvent(1);
        });

        spawnWound.onClick.AddListener(() =>
        {
            List<TokenType> tokenTypes = new List<TokenType> { TokenType.BuildWound, TokenType.GatherWound, TokenType.ExploreWound };
            List<WoundType> woundTypes = new List<WoundType> { WoundType.Head, WoundType.Belly, WoundType.Leg, WoundType.Arm };
            int randToken = Random.Range(0, 3);
            int randWound = Random.Range(0, 4);
            int randPlayer = Random.Range(0, GameSettings.PlayerCount);
            EventGenerator.Singleton.RaiseSpawnWoundTokenEvent(randPlayer, woundTypes[randWound], tokenTypes[randToken]);
        });

        canOnlyRest.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaisePlayerCanOnlyRestThisTurnEvent(0);
        });

        drawTile.onClick.AddListener(() =>
        {
            int rand = Random.Range(0, 10);
            EventGenerator.Singleton.RaiseDrawIslandTileEvent(rand);
        });

        gainHide.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseGainHideEvent(1);
        });

        gainRoof.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseGainRoofEvent(1);
        });
        loseRoof.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseLoseRoofEvent(1);
        });

        drawBeastCard.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Beast);
        });

        drawEventCard.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseDrawCardEvent(Deck.Event);
        });

        gainDetermination.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseGainDeterminationEvent(DeterminationEvent.AllPlayers, 1);
        });
        loseDetermination.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseLoseDeterminationEvent(DeterminationEvent.AllPlayers, 1);
        });

        drawDiscoveryToken.onClick.AddListener(() =>
        {
            int rand = Random.Range(1, 4);
            EventGenerator.Singleton.RaiseDrawDiscoveryTokenEvent(rand);
        });

        gainWeapon.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseGainWeaponEvent(1);
        });
        spawnWeather.onClick.AddListener(() =>
        {
            List<TokenType> weatherTokens = new List<TokenType> { TokenType.Storm, TokenType.SnowCloud, TokenType.RainCloud,
                TokenType.OrangeWeatherDie, TokenType.WhiteWeatherDie, TokenType.RedWeatherDie };
            int randomIndex = Random.Range(0, weatherTokens.Count);
            EventGenerator.Singleton.RaiseSpawnTokenInWeatherAreaEvent(weatherTokens[randomIndex]);
        });

        exhaustFish.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseExhaustSourceByIslandTileIdEvent(8, Source.Fish, !fishExhausted);
            fishExhausted = !fishExhausted;
        });

        drawEquipment.onClick.AddListener(() =>
        {
            EventGenerator.Singleton.RaiseDrawEquipmentCardEvent();
        });
    }

}
