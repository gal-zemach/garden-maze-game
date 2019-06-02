using System.Collections;
using System.Collections.Generic;
using NesScripts.Controls.PathFind;
using UnityEngine;

public class NavigationAgent : Controller
{
    [HideInInspector] public TileMap map;
    
    public Vector2 currentCell, currentPosition;
    public Vector2 destination, currentDestination;
    public bool reachedDestination;
    public int remainingDistance;

    public int lookRadius = 3;
    public int lookRadiusInPixels = 100;
    
    
    private PlayerScript playerScript;
    private bool stopped;
    private float horizontalDirection, verticalDirection;
    public List<Point> path;
    
    private const float PROXIMITY_EPSILON = 0.1f;
    
    void Start()
    {
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        playerScript = GetComponent<PlayerScript>();
        
        horizontalDirection = 0;
        verticalDirection = 0;
        path = new List<Point>();
        
        destination = currentDestination = playerScript.gridCell;
    }
    
    void FixedUpdate()
    {
        if (!playerScript.playerIsMoving)
            Stop();
        else
        {
            if (stopped)
                currentDestination = playerScript.gridCell;
            Resume();
        }
        
        if (stopped)
        {
            horizontalDirection = 0;
            verticalDirection = 0;
            return;
        }
        
        // update position
        currentCell = playerScript.gridCell;
        currentPosition = playerScript.gridPosition;
        
        SetMoveParameters();

        // if reached destination - choose a new one
        if (Mathf.Approximately(horizontalDirection, 0) && Mathf.Approximately(verticalDirection, 0))
        {
            GetNextDestination();
        }
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
        Stop();
        destination = newDest;
        
        var startPos = new Point((int)currentCell.x, (int)currentCell.y);
        var endPos = new Point((int) newDest.x, (int)newDest.y);
        path = Pathfinding.FindPath(map.pGrid, startPos, endPos, Pathfinding.DistanceType.Manhattan);
        remainingDistance = path.Count;
        reachedDestination = (remainingDistance == 0);
        if (path.Count == 0)
        {
            if (Mathf.Approximately((destination - currentCell).magnitude, 1))
            {
                currentDestination = destination;
            }
            else
            {
                var randomCell = WallFollower();
                path.Add(new Point((int)randomCell.x, (int)randomCell.y));
            }
        }
        
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
        if (playerScript != null)
        {
            IsoVectors.drawPoint(currentPosition, Color.green, playerScript.tileSize);
            IsoVectors.drawPoint(currentDestination, Color.yellow, playerScript.tileSize);
            IsoVectors.drawPoint(destination, Color.red, playerScript.tileSize);
        }
    }

    
    
    #region FromOldAIController

    private bool approachingWall;
    private int currentDirectionIndex;

    private Vector2[] direction = 
    {
        Vector2.up, 
        Vector2.right, 
        Vector2.down, 
        Vector2.left, 
    };

    private Vector2[] allDirections = 
    {
        Vector2.up, 
        new Vector2(1, 1), 
        Vector2.right, 
        new Vector2(1, -1),
        Vector2.down, 
        new Vector2(-1, -1),
        Vector2.left, 
        new Vector2(-1, 1),
    };
    
    private Vector2 RandomNeighbor()
    {
        // move to a random neighbor floor tile
        var i = Random.Range(0, 4);
        while (!map.IsWalkable(currentCell + direction[i]))
        {
            i = Random.Range(0, 4);
        }
        return currentCell + direction[i];
    }
    
    private Vector2 WallFollower()
    {
        
        // if not around a wall, go to a random walkable direction
        if (!TouchingWall())
        {
            approachingWall = true;
            return KeepStraight();
        }

        // aligning direction so wall is on the left
        if (approachingWall)
        {
            AlignPlayerDirection();
            approachingWall = false;
        }
        
        // wall follower
        var directionIndices = DirectionsArray();

        var i = 0;
        while (i < 4 && !map.IsWalkable(currentCell + direction[directionIndices[i]]))
        {
            i++;
        }

        if (i == 4)
        {
            return currentCell;
        }

        currentDirectionIndex = directionIndices[i];
        return currentCell + direction[currentDirectionIndex];
    }

    private int[] DirectionsArray()
    {
        int[] directions = new[]
            {
                currentDirectionIndex, // same direction
                Mod(currentDirectionIndex - 1, 4), // left turn
                // same direction used to be here
                
                Mod(currentDirectionIndex + 1, 4), // right turn
                Mod(currentDirectionIndex + 2, 4) // 180 turn
            };
        
        return directions;
    }
    
    private Vector2 KeepStraight()
    {
        return currentCell + direction[currentDirectionIndex];
    }

    private void AlignPlayerDirection()
    {
        var j = 0;
        var currentDir = currentDirectionIndex;

        // finding direction index in allDirections
        for (int k = 0; k < allDirections.Length; k++)
        {
            if (allDirections[k] == direction[currentDirectionIndex])
            {
                currentDir = k;
                break;
            }
        }

        // checking if the player is already oriented correctly
        if (map.IsWalkable(currentCell + allDirections[currentDir]) ||
            map.IsWalkable(currentCell + allDirections[Mod(currentDir - 1, 8)]))
        {
            return;
        }
    
        // checking all directions for the wall location
        while (map.IsWalkable(currentCell + allDirections[Mod(currentDir, 8)]))
        {
            currentDir++;
        }
        currentDir = Mod(currentDir, 8);

        // if wall is diagonal - changing to prev direction (that is not diagonal)
        if (allDirections[currentDir].magnitude > 1)
        {
            currentDir = Mod(currentDir - 1, 8);
        }
        
        // finding direction in the 4 dir array and taking the direction after it
        for (int k = 0; k < direction.Length; k++)
        {
            if (direction[k] == allDirections[currentDir])
            {
                currentDirectionIndex = Mod(k + 1, 4);
                return;
            }
        }
    }
    
    int Mod(int x, int m) 
    {
        var r = x % m;
        return r < 0 ? r + m : r;
    }

    bool TouchingWall()
    {
        var i = 0;
        while (i < 8 && map.IsWalkable(currentCell + allDirections[i]))
            i++;
        return i != 8;
    }

    #endregion  
}
