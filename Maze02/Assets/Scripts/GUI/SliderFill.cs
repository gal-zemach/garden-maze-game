using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 
[RequireComponent(typeof(Slider))]
public class SliderFill : MonoBehaviour {
 
    private float slowFillSpeed = 2.0f;
    private float fastFillSpeed = 10.0f;
    private float fillSpeed;
 
    private Slider slider;
    private RectTransform fillRect;
    private float targetValue = 0f;
    private float curValue = 0f;
    private int loops;
    private PlayerScript playerScript;
    private AudioManager audioManager;
    private GUIManager guiManager;
 
    void Awake () {
        slider = GetComponent<Slider>();
        fillRect = slider.fillRect;
        targetValue = curValue = slider.value;
        playerScript = GetComponentInParent<PlayerScript>();
        fillSpeed = slowFillSpeed;

        var gameManagerGO = GameObject.Find("Game Manager");
        guiManager = gameManagerGO.GetComponent<GUIManager>();
        audioManager = gameManagerGO.GetComponent<AudioManager>();
    }
         
    // Update is called once per frame
    void Update () {
        if (playerScript.isRunning)
        {
            fillSpeed = fastFillSpeed;
        }
        else
        {
            fillSpeed = slowFillSpeed;
        }
        
        if (loops == 0)
        {
            curValue = Mathf.MoveTowards(curValue, targetValue, Time.deltaTime * fillSpeed);
        }
        else
        {
            curValue = Mathf.MoveTowards(curValue, slider.maxValue, Time.deltaTime * fillSpeed);
        }
        
        Vector2 fillAnchor = fillRect.anchorMax;
        fillAnchor.x = Mathf.Clamp01(curValue/slider.maxValue);
        fillRect.anchorMax = fillAnchor;
        slider.value = curValue;

        if (slider.value == slider.maxValue)
        {
            audioManager.PlayNewChangeableTile();
            guiManager.UpdateChangeableTiles();
            slider.value = curValue = 0;
            loops--;
        }
    }

    public void IncrementValue()
    {
        targetValue++;
        if (targetValue == slider.maxValue)
        {
            loops++;
            targetValue = 0;
        }
    }   
}