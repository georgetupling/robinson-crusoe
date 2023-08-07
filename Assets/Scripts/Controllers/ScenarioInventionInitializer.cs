using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioInventionInitializer : MonoBehaviour
{
    [SerializeField] InventionCardController invention0;
    [SerializeField] InventionCardController invention1;
    List<InventionCardController> inventionCardControllers;

    bool waitingOnInventionCard;
    Dictionary<Invention, InventionCard> inventionCards = new Dictionary<Invention, InventionCard>();

    void Awake()
    {
        EventGenerator.Singleton.AddListenerToGetInventionCardEvent(OnGetInventionCardEvent);
    }

    void Start()
    {
        inventionCardControllers = new List<InventionCardController> { invention0, invention1 };
        List<Invention> inventions;
        switch (GameSettings.CurrentScenario)
        {
            case Scenario.Castaways:
                inventions = new List<Invention> { Invention.Hatchet, Invention.Mast }; break;

            default:
                Debug.LogError($"Invention list not set for {GameSettings.CurrentScenario} scenario.");
                return;
        }
        StartCoroutine(InitializeScenarioInventions(inventions));
    }

    void OnGetInventionCardEvent(string eventType, Invention invention, InventionCard inventionCard)
    {
        if (eventType != GetInventionCardEvent.Response)
        {
            return;
        }
        inventionCards.Add(invention, inventionCard);
        waitingOnInventionCard = false;
    }

    IEnumerator InitializeScenarioInventions(List<Invention> inventions)
    {
        for (int i = 0; i < inventions.Count; i++)
        {
            Invention invention = inventions[i];
            InventionCardController inventionCardController = inventionCardControllers[i];
            waitingOnInventionCard = true;
            EventGenerator.Singleton.RaiseGetInventionCardEvent(invention);
            while (waitingOnInventionCard)
            {
                yield return null;
            }
            InventionCard inventionCard = inventionCards[invention];
            inventionCardController.InitializeCard(inventionCard);
        }
        this.enabled = false;
    }
}
