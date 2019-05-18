using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClicker : MonoBehaviour
{

    private PlayerScript playerScript;
    
    void Start()
    {
        playerScript = GetComponent<GameManager>().player.GetComponent<PlayerScript>();
    }

    void Update()
    {
        var mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit2D hit;
        hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (Input.GetMouseButtonDown(0) && hit.collider != null)
        {
            var tile = hit.collider.gameObject.GetComponent<Tile>();
            if (tile != null)
            {
                if (tile.index == playerScript.gridCell)
                {
                    Debug.Log("TileClicker: clicked on player's tile");
                }
                else
                {
                    Debug.Log("TileClicker: clicked on " + hit.collider.name);
                    tile.Toggle();
                
                    // should be in Tile class but it doesn't work from there!!
                    if (tile.type == TileMap.TileType.Floor)
                    {
                        tile.type = TileMap.TileType.moveableWall;
                    }
                    else if (tile.type == TileMap.TileType.moveableWall)
                    {
                        tile.type = TileMap.TileType.Floor;
                    }
                }
            }
        }
    }
}
