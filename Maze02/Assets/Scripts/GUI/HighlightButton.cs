using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class HighlightButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    private GameObject buttonHighlight;
    
    void Start()
    {
        buttonHighlight = transform.Find("Button Highlight").gameObject;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonHighlight.gameObject.SetActive(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonHighlight.gameObject.SetActive(false);
    }
}
