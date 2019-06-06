using System.Collections;
using System.Collections.Generic;
using NesScripts.Controls.PathFind;
using UnityEngine;

public class EnemyNavigationAgent : Controller
{
        [HideInInspector] public TileMap map;
    
    public Vector2 currentCell, currentPosition;
    public Vector2 oldDestination, destination, currentDestination;
    public bool reachedDestination;
    public int remainingDistance;

    private GameManager gameManager;
    private PlayerScript playerScript;
    
    private ChasingEnemy chasingEnemyScript;
    private bool stopped;
    private float horizontalDirection, verticalDirection;
    public List<Point> path;
    
    private const float PROXIMITY_EPSILON = 0.1f;
    
    void Start()
    {
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        chasingEnemyScript = GetComponent<ChasingEnemy>();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerScript = gameManager.player.GetComponent<PlayerScript>();
        
        horizontalDirection = 0;
        verticalDirection = 0;
        path = new List<Point>();
        
        destination = currentDestination = chasingEnemyScript.gridCell;
    }
    
    void FixedUpdate()
    {
        if (!chasingEnemyScript.playerIsMoving)
            Stop();
        else
        {
            if (stopped)
                currentDestination = chasingEnemyScript.gridCell;
            Resume();
        }
        
        if (stopped)
        {
            horizontalDirection = 0;
            verticalDirection = 0;
            return;
        }
        
        // update position
        currentCell = chasingEnemyScript.gridCell;
        currentPosition = chasingEnemyScript.gridPosition;

        var playerCell = playerScript.gridCell;
        if (playerCell != oldDestination)
        {
            UpdateDestination(playerCell);
            oldDestination = playerCell;
        }
        
        SetMoveParameters();
        
        ResetIfStuck();
        
        // if reached destination - choose a new one
        if (Mathf.Approximately(horizontalDirection, 0) && Mathf.Approximately(verticalDirection, 0))
        {
            GetNextDestination();
        }
        
//        ResetIfStuck();
    }
    
    public override float HorizontalAxis()
    {
        return horizontalDirection;
    }

    public override float VerticalAxis()
    {
        return verticalDirection;
    }
    
    public void Resume()
    {
        stopped = false;
    }

    public void Stop()
    {
        stopped = true;
    }

    public void UpdateDestination(Vector2 newDest)
    {
        if (newDest.x < 0 || newDest.y < 0)
            return;
        
        Stop();
        var endIndex = FindClosestWalkableTile(currentCell, newDest);
        destination = endIndex;
        
        var startPos = new Point((int)currentCell.x, (int)currentCell.y);
        var endPos = new Point((int)endIndex.x, (int)endIndex.y);
        path = Pathfinding.FindPath(map.pCutGrassGrid, startPos, endPos, Pathfinding.DistanceType.Manhattan, false, false);
        
        remainingDistance = path.Count;
        reachedDestination = (remainingDistance == 0);
        
        Resume();
    }

    public void UpdateDestination(Vector3 destPosition)
    {
        var newDest = IsoVectors.WorldToIsoRounded(destPosition, map.actualTileSize);
        UpdateDestination(newDest);
    }

    private void GetNextDestination()
    {
        if (path.Count > 0)
        {
            reachedDestination = false;
            var p = path[0];
            currentDestination = new Vector2(p.x, p.y);
            path.RemoveAt(0);
            remainingDistance--;
            return;
        }
        
        reachedDestination = true;
    }

    private void SetMoveParameters()
    {
        var offset = Vector2.zero;
        var delta = currentPosition - (currentDestination + offset);
        if (Mathf.Abs(delta.y) > PROXIMITY_EPSILON)
        {
            horizontalDirection = Mathf.Sign(delta.y);
        }
        else
        {
            horizontalDirection = 0;
            
            if (Mathf.Abs(delta.x) > PROXIMITY_EPSILON)
            {
                verticalDirection = -Mathf.Sign(delta.x);
            }
            else
            {
                verticalDirection = 0;
            }
        }
    }
        
    private void OnDrawGizmos()
    {
        if (chasingEnemyScript != null)
        {
            IsoVectors.drawPoint(currentPosition, Color.green + Color.gray, chasingEnemyScript.tileSize);
            IsoVectors.drawPoint(currentDestination, Color.yellow + Color.gray, chasingEnemyScript.tileSize);
            IsoVectors.drawPoint(destination, Color.red + Color.gray, chasingEnemyScript.tileSize);
        }
    }


    private float stuckDistance = 0.2f;
    private float lastCheckTime = 0f;
    private Vector2 lastCheckCell;
    private float stuckCheckTime = 1f;
    
    private void ResetIfStuck()
    {
        if (Time.time - lastCheckTime > stuckCheckTime)
        {
            if ((currentPosition - lastCheckCell).magnitude < stuckDistance)
            {
                chasingEnemyScript.playerIsMoving = false;
                transform.position = IsoVectors.IsoToWorld(currentCell, map.actualTileSize);
                
                Debug.Log("EnemyNavigationAgent: stuck for too long");
                currentDestination = currentCell;
                UpdateDestination(destination);

                chasingEnemyScript.playerIsMoving = true;
            }

            lastCheckCell = currentPosition;
            lastCheckTime = Time.time;
        }
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
