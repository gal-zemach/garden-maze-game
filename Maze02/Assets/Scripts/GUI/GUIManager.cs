using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public bool showEndGameMenu;
    public int mockScore;

    [Space(20)] 
    public bool immediateScore;
    
    [Space(20)]
    public Vector2 numberToMouseOffset = new Vector2(40, -40);
    
    [Space(20)]
    public Text score;
    
    private GameManager gameManager;
    private Canvas canvas;
    private GameObject endGameMenu;
    private Slider playerSlider;
    private SliderFill playerSliderChanger;
    private Transform mouseCanvas;
    private Transform changeableTilesParent;
    private Text changeableTilesText;

    private bool endMenuOn = true;
    
    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        endGameMenu = canvas.transform.Find("End Game Menu").gameObject;
        endGameMenu.SetActive(false);
        
        mouseCanvas = GameObject.Find("CanvasMouse").transform;
        changeableTilesParent = mouseCanvas.Find("ChangeableTiles");
        changeableTilesText = mouseCanvas.Find("ChangeableTiles/Text").GetComponent<Text>();

        mockScore = 0;
        UpdateChangeableTiles();
    }
    
    private void Update()
    {
        var offset = new Vector3(numberToMouseOffset.x, numberToMouseOffset.y, 500);
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        changeableTilesParent.position = mouseWorldPos;
//        changeableTilesText.text = gameManager.changeableTiles.ToString();
        
        if (playerSlider == null && gameManager.player != null)
        {
            var sliderGO = gameManager.player.transform.Find("Canvas/Slider");
            playerSlider = sliderGO.GetComponent<Slider>();
            playerSlider.maxValue = gameManager.GrassToTileRatio.x;
            playerSlider.value = 0;
            playerSliderChanger = sliderGO.GetComponent<SliderFill>();
        }

        if (playerSlider != null)
        {
//            var sliderValue = gameManager.grassCut % gameManager.GrassToTileRatio.x;
        }
        
        if (endMenuOn && showEndGameMenu)
        {
            endMenuOn = false;
            ShowEndGameMenu(mockScore);
        }
    }

    public void StartGUI()
    {
        score.text = gameManager.score.ToString();
    }

    public void UpdateGUI()
    {
        score.text = gameManager.score.ToString();
    }

    public void ShowEndGameMenu(int finalScore)
    {
        HideMouseUI();
        foreach (Transform t in canvas.gameObject.transform)
        {
            t.gameObject.SetActive(false);
        }
        
        endGameMenu.SetActive(true);
        var endGameMenuScript = endGameMenu.GetComponent<EndGameMenu>();
        endGameMenuScript.immediateScore = immediateScore;
        endGameMenuScript.StartAnimation(finalScore);
    }

    public void IncrementGrassCut()
    {
        playerSliderChanger.IncrementValue();
    }

    public void HideMouseUI()
    {
        mouseCanvas.gameObject.SetActive(false);
    }

    public void UpdateChangeableTiles()
    {
        changeableTilesText.text = gameManager.changeableTiles.ToString();
    }
}
