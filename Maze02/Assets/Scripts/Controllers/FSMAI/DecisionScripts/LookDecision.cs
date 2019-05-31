using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LookDecision : Decision
{
    
    protected bool LookFor(StateController controller, string targetLayerName)
    {
        // raycast to 4 sides on the given layer
        int wallLayer = LayerMask.NameToLayer("Walls");
        int targetLayer = LayerMask.NameToLayer(targetLayerName);
        LayerMask combinedLayerMask = ((1 << targetLayer) | (1 << wallLayer));
        
        var playerPos = controller.transform.position;
        var lookRadius = controller.navAgent.lookRadiusInPixels;

        var circleRadius = 5;
        var castOffset = 50;
        var castPos = playerPos + IsoVectors.LEFT * castOffset;
        var targetVisibleLeft = Physics2D.CircleCast(castPos, circleRadius, IsoVectors.LEFT, lookRadius, combinedLayerMask);
        
        castPos = playerPos + IsoVectors.RIGHT * castOffset;
        var targetVisibleRight = Physics2D.CircleCast(castPos, circleRadius, IsoVectors.RIGHT, lookRadius, combinedLayerMask);
        
        castPos = playerPos + IsoVectors.UP * castOffset;
        var targetVisibleUp = Physics2D.CircleCast(castPos, circleRadius, IsoVectors.UP, lookRadius, combinedLayerMask);
        
        castPos = playerPos + IsoVectors.DOWN * castOffset;
        var targetVisibleDown = Physics2D.CircleCast(castPos, circleRadius, IsoVectors.DOWN, lookRadius, combinedLayerMask);
        
        
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
}
