using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBrush : MonoBehaviour
{
    public Vector2 brushSize = Vector2.zero;
    public int tileID = 0;
    public SpriteRenderer renderer2D;

    public Color drawingColor = Color.red;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = drawingColor;
        Gizmos.DrawWireCube(transform.position, brushSize);
    }

    public void UpdateBrush(Sprite sprite)
    {
        renderer2D.sprite = sprite;
    }
}
