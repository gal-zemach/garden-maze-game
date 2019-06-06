﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClicker : MonoBehaviour
{
    public bool infiniteTiles;
    public bool toggleBackTimer;
    public float secondsToToggleBack = 3;
    
    private GameManager gameManager;
    private PlayerScript playerScript;
    private Tile lastChangedTile;
    private GameObject marker, wallMarker;

    private WaitForSeconds timeToToggleBack;
    private bool enabled;
    
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        playerScript = gameManager.player.GetComponent<PlayerScript>();
        marker = transform.Find("Marker").gameObject;
        wallMarker = transform.Find("Wall Marker").gameObject;
        
        timeToToggleBack = new WaitForSeconds(secondsToToggleBack);
    }

    void Update()
    {
        if (!enabled)
            return;
        
        var mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit2D hit;
        hit = Physics2D.Raycast(ray.origin, ray.direction);
        
        if (hit.collider != null)
        {
            var tile = hit.collider.gameObject.GetComponent<Tile>();
            var moveableWall = tile as MoveableWall; // this is only used so the marker doesn't move to a non-clickable tile
            if (moveableWall != null)
            {
                if (tile.index == playerScript.gridCell || moveableWall.infected)
                {
                    marker.SetActive(false);
                    wallMarker.SetActive(false);
                    return;
                }

                UpdateMarker(moveableWall, hit.collider.transform.position);
                
                if (!Input.GetMouseButton(0))
                    return;
                
                if (tile.index == playerScript.gridCell)
                {
                    Debug.Log("TileClicker: clicked on player's tile");
                }
                else if (moveableWall.infected)
                {
                    Debug.Log("TileClicker: clicked on infected tile");
                }
                else if (Input.GetMouseButtonDown(0) || lastChangedTile != tile)
                {
                    OnTileClick(tile);
                    lastChangedTile = tile;
                }
            }
        }
    }

    private void OnTileClick(Tile tile)
    {
//        Debug.Log("TileClicker: clicked on " + tile.gameObject.name);
        if (infiniteTiles)
        {
            ToggleTile(tile);
            return;
        }
        
        if (gameManager.changeableTiles <= 0)
            return;
        
        var toggled = ToggleTile(tile);
        if (toggled)
            gameManager.changeableTiles--;
        
        if (toggleBackTimer)
            StartCoroutine(ToggleTileBack(tile));
    }

    private bool ToggleTile(Tile tile)
    {
        if (tile.type != TileMap.TileType.Floor && tile.type != TileMap.TileType.moveableWall)
            return false;
        
        var moveableWall = tile as MoveableWall;
        if (moveableWall == null)
        {
            Debug.LogError("TileClicker: Could not cast tile to MoveableWall");
            return false;
        }
        
        moveableWall.Toggle();
                
        // should be in Tile class but it doesn't work from there!!
        if (moveableWall.type == TileMap.TileType.Floor)
        {
            moveableWall.type = TileMap.TileType.moveableWall;
        }
        else if (moveableWall.type == TileMap.TileType.moveableWall)
        {
            moveableWall.type = TileMap.TileType.Floor;
        }

        return true;
    }

    private IEnumerator ToggleTileBack(Tile tile)
    {
        yield return timeToToggleBack;
        ToggleTile(tile);
    }

    private void UpdateMarker(MoveableWall moveableWall, Vector3 pos)
    {
        if (moveableWall.type == TileMap.TileType.Floor)
        {
            wallMarker.SetActive(false);
            marker.SetActive(true);
            marker.transform.position = pos;
        }
        else
        {
            marker.SetActive(false);
            wallMarker.SetActive(true);
            wallMarker.transform.position = pos + new Vector3(0, 0, -1);
        }
    }

    public void EnableControls()
    {
        enabled = true;
    }

    public void DisableControls()
    {
        enabled = false;
    }
}


//private List<Tile> tileQueue;
//tileQueue = new List<Tile>();

//        if (tileQueue.Remove(tile))
//        {
//            return;
//        }
//            
//        tileQueue.Add(tile);
//        if (tileQueue.Count > maxChangedTiles)
//        {
//            var oldTile = tileQueue[0];
//            ToggleTile(oldTile);
//            tileQueue.RemoveAt(0);
//        }