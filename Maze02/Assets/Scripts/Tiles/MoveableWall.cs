using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableWall : Tile
{
    private Animator animator;
    private bool visited;
    private Collider2D wallTrigger;

    private GameManager gameManager;
    
    void Start()
    {
        base.Start();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        wallTrigger = AddTileTrigger();
        animator = gameObject.GetComponent<Animator>();

        if (type == TileMap.TileType.Floor)
        {
            animator.Play("tile_floor_unvisited");
            Disable();
        }
        else  // type == TileMap.TileType.moveableWall
        {
            animator.Play("tile_wall");
            Enable();
        }

        MarkAsUnvisited();
    }
    
    void Update()
    {
        
    }
    
    public void Enable()
    {
        animator.SetBool("isWall", true);
        spriteRenderer.sortingOrder = 1;
        
        collider.isTrigger = false;
        wallTrigger.enabled = true;
        gameObject.layer = LayerMask.NameToLayer("Walls");

//        type = TileMap.TileType.moveableWall;
    }
    
    public void Disable()
    {
        animator.SetBool("isWall", false);
//        spriteRenderer.sortingOrder = 0; // moved to animation event
        
        collider.isTrigger = true;
        wallTrigger.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Tiles");

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
    
    
    public bool MarkAsVisited()
    {
        var retVal = !visited;
        visited = true;
        animator.SetBool("wasVisited", true);

        if (retVal)
        {
            gameManager.SubTile();
        }
        return retVal;
    }
    
    public void MarkAsUnvisited()
    {
        visited = false;
        animator.SetBool("wasVisited", false);
        gameManager.AddTile();
    }

    private void MoveToFront()
    {
        spriteRenderer.sortingOrder = 1;
    }
    
    private void MoveToBack()
    {
        spriteRenderer.sortingOrder = 0;
    }
}
