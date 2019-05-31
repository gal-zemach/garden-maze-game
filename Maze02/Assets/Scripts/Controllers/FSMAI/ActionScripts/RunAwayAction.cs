using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Run Away")]
public class RunAwayAction : Action
{
    public override void Act(StateController controller)
    {
        RunAway(controller);
    }

    private void RunAway(StateController controller)
    {
        controller.navAgent.UpdateDestination(controller.runAwayPoint.position);
        
//        var playerPos = controller.navAgent.currentCell;
//        var targetPos = controller.targetObject;
//        
//        var largestAnglePoint = controller.wayPointList[0];
//        var largestAnglePointGridPosition = IsoVectors.WorldToIso(largestAnglePoint.position, controller.navAgent.map.actualTileSize);
//        float largestAngle = Vector3.Angle(targetPos - playerPos, largestAnglePointGridPosition - playerPos);
//        
//        for (int i = 1; i < controller.wayPointList.Count; i++)
//        {
//            var pointGridPosition = IsoVectors.WorldToIso(controller.wayPointList[i].position, controller.navAgent.map.actualTileSize);
//            var angle = Vector3.Angle(targetPos - playerPos, pointGridPosition - playerPos);
//            if (angle > largestAngle)
//            {
//                largestAngle = angle;
//                largestAnglePoint = controller.wayPointList[i];
//            }
//        }
//        controller.navAgent.UpdateDestination(largestAnglePoint.position);
    }
}
