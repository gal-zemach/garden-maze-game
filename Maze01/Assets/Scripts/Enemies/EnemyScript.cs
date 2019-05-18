﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    protected Vector2 gridCell, tileSize;
    protected TileMap map;
    protected IsoCollider isoCollider;

    protected void EnemyBaseStart()
    {
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        tileSize = new Vector2(map.tileSize.x, map.tileSize.y / 2);
        
        isoCollider = gameObject.AddComponent<IsoCollider>();
        isoCollider.tileSize = 35; // 64 makes for larger than map tiles' tiles, this is smaller than the map tiles
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("enemyScript: hit player");
            var playerScript = other.gameObject.GetComponentInParent<PlayerScript>();
//            playerScript.ReduceHealth();
        }
    }
}
