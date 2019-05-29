using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileMap.TileType type;
    public Vector2Int index;
    
    protected Vector2 tileSize;
    
    protected SpriteRenderer spriteRenderer;
    protected Collider2D collider;
    protected TileMap map;
    
    protected void Start()
    {
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        tileSize = map.tileSize;
        collider = AddTileCollider();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private Collider2D AddTileCollider()
    {
        var collider = gameObject.AddComponent<PolygonCollider2D>();
        var offset = new Vector2(0, -tileSize.y / 4);
        collider.points = new[]
        {
            new Vector2(0, tileSize.y / 4) + offset,
            new Vector2(tileSize.x / 2, 0) + offset,
            new Vector2(0 , -tileSize.y / 4) + offset,
            new Vector2(-tileSize.x / 2, 0) + offset
        };
        return collider;
    }
}


//        switch (type)
//        {
//            case TileMap.TileType.trap:
//                collider.enabled = false;
//                wallTrigger.enabled = false;
//                break;
//            
//            case TileMap.TileType.constWall:
//                spriteRenderer.sortingOrder = 1;
//                
////                animator.Play("tile_wall");
////                animator.SetBool("isWall", true);
//        
//                collider.isTrigger = false;
//                wallTrigger.enabled = false;
//                break;
//                
//            case TileMap.TileType.moveableWall:
//                animator.Play("tile_wall");
//                Enable();
//                break;
//            
//            case TileMap.TileType.Floor:
//                animator.Play("tile_floor_unvisited");
//                Disable();
//                break;
//        }
