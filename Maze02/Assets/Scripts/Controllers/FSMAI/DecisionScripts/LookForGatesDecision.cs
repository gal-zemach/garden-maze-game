using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Look For Gates")]
public class LookForGatesDecision : Decision
{

    public bool seeThroughWalls;
    
    public override bool Decide(StateController controller)
    {
        var target = LookForGates(controller);
        return target;
    }

    private bool LookForGates(StateController controller)
    {
        var currentCell = controller.navAgent.currentCell;
        bool foundWall = false;
        bool checkLeft = true, checkRight = true, checkUp = true, checkDown = true;
        
        int d = 0;
        while (d < controller.navAgent.lookRadius)
        {
            if (checkLeft && 
                isOpenGate(controller, new Vector2Int((int) currentCell.x, (int) currentCell.y + d), out foundWall))
            {
                return true;
            }
            if (foundWall)
            {
                checkLeft = false;
            }
            
            if (checkRight && 
                isOpenGate(controller, new Vector2Int((int) currentCell.x, (int) currentCell.y - d), out foundWall))
            {
                return true;
            }
            if (foundWall)
            {
                checkRight = false;
            }
            
            if (checkUp &&
                isOpenGate(controller, new Vector2Int((int) currentCell.x + d, (int) currentCell.y), out foundWall))
            {
                return true;
            }
            if (foundWall)
            {
                checkUp = false;
            }
            
            if (checkDown &&
                isOpenGate(controller, new Vector2Int((int) currentCell.x - d, (int) currentCell.y), out foundWall))
            {
                return true;
            }
            if (foundWall)
            {
                checkDown = false;
            }

            d++;
        }
        
        return false;
    }

    private bool isOpenGate(StateController controller, Vector2Int index, out bool foundWall)
    {
        foundWall = false;
        
        if (index.x < 0 || index.x >= controller.navAgent.map.mapSize.x ||
            index.y < 0 || index.y >= controller.navAgent.map.mapSize.y)
        {
            foundWall = true;
            return false;
        }
        
        var tileIndex = controller.navAgent.map.TileIndex(index.x, index.y);
        var tile = controller.navAgent.map.tiles[tileIndex];

        var moveableWall = tile as MoveableWall;
        if (!seeThroughWalls && moveableWall != null)
        {
            if (moveableWall.type == TileMap.TileType.moveableWall)
            {
                foundWall = true;
                return false;
            }
        } 
        
        var gate = tile as Gate;
        if (gate == null)
            return false;

        if (gate.open)
        {
            controller.SetTargetObject(gate.index);
            return true;
        }

        return false;
    }
}
