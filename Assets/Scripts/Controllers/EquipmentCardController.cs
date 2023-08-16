using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentCardController : CardController, IPointerClickHandler
{
    public EquipmentCard data { get; private set; }

    private int usesRemaining = 2;
    [SerializeField] private Transform position0;
    [SerializeField] private Transform position1;

    protected override void Awake()
    {
        base.Awake();
        EventGenerator.Singleton.AddListenerToEquipmentActivatedEvent(OnEquipmentActivatedEvent);
    }

    protected override void Start()
    {
        base.Start();
    }

    public void InitializeCard(EquipmentCard equipmentCard)
    {
        if (!isInitialized)
        {
            data = equipmentCard;
            isInitialized = true;
            meshRenderer.material = data.cardMaterial;
        }
        else
        {
            Debug.Log("EquipmentCardController already initialized.");
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (usesRemaining == 0)
        {
            Shake();
            return;
        }
        EventGenerator.Singleton.RaiseSpawnItemActivationPopupEvent(data);
    }

    void OnEquipmentActivatedEvent(Equipment equipment)
    {
        if (equipment != data.equipment)
        {
            return;
        }
        TokenController prefab = PrefabLoader.Singleton.GetPrefab(TokenType.BlackMarker);
        if (usesRemaining == 2)
        {
            TokenController newToken = Instantiate(prefab, position0, false);
            Vector3 localPosition = new Vector3(0, 0, -ComponentDimensions.GetHeight(TokenType.BlackMarker) / 2f);
            newToken.transform.localPosition = localPosition;
        }
        else if (usesRemaining == 1)
        {
            TokenController newToken = Instantiate(prefab, position1, false);
            Vector3 localPosition = new Vector3(0, 0, -ComponentDimensions.GetHeight(TokenType.BlackMarker) / 2f);
            newToken.transform.localPosition = localPosition;
        }
        usesRemaining--;
    }
}
