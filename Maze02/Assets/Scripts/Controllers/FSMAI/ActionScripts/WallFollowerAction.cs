using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/WallFollower")]
public class WallFollowerAction : Action
{    
    public override void Act(StateController controller)
    {
        FollowWall(controller);
    }

    void FollowWall(StateController controller)
    {
        map = controller.navAgent.map;
        currentCell = controller.navAgent.currentCell;
        if (controller.navAgent.reachedDestination)
        {
            var destCell = WallFollower();
            
//            Debug.Log("destination = " + destCell);
            controller.navAgent.UpdateDestination(destCell);
        }
    }
    

// AIController WallFollower code:
 
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
    
    
    private TileMap map;
    private Vector2 currentCell;
    private int currentDirectionIndex;
    private bool approachingWall = false;
    
    
    private void AlignDirection(StateController controller)
    {
        var horizontalDirection = controller.navAgent.HorizontalAxis();
        var verticalDirection = controller.navAgent.VerticalAxis();
        
        if (horizontalDirection > 0)
        {
//                currentDirection = Vector2.right;
            currentDirectionIndex = 1;
        }
        else if (horizontalDirection < 0)
        {
//                currentDirection = Vector2.left;
            currentDirectionIndex = 3;
        }
        else if (verticalDirection > 0)
        {
//                currentDirection = Vector2.up;
            currentDirectionIndex = 0;
        }
        else if (verticalDirection < 0)
        {
//                currentDirection = Vector2.down;
            currentDirectionIndex = 2;
        }
    }
    

    
    private Vector2 WallFollower()
    {
        
        // if not around a wall, go to a random walkable direction
        if (!TouchingWall())
        {
//            Debug.Log("WallFollower: not touching wall");
            approachingWall = true;
            return KeepStraight();
        }

        // aligning direction so wall is on the left
        if (approachingWall)
        {
//            Debug.Log("WallFollower: approaching wall");
            AlignPlayerDirection();
            approachingWall = false;
        }
        
        // wall follower
        var directionIndices = DirectionsArray();

        var i = 0;
        while (i < 4 && !map.IsWalkable(currentCell + direction[directionIndices[i]]))
        {
//            Debug.Log("WallFollowr: direction " + i + " is not walkable. (cell: " + (currentCell + direction[directionIndices[i]]) + ")");
            i++;
        }

        if (i == 4)
        {
//            Debug.Log("WallFollower: all cells are blocked");
            return currentCell;
        }

//        Debug.Log("WallFollower: chose direction");
        currentDirectionIndex = directionIndices[i];
        return currentCell + direction[currentDirectionIndex];
    }

    private int[] DirectionsArray()
    {
        int[] directions = new[]
            {
                // left, same, right, 180                
                currentDirectionIndex, // same direction
                
                Mod(currentDirectionIndex - 1, 4), // left turn
                
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
}
