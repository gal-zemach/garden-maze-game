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
        if (LookFor(controller, "Traps"))
        {
            Debug.Log("saw trap at " + controller.targetObject);
            return 1;
        }
        if (LookFor(controller, "Items"))
        {
            Debug.Log("saw item at " + controller.targetObject);
            return 2;
        }
        if (LookForUncutGrass(controller))
            return 3;
        
        return 0;
    }

    private bool LookFor(StateController controller, string targetLayerName)
    {
        // raycast to 4 sides on the given layer
        int wallLayer = LayerMask.NameToLayer("Walls");
        int targetLayer = LayerMask.NameToLayer(targetLayerName);
        LayerMask combinedLayerMask = ((1 << targetLayer) | (1 << wallLayer));
        
        var playerPos = controller.transform.position;
        var lookRadius = controller.navAgent.lookRadiusInPixels;

        var targetVisibleLeft = Physics2D.Raycast(playerPos, IsoVectors.LEFT, lookRadius, combinedLayerMask);
        var targetVisibleRight = Physics2D.Raycast(playerPos, IsoVectors.RIGHT, lookRadius, combinedLayerMask);
        var targetVisibleUp = Physics2D.Raycast(playerPos, IsoVectors.UP, lookRadius, combinedLayerMask);
        var targetVisibleDown = Physics2D.Raycast(playerPos, IsoVectors.DOWN, lookRadius, combinedLayerMask);
        
        if (targetVisibleLeft && targetVisibleLeft.collider.gameObject.layer == targetLayer)
        {
//            Debug.Log(targetVisibleLeft.collider.gameObject.transform.parent.name);
            controller.SetTargetObject(targetVisibleLeft.collider.transform.position);
            return true;
        }
        if (targetVisibleRight && targetVisibleRight.collider.gameObject.layer == targetLayer)
        {
//            Debug.Log(targetVisibleRight.collider.gameObject.transform.parent.name);
            controller.SetTargetObject(targetVisibleRight.collider.transform.position);
            return true;
        }
        if (targetVisibleUp && targetVisibleUp.collider.gameObject.layer == targetLayer)
        {
//            Debug.Log(targetVisibleUp.collider.gameObject.transform.parent.name);
            controller.SetTargetObject(targetVisibleUp.collider.transform.position);
            return true;
        }
        if (targetVisibleDown && targetVisibleDown.collider.gameObject.layer == targetLayer)
        {
//            Debug.Log(targetVisibleDown.collider.gameObject.transform.parent.name);
            controller.SetTargetObject(targetVisibleDown.collider.transform.position);
            return true;
        }

        return false;
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
