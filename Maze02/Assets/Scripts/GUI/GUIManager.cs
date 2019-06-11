using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public bool showEndGameMenu;
    public float mockPercentage;
    
    [Space(20)]
    public Text changeableTiles;
    public FillBar mainFillbar;
    
    private GameManager gameManager;
    private Canvas canvas;
    private GameObject endGameMenu;

    private bool endMenuOn = true;
    
    private void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        endGameMenu = canvas.transform.Find("End Game Menu").gameObject;
    }
    
    private void Update()
    {
        if (endMenuOn && showEndGameMenu)
        {
            endMenuOn = false;
            ShowEndGameMenu(mockPercentage);
        }
    }

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
    
    public void ShowEndGameMenu(float completionPercentage)
    {
        foreach (Transform t in canvas.gameObject.transform)
        {
            t.gameObject.SetActive(false);
        }
        
        endGameMenu.SetActive(true);
        var endGameMenuScript = endGameMenu.GetComponent<EndGameMenu>();
        endGameMenuScript.StartAnimation(completionPercentage);
    }
}
