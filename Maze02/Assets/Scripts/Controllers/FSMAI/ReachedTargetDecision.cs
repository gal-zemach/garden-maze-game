using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Reached Target")]
public class ReachedTargetDecision : Decision
{
    public override int Decide(StateController controller)
    {
        if (controller.navAgent.reachedDestination)
            return 1;
        return 0;
    }
}
