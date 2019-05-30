using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Go To Target")]
public class GoToTargetAction : Action
{
    public override void Act(StateController controller)
    {
        GoToTarget(controller);
    }

    private void GoToTarget(StateController controller)
    {
        controller.navAgent.UpdateDestination(controller.targetObject);
    }
}
