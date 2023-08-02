using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReconnaissancePopupController : MonoBehaviour
{
    [SerializeField] Image image0;
    [SerializeField] Image image1;
    [SerializeField] Image image2;
    List<Image> images;

    const float TileThickness = 0.016f;
    
    void Awake() {
        images = new List<Image> { image0, image1, image2 };
        EventGenerator.Singleton.RaiseDisableMainUIEvent();
    }

    public void Initialize(List<IslandTileController> topTiles, Stack<IslandTileController> deck) {
        // Sets the image sprites and adds listeners to their buttons
        for (int i = 0; i < 3; i++) {
            Sprite tileSprite = topTiles[i].IslandTile.Sprite;
            if (tileSprite != null) {
                images[i].sprite = tileSprite;
            }
            int index = i;
            Button button = images[i].GetComponent<Button>();
            button.onClick.AddListener(() => {
                PutTileOnTop(index, topTiles, deck);
                EventGenerator.Singleton.RaiseEnableMainUIEvent();
                Destroy(gameObject);
            });
        }
    }

    void PutTileOnTop(int index, List<IslandTileController> topTiles, Stack<IslandTileController> deck) {
        // Turns the deck into a List and reinserts the 3 drawn tiles
        List<IslandTileController> deckList = new List<IslandTileController>(deck);
        IslandTileController selectedTile = topTiles[index];
        topTiles.RemoveAt(index);
        foreach (IslandTileController unselectedTile in topTiles) {
            deckList.Add(unselectedTile);
        }
        deckList.Sort((a, b) => Random.Range(-1, 2));
        deckList.Add(selectedTile);
        // Reconstructs the deck
        deck.Clear();
        foreach (IslandTileController tile in deckList) {
            deck.Push(tile);
            tile.transform.localPosition = new Vector3 (0, 0, (-1) * deck.Count * TileThickness);
        }
        // Turns the selected tile face up
        selectedTile.transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }
}
