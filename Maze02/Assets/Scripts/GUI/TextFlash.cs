using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFlash : MonoBehaviour
{
    public List<Color> colors;
    public float timeForEachColor;
    
    private Text text;
    private WaitForSeconds changeTime;
    private int colorIndex;

    void Start()
    {
        text = GetComponent<Text>();
        changeTime = new WaitForSeconds(timeForEachColor);
        colorIndex = 0;
        
        if (colors == null || colors.Count == 0)
        {
            colors = new List<Color>();
//            colors.Add(Color.white);
            colors.Add(IsoVectors.GAME_RED);
            colors.Add(Color.black);
        }
        
        StartCoroutine(ChangeColor());
    }

    private IEnumerator ChangeColor()
    {
        text.color = colors[colorIndex];
        yield return changeTime;

        colorIndex = (colorIndex + 1) % colors.Count;
        StartCoroutine(ChangeColor());
    }
}
