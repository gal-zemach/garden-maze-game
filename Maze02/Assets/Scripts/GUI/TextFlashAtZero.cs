using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFlashAtZero : MonoBehaviour
{
    public List<Color> colors;
    public float timeForEachColor;
    
    private Text text;
    private WaitForSeconds changeTime;
    private int colorIndex;

    private GameManager gameManager;
    private bool currentlyFlashing;

    void Start()
    {
        text = GetComponent<Text>();
        changeTime = new WaitForSeconds(timeForEachColor);
        colorIndex = 0;
        
        if (colors == null || colors.Count == 0)
        {
            colors = new List<Color>();
            colors.Add(Color.white);
            colors.Add(IsoVectors.GAME_RED);
//            colors.Add(Color.black);
        }

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        StartCoroutine(ChangeColor());
    }

    private IEnumerator ChangeColor()
    {
        if (gameManager.changeableTiles != 0)
        {
            text.color = Color.white;
        }
        else
        {
            text.color = colors[colorIndex];
        }
        
        yield return changeTime;
        colorIndex = (colorIndex + 1) % colors.Count;
        
        StartCoroutine(ChangeColor());
    }
}
