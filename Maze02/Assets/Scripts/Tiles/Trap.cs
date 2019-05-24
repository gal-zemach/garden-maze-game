using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Tile
{
    public GameObject replacementTile;
    private TileMap map;
    
    void Start()
    {
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
    }

    void Update()
    {
        
    }

    public void changeToFloorTile()
    {
        var floorTile = (GameObject) Instantiate(replacementTile, transform.parent);
        var tilePos = transform.position;
        floorTile.name = gameObject.name;
        floorTile.transform.position = tilePos;

        var floorTileScript = floorTile.GetComponent<Tile>();
        floorTileScript.index = index;
        map.tiles[map.TileIndex((int) index.x, (int) index.y)] = floorTileScript;
        
        Destroy(gameObject);
    }
}
