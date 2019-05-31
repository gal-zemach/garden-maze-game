using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Look For Tools")]
public class LookForToolsDecision : LookDecision
{
    public override bool Decide(StateController controller)
    {
        return LookFor(controller, "Tools");
    }
}
