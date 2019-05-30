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
}
