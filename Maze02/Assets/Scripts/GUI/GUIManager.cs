using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public bool showEndGameMenu;
    public float mockPercentage;
    
    [Space(20)]
    public Text score;
    
    private GameManager gameManager;
    private Canvas canvas;
    private GameObject endGameMenu;

    private bool endMenuOn = true;
    
    private void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        endGameMenu = canvas.transform.Find("End Game Menu").gameObject;
        endGameMenu.SetActive(false);
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
        score.text = gameManager.score.ToString();
    }

    public void UpdateGUI()
    {
        score.text = gameManager.score.ToString();
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
