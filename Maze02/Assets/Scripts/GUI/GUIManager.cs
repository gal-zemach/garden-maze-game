using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public Text grassTilesLeft, changeableTiles;
    public FillBar mainFillbar;
    
    private GameManager gameManager;
    
    public void StartGUI()
    {
        gameManager = GetComponent<GameManager>();
        
        mainFillbar.SetMax(gameManager.totalGrassToCut);
        mainFillbar.SetThreshold(gameManager.percentageToCompletion);
    }

    public void UpdateGUI()
    {
        if (grassTilesLeft == null || changeableTiles == null)
            return;
        
        grassTilesLeft.text = "Tiles Left: " + gameManager.grassTilesLeft;
        UpdateChangeableTiles();
        
        
        mainFillbar.SetValue(gameManager.totalGrassToCut - gameManager.grassTilesLeft);
//        Debug.Log("updating to " + (gameManager.totalGrassToCut - gameManager.grassTilesLeft));
    }

    private void UpdateChangeableTiles()
    {
        var changeableTilesValue = gameManager.changeableTiles;
        if (changeableTilesValue == 0)
        {
            changeableTiles.color = Color.red;
        }
        else
        {
            changeableTiles.color = Color.white;
        }
        changeableTiles.text = changeableTilesValue.ToString();
    }
}
