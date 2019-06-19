using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBack : MonoBehaviour
{
    public Color defaultColor;
    public float timeToReturn;

    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    
    void Update()
    {
        var currentColor = spriteRenderer.color;
        if (currentColor != defaultColor)
        {
            StartCoroutine(WaitThenChangeColor());
//            spriteRenderer.color = defaultColor;
        }
    }

    private IEnumerator WaitThenChangeColor()
    {
        yield return new WaitForSeconds(timeToReturn);
        spriteRenderer.color = defaultColor;
    }
}
