using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public Text changeableTiles;
    public FillBar mainFillbar;
    
    private GameManager gameManager;
    
    public void StartGUI()
    {
        gameManager = GetComponent<GameManager>();

        if (mainFillbar != null)
        {
            mainFillbar.SetMax(gameManager.totalGrassToCut);
            mainFillbar.SetThreshold(gameManager.percentageToCompletion);
        }
    }

    public void UpdateGUI()
    {
        if (changeableTiles != null)
        {
            UpdateChangeableTiles();
        }

        if (mainFillbar != null)
        {
            mainFillbar.SetValue(gameManager.totalGrassToCut - gameManager.grassTilesLeft);
        }
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
