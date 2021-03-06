﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NesScripts.Controls.PathFind;
using Random = UnityEngine.Random;

public class TileSpreadingEnemy : MonoBehaviour
{
    public GameObject bodyElementPrefab;
    public MultiSidedTile spriteUpdater;
    public Vector2Int bodyStart;
    
    [Space(20)] 
    public bool tileTrigger;
    public Vector2 triggerIndex;
    
    [Space(20)]
    public float timeToStart;
    public float timeToNextSpread;
    public int cellsPerSpread = 1;
    public int directionality = 1;
    public bool isSpreading;
    [Space(20)]
    public int bodySize = 0;
    
    private GameObject[,] body;
    private Transform bodyElementsParent;
    private WaitForSeconds secondsToNextSpread;
    
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private TileMap map;
    private PlayerScript playerScript;
    private Animator animator;

    private float directionalityFactor;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        playerScript = gameManager.player.GetComponent<PlayerScript>();

        bodyElementsParent = transform.Find("Body Elements Parent");
        secondsToNextSpread = new WaitForSeconds(timeToNextSpread);

        // align bodyStart & Position
        if (bodyStart == Vector2Int.zero)
        {
            var goPos = transform.position;
            var index = IsoVectors.WorldToIsoRounded(goPos, map.actualTileSize);
            bodyStart = new Vector2Int((int)index.x, (int)index.y);
        }
        Vector3 pos = IsoVectors.IsoToWorld(bodyStart, map.actualTileSize);
        pos.z = bodyStart.x + bodyStart.y;
        transform.position = pos;
        
        // initializing body array
        body = new GameObject[(int)map.mapSize.x, (int)map.mapSize.y];
        for (int i = 0; i < body.GetLength(0); i++)
        {
            for (int j = 0; j < body.GetLength(1); j++)
            {
                body[i, j] = null;
            }
        }
        body[bodyStart.x, bodyStart.y] = gameObject;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = spriteUpdater.zeroSided;

        animator = GetComponentInChildren<Animator>();
        animator.enabled = false;
        spriteRenderer.enabled = false;
        
        directionalityFactor = 1 / (Mathf.Pow(2, directionality));

        isSpreading = false;
        if (!tileTrigger)
        {
            StartSpreading();
        }   
    }

    private void Update()
    {
        if (!tileTrigger)
            return;

        if (!isSpreading && playerScript!= null && playerScript.gridCell == triggerIndex)
        {
            StartSpreading();
        }
    }

    private void StartSpreading()
    {
        Debug.Log("TileSpreadingEnemy: starting to spread");
        isSpreading = true;
        StartCoroutine(BeforeSpread());
    }

    private IEnumerator BeforeSpread()
    {
        yield return new WaitForSeconds(timeToStart);
        
        animator.runtimeAnimatorController = spriteUpdater.zeroSidedAnim;
        spriteRenderer.enabled = true;
        animator.enabled = true;
        
        StartCoroutine(SpreadLoop());
    }


    private IEnumerator SpreadLoop()
    {
        yield return StartCoroutine(Spread());
        
        spriteRenderer.sprite = spriteUpdater.UpdateSprite(map, bodyStart);
//        animator.runtimeAnimatorController = spriteUpdater.UpdateAnimator(map, bodyStart);
//        animator.Play(spriteUpdater.SPRITE_NAME);
        
        if (isSpreading)
            StartCoroutine(SpreadLoop());
    }
    

    private IEnumerator Spread()
    {
        var playerIndex = playerScript.gridCell;
        
        // find closest walkable tile
        var perimeterCells = GetPerimeterCellsList(bodyStart, playerIndex);
        var sortedCells = perimeterCells.OrderBy(x => x.distanceToTarget).ToList();

        var chosenList = new List<int>();
        for (int i = 0; i < Mathf.Min(cellsPerSpread, sortedCells.Count); i++)
        {
            // choose random index
            // using function that favors low values
            // https://stackoverflow.com/questions/1589321/adjust-items-chance-to-be-selected-from-a-list
            var r = Random.Range(0f, 1f);
            var chosenIndex = Mathf.FloorToInt(sortedCells.Count * (1 - Mathf.Pow(r, directionalityFactor)));
            while (chosenList.Contains(chosenIndex))
            {
                r = Random.Range(0f, 1f);
                chosenIndex = Mathf.FloorToInt(sortedCells.Count * (1 - Mathf.Pow(r, directionalityFactor)));
            }
            chosenList.Add(chosenIndex);
        }

        foreach (var chosenIndex in chosenList)
        {
            var index = new Vector2Int((int)sortedCells[chosenIndex].index.x, (int)sortedCells[chosenIndex].index.y);
            var tile = map.tiles[map.TileIndex(index.x, index.y)];
            var moveableWall = tile as MoveableWall;
            if (moveableWall != null)
            {
                // create body element in this index
                moveableWall.Infect();
                if (moveableWall.infected)
                    CreateNewBodyElement(index);
            }
        }
        
        yield return secondsToNextSpread;
    }


    private void CreateNewBodyElement(Vector2Int index)
    {
        var newElement = Instantiate(bodyElementPrefab, bodyElementsParent);
        newElement.SetActive(false);
        
        newElement.name = "be_" + index.x + "_" + index.y;
        Vector3 pos = IsoVectors.IsoToWorld(index, map.actualTileSize);
        pos.z = index.x + index.y - 0.1f;
        newElement.transform.position = pos;

        var elementScript = newElement.GetComponent<TileSpreadingEnemyElement>();
        elementScript.index = index;
        elementScript.map = map;
        
        newElement.SetActive(true);
        
        body[index.x, index.y] = newElement;
        bodySize++;
    }
    
    
    struct PerimeterCell
    {
        public Vector2 index;
        public float distanceToTarget;
    }
    
    private List<PerimeterCell> GetPerimeterCellsList(Vector2 start, Vector2 end)
    {
        var grid = (byte[,]) map.pCutGrassRefGrid.Clone();
        
        var result = new HashSet<PerimeterCell>();
        FloodFill2(ref grid, start, end, ref result);

        return result.ToList();
    }
    
    private void FloodFill2(ref byte[,] grid, Vector2 start, Vector2 end, ref HashSet<PerimeterCell> result)
    {
        if (!map.IsValidIndex(start))
            return;
        
        if (grid[(int) start.x, (int) start.y] == map.GRID_BLOCKED)
            return;

        var tileScript = map.tiles[map.TileIndex((int)start.x, (int)start.y)];
        
        var moveableWall = tileScript as MoveableWall;
        if (moveableWall == null)
            return;

//        if (moveableWall.type == TileMap.TileType.moveableWall)
//            return;

        if (!moveableWall.visited)
            return;

        if (moveableWall.infected)
        {
            // "color" gird position
            grid[(int) start.x, (int) start.y] = map.GRID_BLOCKED;
            
            FloodFill2(ref grid, start + Vector2.up, end, ref result);
            FloodFill2(ref grid, start + Vector2.down, end, ref result);
            FloodFill2(ref grid, start + Vector2.left, end, ref result);
            FloodFill2(ref grid, start + Vector2.right, end, ref result);
            return;
        }
        
        var perCell = new PerimeterCell();
        perCell.index = start;
        perCell.distanceToTarget = (end - start).magnitude;
        result.Add(perCell);
    }

    public bool Contains(Vector2 index)
    {
        return (body[(int) index.x, (int) index.y] != null);
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("TileSpreadingEnemy: Hit Player");
        playerScript.ReduceLives();
    }

    private void OnDrawGizmos()
    {
        if (tileTrigger && map != null)
            IsoVectors.drawPoint(triggerIndex, Color.cyan, map.actualTileSize);
    }
}
