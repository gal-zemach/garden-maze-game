﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClicker : MonoBehaviour
{
    public int maxChangedTiles = 3;
    private PlayerScript playerScript;
    private Queue<Tile> tileQueue;
    
    void Start()
    {
        playerScript = GetComponent<GameManager>().player.GetComponent<PlayerScript>();
        tileQueue = new Queue<Tile>();
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
                    OnTileClick(tile);
                }
            }
        }
    }

    private void OnTileClick(Tile tile)
    {
        ToggleTile(tile);
        Debug.Log("TileClicker: clicked on " + tile.gameObject.name);
        tileQueue.Enqueue(tile);
        if (tileQueue.Count > maxChangedTiles)
        {
            var oldTile = tileQueue.Dequeue();
//            Debug.Log("TileClicker: unclicked on " + tile.gameObject.name);
            ToggleTile(oldTile);
        }
    }

    private void ToggleTile(Tile tile)
    {
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
