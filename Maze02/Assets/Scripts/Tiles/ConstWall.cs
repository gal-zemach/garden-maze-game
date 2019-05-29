using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstWall : Tile
{
    void Start()
    {
        base.Start();
        spriteRenderer.sortingOrder = 1;
        map.UpdateWalkabilityGrid(index, false);
    }

    void Update()
    {
        
    }
}
