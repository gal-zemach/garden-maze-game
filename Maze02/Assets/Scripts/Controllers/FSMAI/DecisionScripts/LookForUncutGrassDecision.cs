using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Look For Uncut Grass")]
public class LookForUncutGrassDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        var target = LookForUncutGrass(controller);
        return target;
    }

    private bool LookForUncutGrass(StateController controller)
    {
        var currentCell = controller.navAgent.currentCell;
        bool foundWall;
        
        int dy = 0;
        while (dy < controller.navAgent.lookRadius)
        {
            if (isUncutGrass(controller, new Vector2Int((int) currentCell.x, (int) currentCell.y + dy), out foundWall))
            {
                return true;
            }
            if (foundWall)
            {
                break;
            }
            dy++;
        }
        dy = 0;
        while (dy < controller.navAgent.lookRadius)
        {
            if (isUncutGrass(controller, new Vector2Int((int) currentCell.x, (int) currentCell.y - dy), out foundWall))
            {
                return true;
            }
            if (foundWall)
            {
                break;
            }
            dy++;
        }
        
        int dx = 0;
        while (dx < controller.navAgent.lookRadius)
        {
            if (isUncutGrass(controller, new Vector2Int((int) currentCell.x + dx, (int) currentCell.y), out foundWall))
            {
                return true;
            }
            if (foundWall)
            {
                break;
            }
            dx++;
        }

        dx = 0;
        while (dx < controller.navAgent.lookRadius)
        {
            if (isUncutGrass(controller, new Vector2Int((int) currentCell.x - dx, (int) currentCell.y), out foundWall))
            {
                return true;
            }
            if (foundWall)
            {
                break;
            }
            dx++;
        }
        
        return false;
    }

    private bool isUncutGrass(StateController controller, Vector2Int index, out bool foundWall)
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
        if (moveableWall == null)
            return false;

        if (moveableWall.type == TileMap.TileType.moveableWall)
        {
            foundWall = true;
            return false;
        }

        if (!moveableWall.visited)
        {
            controller.SetTargetObject(moveableWall.index);
            return true;
        }

        return false;
    }
}
