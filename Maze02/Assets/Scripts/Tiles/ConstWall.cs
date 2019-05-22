using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstWall : Tile
{
    void Start()
    {
        base.Start();
        spriteRenderer.sortingOrder = 1;
    }

    void Update()
    {
        
    }
}
