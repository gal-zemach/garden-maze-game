using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Look For Traps")]
public class LookForTrapsDecision : LookDecision
{
    public override bool Decide(StateController controller)
    {
        return LookFor(controller, "Traps");
    }
}
