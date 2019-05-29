using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Look")]
public class LookDecision : Decision
{
    public override int Decide(StateController controller)
    {
        var target = Look(controller);
        return target;
    }

    private int Look(StateController controller)
    {
//        if (LookFor(controller, "Trap"))
//            return 1;
//        if (LookFor(controller, "Tool"))
//            return 2;
        if (LookForUncutGrass(controller))
            return 3;
        
        return 0;
    }

    private bool LookFor(StateController controller, string targetLayerName)
    {
        // raycast to 4 sides on the given layer
        var targetLayer = LayerMask.NameToLayer(targetLayerName);
        var playerPos = controller.transform.position;
        var lookRadius = controller.navAgent.lookRadiusInPixels;

        var targetVisibleUp = Physics2D.Raycast(playerPos, IsoVectors.UP, lookRadius, targetLayer);
        var targetVisibleDown = Physics2D.Raycast(playerPos, IsoVectors.DOWN, lookRadius, targetLayer);
        var targetVisibleLeft = Physics2D.Raycast(playerPos, IsoVectors.LEFT, lookRadius, targetLayer);
        var targetVisibleRight = Physics2D.Raycast(playerPos, IsoVectors.RIGHT, lookRadius, targetLayer);
        
        if (targetVisibleUp)
        {
            controller.targetObject = targetVisibleUp.collider.transform;
            return true;
        }
        if (targetVisibleDown)
        {
            controller.targetObject = targetVisibleDown.collider.transform;
            return true;
        }
        if (targetVisibleLeft)
        {
            controller.targetObject = targetVisibleLeft.collider.transform;
            return true;
        }
        if (targetVisibleRight)
        {
            controller.targetObject = targetVisibleRight.collider.transform;
            return true;
        }

        return false;
    }

    private bool LookForUncutGrass(StateController controller)
    {
        var currentCell = controller.navAgent.currentCell;

        int dx = 1;
        while (dx < controller.navAgent.lookRadius)
        {
            if (isUncutGrass(controller, new Vector2Int((int) currentCell.x + dx, (int) currentCell.y)) || 
                isUncutGrass(controller, new Vector2Int((int) currentCell.x - dx, (int) currentCell.y)))
            {
                return true;
            }
            dx++;
        }
        
        int dy = 1;
        while (dy < controller.navAgent.lookRadius)
        {
            if (isUncutGrass(controller, new Vector2Int((int) currentCell.x, (int) currentCell.y + dy)) || 
                isUncutGrass(controller, new Vector2Int((int) currentCell.x, (int) currentCell.y + dy)))
            {
                return true;
            }
            dy++;
        }

        return false;
    }

    private bool isUncutGrass(StateController controller, Vector2Int index)
    {
        if (index.x < 0 || index.x >= controller.navAgent.map.mapSize.x ||
            index.y < 0 || index.y >= controller.navAgent.map.mapSize.y)
            return false;
        
        var tileIndex = controller.navAgent.map.TileIndex(index.x, index.y);
        var tile = controller.navAgent.map.tiles[tileIndex];
        var moveableWall = tile as MoveableWall;
        if (moveableWall == null || moveableWall.type == TileMap.TileType.moveableWall)
            return false;

        if (!moveableWall.visited)
        {
            controller.targetObject = moveableWall.transform;
            return true;
        }

        return false;
    }
}
