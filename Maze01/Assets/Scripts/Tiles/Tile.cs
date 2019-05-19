using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileMap.TileType type;
    public Vector2 index;
    
    private Vector2 tileSize;
    private bool visited;
    
    private SpriteRenderer spriteRenderer;
    private Collider2D collider;
    private Collider2D wallTrigger;
    private Animator animator;
    
    void Start()
    {
        tileSize = GameObject.Find("Tile Map").GetComponent<TileMap>().tileSize;
        collider = AddTileCollider();
        wallTrigger = AddTileTrigger();
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (type == TileMap.TileType.Floor || type == TileMap.TileType.moveableWall)
        {
            animator = gameObject.AddComponent<Animator>();
            var animationController = Resources.Load("Animation/tile") as RuntimeAnimatorController;
            animator.runtimeAnimatorController = animationController;
            if (animationController == null)
            {
                Debug.Log("animation controller not found in Resources/Animation");
            }
        }
        
        MarkAsUnvisited();
        
        switch (type)
        {
            case TileMap.TileType.trap:
                collider.enabled = false;
                wallTrigger.enabled = false;
                break;
            
            case TileMap.TileType.constWall:
                spriteRenderer.sortingOrder = 1;
                
//                animator.Play("tile_wall");
//                animator.SetBool("isWall", true);
        
                collider.isTrigger = false;
                wallTrigger.enabled = false;
                break;
                
            case TileMap.TileType.moveableWall:
                animator.Play("tile_wall");
                Enable();
                break;
            
            case TileMap.TileType.Floor:
                animator.Play("tile_floor_unvisited");
                Disable();
                break;
        }
    }

    public void Enable()
    {
        if (type == TileMap.TileType.constWall || type == TileMap.TileType.trap)
            return;

        animator.SetBool("isWall", true);
        spriteRenderer.sortingOrder = 1;
        
        collider.isTrigger = false;
        wallTrigger.enabled = true;

//        type = TileMap.TileType.moveableWall;
    }
    
    public void Disable()
    {
        if (type == TileMap.TileType.constWall || type == TileMap.TileType.trap)
            return;

        animator.SetBool("isWall", false);
        spriteRenderer.sortingOrder = 0;
        
        collider.isTrigger = true;
        wallTrigger.enabled = false;
        
//        type = TileMap.TileType.Floor;
    }

    public void Toggle()
    {
        if (type == TileMap.TileType.constWall || type == TileMap.TileType.trap)
            return;

        if (type == TileMap.TileType.Floor)
        {
            Enable();
        }

        if (type == TileMap.TileType.moveableWall)
        {
            Disable();
        }
    }

    public void MarkAsVisited()
    {
        if (type == TileMap.TileType.constWall || type == TileMap.TileType.trap)
            return;

        visited = true;
        animator.SetBool("wasVisited", true);
    }
    
    public void MarkAsUnvisited()
    {
        if (type == TileMap.TileType.constWall || type == TileMap.TileType.trap)
            return;

        visited = true;
        animator.SetBool("wasVisited", false);
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
            new Vector2(0, 0) + offset, // top
            new Vector2(tileSize.x / 2, -tileSize.y / 4) + offset,  // top right
            
            new Vector2(tileSize.x / 2, -tileSize.y / 2) + offset,
            new Vector2(0 , -tileSize.y / 4 * 3) + offset,  // bottom
            new Vector2(-tileSize.x / 2, -tileSize.y / 2) + offset,
            
            new Vector2(-tileSize.x / 2, -tileSize.y / 4) + offset  // top left
        };
        return collider;
    }
    
}
