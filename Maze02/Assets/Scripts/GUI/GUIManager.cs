using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public Text grassTilesLeft, changeableTiles;
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        if (grassTilesLeft == null || changeableTiles == null)
            return;
        
        grassTilesLeft.text = "Tiles Left: " + gameManager.grassTilesLeft;
        changeableTiles.text = gameManager.changeableTiles.ToString();
    }
}
