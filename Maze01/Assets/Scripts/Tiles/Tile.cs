using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileMap.TileType type;
    public Vector2 index;
    
    private Vector2 tileSize;
    
    private SpriteRenderer spriteRenderer;
    private Collider2D collider;
    private Collider2D wallTrigger;
    private Animator animator;
    
    void Start()
    {
        tileSize = GameObject.Find("Tile Map").GetComponent<TileMap>().tileSize;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        collider = AddTileCollider();
        wallTrigger = AddTileTrigger();
        
        switch (type)
        {
            case TileMap.TileType.constWall:
                spriteRenderer.sortingOrder = 1;
                
                animator.Play("wall_asWall");
                animator.SetBool("isWall", true);
        
                collider.isTrigger = false;
                wallTrigger.enabled = false;
                break;
                
            case TileMap.TileType.moveableWall:
                animator.Play("wall_asWall");
                Enable();
                break;
            
            case TileMap.TileType.Floor:
                animator.Play("wall_asFloor");
                Disable();
                break;
        }
    }

    public void Enable()
    {
        if (type == TileMap.TileType.constWall)
            return;

        animator.SetBool("isWall", true);
        spriteRenderer.sortingOrder = 1;
        
        collider.isTrigger = false;
        wallTrigger.enabled = true;

//        type = TileMap.TileType.moveableWall;
    }
    
    public void Disable()
    {
        if (type == TileMap.TileType.constWall)
            return;

        animator.SetBool("isWall", false);
        spriteRenderer.sortingOrder = 0;
        
        collider.isTrigger = true;
        wallTrigger.enabled = false;
        
//        type = TileMap.TileType.Floor;
    }

    public void Toggle()
    {
        if (type == TileMap.TileType.Floor)
        {
            Enable();
        }

        if (type == TileMap.TileType.moveableWall)
        {
            Disable();
        }
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
    
    private Collider2D AddTileTrigger()
    {
        var collider = gameObject.AddComponent<PolygonCollider2D>();
        collider.isTrigger = true;
        var offset = new Vector2(0, tileSize.y / 4);
        collider.points = new[]
        {
            new Vector2(0, tileSize.y / 4) + offset,
            new Vector2(tileSize.x / 2, 0) + offset,
            
            
            new Vector2(tileSize.x / 2, -tileSize.y / 2) + offset,
            
            new Vector2(0 , -tileSize.y / 4 * 3) + offset,
            
            new Vector2(-tileSize.x / 2, -tileSize.y / 2) + offset,
            
            
            new Vector2(-tileSize.x / 2, 0) + offset
        };
        return collider;
    }
    
}
