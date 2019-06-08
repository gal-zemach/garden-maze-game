using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    public Slider slider;

    private RectTransform thresholdMarker, sliderBg;
    private float currentValue;
    private bool maxSet;
    
    public float CurrentValue
    {
        get => currentValue;
        set
        {
            currentValue = value;
            slider.value = value;
        }
    }

    void Start()
    {
        slider = GetComponent<Slider>();
        CurrentValue = 0;
        
        thresholdMarker = transform.Find("Threshold Marker").GetComponent<RectTransform>();
        sliderBg = transform.Find("Background").GetComponent<RectTransform>();
    }

    public void SetMax(int value)
    {
        if (value == 0)
            return;
        
        slider.maxValue = value;        
        maxSet = true;
    }

    public void SetThreshold(float value)
    {
        var markerPosition = value * sliderBg.rect.width;
        thresholdMarker.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, markerPosition, 4);
    }
    
    public void Increment()
    {
        if (!maxSet)
            return;
        
        CurrentValue++;
    }

    public void SetValue(float value)
    {
        if (!maxSet)
            return;
        
        CurrentValue = value;
    }
}

//    private void UpdateSlider(float value)
//    {
//        if (currentValue % valueToUnit == 0)
//        {
//            slider.value = currentValue;
//        }
//        else
//        {
//            slider.value = Mathf.Floor(currentValue / valueToUnit) * valueToUnit;
//        }
//    }
