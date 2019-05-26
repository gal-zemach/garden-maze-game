using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIController : Controller
{
    private byte[] grid;
    private const byte CAN_WALK = 1;
    private const byte CANT_WALK = 1;

    private PlayerScript playerScript;
    private TileMap map;
    private Vector2 mapSize;
    
    public int visionRadius = 2;

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
    
    private const float proximityEpsilon = 0.1f;
    private const float stuckProximityEpsilon = 0.05f;
    public Vector2 currentCell, currentPosition, currentDestination, oldPosition;
    private int currentDirectionIndex;

    private float lastCheckTime = 0f;
    private Vector2 lastCheckCell;
    private float stuckCheckTime = 2f;

    public float horizontalDirection, verticalDirection;

    private Vector2 EMPTY_CELL = new Vector2(-1, -1);
    
    void Start()
    {
        playerScript = GetComponent<PlayerScript>();
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        mapSize = map.mapSize;
        currentDestination = EMPTY_CELL;
        horizontalDirection = 0;
        verticalDirection = 0;
        currentDirectionIndex = 0;
        
        grid = new byte[(int)(mapSize.x * mapSize.y)];
        for (int i = 0; i < mapSize.x * mapSize.y; i++)
        {
            grid[i] = CANT_WALK;
        }
    }

    void FixedUpdate()
    {
        if (!playerScript.movementStarted)
            return;
        
        // update map memory
        currentCell = playerScript.gridCell;
        currentPosition = playerScript.gridPosition;
        
        if (currentDestination == EMPTY_CELL)
        {
            currentDestination = currentCell;
        }
        
        // updating AI memory
//        updateSurroundings();

        // go to currentDestination
        var offset = Vector2.zero;
        var delta = Vector2.zero;
        delta = currentPosition - (currentDestination + offset);
        if (Mathf.Abs(delta.y) > proximityEpsilon)
        {
            horizontalDirection = Mathf.Sign(delta.y);
        }
        else
        {
            horizontalDirection = 0;
            
            if (Mathf.Abs(delta.x) > proximityEpsilon)
            {
                verticalDirection = -Mathf.Sign(delta.x);
            }
            else
            {
                verticalDirection = 0;
            }
        }
        
        // check if the player is stuck
        if (Time.time - lastCheckTime > stuckCheckTime)
        {
            if (currentCell == lastCheckCell)
            {
                Debug.Log("AIController: stuck for too long");
                horizontalDirection = 0;
                verticalDirection = 0;
            }

            lastCheckCell = currentCell;
            lastCheckTime = Time.time;
        }
        
        // if reached destination - choose a new one
        if (Mathf.Approximately(horizontalDirection, 0) && 
            Mathf.Approximately(verticalDirection, 0))
        {
            currentDestination = WallFollower();
//            Debug.Log("AIController: currentDestination = " + currentDestination);
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

    private Vector2 KeepStraight()
    {
        return currentCell + direction[currentDirectionIndex];
    }
    
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

    private bool approachingWall = false;
    
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

    int NearestWallDirection()
    {
        var i = 0;
        var mult = 1;
        var closestBorder = Mathf.Min(map.mapSize.x, map.mapSize.y);
        while (mult < closestBorder && map.IsWalkable(currentCell + direction[i] * mult))
        {
            i++;
            if (i == 4)
            {
                mult++;
                i = 0;
            }
        }

        return i;
    }


    private void updateSurroundings()
    {
        for (int column = (int) (currentCell.x - visionRadius); column <= currentCell.x + visionRadius; column++)
        {
            for (int row = (int) (currentCell.y - visionRadius); row <= currentCell.y + visionRadius; row++)
            {
                if (!map.IsValidIndex(column, row))
                    continue;
                
                var tileIndex = map.TileIndex(column, row);
                Debug.Log(column + ", " + row + " == " + tileIndex);
                
                if (map.GetTileType(column, row) == TileMap.TileType.Floor)
                {
                    grid[tileIndex] = CAN_WALK;
                }
                else
                {
                    grid[tileIndex] = CANT_WALK;
                }
            }
        }
    }
    

    private void OnDrawGizmos()
    {
        if (playerScript != null)
        {
            IsoVectors.drawPoint(currentPosition, Color.yellow, playerScript.tileSize);
            IsoVectors.drawPoint(currentDestination, Color.green, playerScript.tileSize);
        }
    }
}

// old stuck check
//
//
//    private int maxTurnsStuck = 200;
//    private int turnsStuck;
//
//
//        if (currentPosition.x - oldPosition.x < stuckProximityEpsilon &&
//            currentPosition.y - oldPosition.y < stuckProximityEpsilon)
//        {
//            turnsStuck++;
//            if (turnsStuck == maxTurnsStuck)
//            {
//                Debug.Log("AIController: stuck for too long");
//                horizontalDirection = 0;
//                verticalDirection = 0;
//                turnsStuck = 0;
//            }
//        }
//        else
//        {
//            turnsStuck = 0;
//        }
//        oldPosition = currentPosition;