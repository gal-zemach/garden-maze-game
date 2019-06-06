using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NesScripts.Controls.PathFind;

public class SpreadingEnemy : MonoBehaviour
{
    public GameObject bodyElementPrefab;
    public float timeToSpread;
    public bool isSpreading = true;
        
    private GameObject[,] body;
    private Transform bodyElementsParent;
    public Vector2Int bodyStart;
    private WaitForSeconds timeToNextSpread;
    
    private GameManager gameManager;
    private TileMap map;
    private PlayerScript playerScript;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        playerScript = gameManager.player.GetComponent<PlayerScript>();

        bodyElementsParent = transform.Find("Body Elements Parent");
        timeToNextSpread = new WaitForSeconds(timeToSpread);

        // align bodyStart & Position
        var pos = transform.position;
        var index = IsoVectors.WorldToIsoRounded(pos, map.actualTileSize);
        bodyStart = new Vector2Int((int)index.x, (int)index.y);
        pos = IsoVectors.IsoToWorld(index, map.actualTileSize);
        pos.z = index.x + index.y;
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

        StartCoroutine(SpreadLoop());
    }


    private IEnumerator SpreadLoop()
    {
        yield return StartCoroutine(Spread());

        if (isSpreading)
            StartCoroutine(SpreadLoop());
    }
    

    private IEnumerator Spread()
    {
        var playerIndex = playerScript.gridCell;
        
        // find closest walkable tile
        var closestReachableTile = FindClosestWalkableTile(bodyStart, playerIndex);
        
        // find path to it
        var startPos = new Point(bodyStart.x, bodyStart.y);
        var endPos = new Point((int)closestReachableTile.x, (int)closestReachableTile.y);
        var path = Pathfinding.FindPath(map.pCutGrassGrid, startPos, endPos, Pathfinding.DistanceType.Manhattan, false, false);
        
        // get first index in the path that is not already filled with body
        int i = 0;
        for (i = 0; i < path.Count; i++)
        {
            if (body[path[i].x, path[i].y] == null)
            {
                // create body element in this index
                var index = new Vector2Int(path[i].x, path[i].y);
                CreateNewBodyElement(index);
                break;
            }
        }
        // didn't find place to expand on way to player
        if (i == path.Count)
        {
            
        }

        yield return timeToNextSpread;
    }


    private void CreateNewBodyElement(Vector2Int index)
    {
        var newElement = Instantiate(bodyElementPrefab, bodyElementsParent);
        newElement.name = "be_" + index.x + "_" + index.y;
        newElement.transform.position = IsoVectors.IsoToWorld(index, map.actualTileSize);
        body[index.x, index.y] = newElement;
    }
    
    
    private Vector2 FindClosestWalkableTile(Vector2 start, Vector2 end)
    {
        var grid = (byte[,]) map.pCutGrassRefGrid.Clone();
        return FloodFill(ref grid, start, end);
    }

    private Vector2 FloodFill(ref byte[,] grid, Vector2 start, Vector2 end)
    {
        if (!map.IsValidIndex(start))
            return Vector2.positiveInfinity;
        
        if (grid[(int) start.x, (int) start.y] == map.GRID_BLOCKED)
            return Vector2.positiveInfinity;

        var tileScript = map.tiles[map.TileIndex((int)start.x, (int)start.y)];
        
        var moveableWall = tileScript as MoveableWall;
        if (moveableWall == null)
            return Vector2.positiveInfinity;

        if (moveableWall.type == TileMap.TileType.moveableWall)
            return Vector2.positiveInfinity;;

        if (!moveableWall.visited)
            return Vector2.positiveInfinity;
        
        // "color" gird position
        grid[(int) start.x, (int) start.y] = map.GRID_BLOCKED;
        
        // run on children & calculate distances
        var resultingIndices = new Vector2[5];
        resultingIndices[0] = start;
        resultingIndices[1] = FloodFill(ref grid, start + Vector2.up, end);
        resultingIndices[2] = FloodFill(ref grid, start + Vector2.down, end);
        resultingIndices[3] = FloodFill(ref grid, start + Vector2.left, end);
        resultingIndices[4] = FloodFill(ref grid, start + Vector2.right, end);

        var resultingDistances = new float[5];
        for (int i = 0; i < resultingIndices.Length; i++)
        {
            resultingDistances[i] = (end - resultingIndices[i]).magnitude;
        }
        
        // find minimal distance
        float minDistance = float.PositiveInfinity;
        int minDistanceChildIndex = 0;
        for (int i = 0; i < resultingIndices.Length; i++)
        {
            if (resultingDistances[i] < minDistance)
            {
                minDistance = resultingDistances[i];
                minDistanceChildIndex = i;
            }
        }
        
        return resultingIndices[minDistanceChildIndex];
    }
}
