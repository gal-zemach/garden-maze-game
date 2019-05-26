using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClicker : MonoBehaviour
{
    public GameObject marker, wallMarker;
    public int maxChangedTiles = 3;
    private PlayerScript playerScript;
    private List<Tile> tileQueue;
    private Tile lastChangedTile;
    
    void Start()
    {
        playerScript = GetComponent<GameManager>().player.GetComponent<PlayerScript>();
        tileQueue = new List<Tile>();
    }

    void Update()
    {
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
//                if (Mathf.Approximately((tile.index - playerScript.gridCell).magnitude, 1))
                if (tile.index == playerScript.gridCell)
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
        ToggleTile(tile);
//        Debug.Log("TileClicker: clicked on " + tile.gameObject.name);
        if (tileQueue.Remove(tile))
        {
            return;
        }
            
        tileQueue.Add(tile);
        if (tileQueue.Count > maxChangedTiles)
        {
            var oldTile = tileQueue[0];
            ToggleTile(oldTile);
            tileQueue.RemoveAt(0);
        }
    }

    private void ToggleTile(Tile tile)
    {
        if (tile.type != TileMap.TileType.Floor && tile.type != TileMap.TileType.moveableWall)
            return;
        
        var moveableWall = tile as MoveableWall;
        if (moveableWall == null)
        {
            Debug.LogError("TileClicker: Could not cast tile to MoveableWall");
            return;
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
}
