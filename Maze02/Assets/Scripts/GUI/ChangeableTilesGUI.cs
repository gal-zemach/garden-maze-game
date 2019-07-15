using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeableTilesGUI : MonoBehaviour
{
    private GameManager gameManager;
    private Text tilesCount;
    private Slider tilesSlider;
    
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        tilesCount = GetComponentInChildren<Text>();
        tilesSlider = GetComponentInChildren<Slider>();
        tilesSlider.maxValue = gameManager.GrassToTileRatio.x;
    }

    
    void Update()
    {
        tilesCount.text = gameManager.changeableTiles.ToString();

        var sliderValue = gameManager.grassCut % gameManager.GrassToTileRatio.x;
        tilesSlider.value = sliderValue;
    }
}
