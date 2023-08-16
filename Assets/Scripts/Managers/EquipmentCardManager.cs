using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EquipmentCardManager : MonoBehaviour
{
    private static EquipmentCardManager singleton;

    [SerializeField] private Transform position0;
    [SerializeField] private Transform position1;
    [SerializeField] private Transform position2;
    [SerializeField] private Transform position3;
    private List<Transform> positions;

    [SerializeField] private EquipmentCardController prefab;

    private List<EquipmentCard> equipmentCards = new List<EquipmentCard>();
    private Stack<EquipmentCard> equipmentCardDeck = new Stack<EquipmentCard>();
    private List<EquipmentCardController> spawnedEquipment = new List<EquipmentCardController>();

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogError("Scene contains duplicate EquipmentCardManager class.");
            return;
        }
        positions = new List<Transform> { position0, position1, position2, position3 };
        LoadEquipmentCards();
        SpawnStartingEquipment();
    }

    void LoadEquipmentCards()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(Path.Combine("Data", "equipment-cards"));
        string jsonString = jsonTextAsset.text;
        List<string> jsonStrings = Json.ToList(jsonString);
        foreach (string str in jsonStrings)
        {
            EquipmentCardUnprocessedData data = JsonUtility.FromJson<EquipmentCardUnprocessedData>(str);
            EquipmentCard equipmentCard = new EquipmentCard(data);
            equipmentCards.Add(equipmentCard);
            equipmentCardDeck.Push(equipmentCard);
        }
        DeckShuffler.Singleton.ShuffleDeck(equipmentCardDeck);
    }

    void SpawnStartingEquipment()
    {
        for (int i = 0; i < 2; i++)
        {
            DrawEquipmentCard();
        }
    }

    void DrawEquipmentCard()
    {
        // Finds a vacant position
        Transform vacantPosition = null;
        foreach (Transform position in positions)
        {
            if (position.childCount == 0)
            {
                vacantPosition = position;
                break;
            }
        }
        if (vacantPosition == null)
        {
            Debug.LogError("Failed to spawn equipment card.");
            return;
        }
        // Spawns the equipment card
        EquipmentCard equipmentCard = equipmentCardDeck.Pop();
        if (equipmentCard == null)
        {
            return;
        }
        EquipmentCardController newCard = Instantiate(prefab, vacantPosition, false);
        newCard.transform.localPosition = Vector3.zero;
        newCard.InitializeCard(equipmentCard);
        spawnedEquipment.Add(newCard);
    }
}
