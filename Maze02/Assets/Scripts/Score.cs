using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;

    private GameManager gameManager;
    
    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        scoreText.text = "Tiles Left: " + gameManager.grassTilesLeft;
    }
}
