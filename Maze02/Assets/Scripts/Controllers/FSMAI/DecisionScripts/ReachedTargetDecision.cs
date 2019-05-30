using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Reached Target")]
public class ReachedTargetDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return (controller.navAgent.reachedDestination);
    }
}
