using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LargeBar : MonoBehaviour
{
    public GameObject unit;
    public RectTransform bar;
    public Transform barStart, barEnd;
    public int spaceBetweenUnits = 2;

    private int maxValue, currentValue;
    private float valueDelta, nMaxUnits, nCurrentUnits;
    private Vector3 currentPosition, positionDelta;
    private Transform unitsParent;

    private Canvas canvas;

    private float barLength;
    
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        var canvasScale = canvas.transform.localScale.x;
        
        unitsParent = transform.Find("Units Parent");

//        barLength = bar.sizeDelta.x * bar.localScale.x * canvasScale;
        barLength = (barEnd.position - barStart.position).magnitude;
        
        var unitTransform = unit.GetComponent<RectTransform>();
        var unitWidth = unitTransform.sizeDelta.x * unitTransform.localScale.x * canvasScale;
        
        nMaxUnits = Mathf.Floor(barLength / (unitWidth + spaceBetweenUnits));
        positionDelta = new Vector3((unitWidth + spaceBetweenUnits), 0, 0);
        
        currentValue = 0;
        nCurrentUnits = 0;
        
        unit.SetActive(false);
        initBar();
    }
    
    private void initBar()
    {
        if (maxValue == 0)
            return;

        for (int j = 0; j < unitsParent.childCount; j++)
        {
            Destroy(unitsParent.GetChild(j));
        }
        
        currentPosition = barStart.localPosition;
        nCurrentUnits = 0;
    }

    public void SetMaxValue(int value)
    {
        maxValue = value;
        valueDelta = Mathf.Floor(maxValue / nMaxUnits);
        initBar();
        
        Debug.Log(value);
        Debug.Log(nMaxUnits);
        Debug.Log(valueDelta);
    }
    
    public void SetCurrentValue(int value)
    {
        currentValue = value;
    }
    
    public void IncrementCurrentValue()
    {
        currentValue++;
        if (currentValue % valueDelta == 0)
            drawNewUnit();
    }

    private void drawNewUnit()
    {
        var newUnit = Instantiate(unit, unitsParent);
        newUnit.name = "unit_" + nCurrentUnits;
        newUnit.transform.localPosition = currentPosition;
        newUnit.SetActive(true);
        
        currentPosition += positionDelta;
        nCurrentUnits++;
    }
}
