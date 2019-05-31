using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Look For Traps")]
public class LookForTrapsDecision : LookDecision
{
    public override bool Decide(StateController controller)
    {
        var trapFound = LookFor(controller, "Traps");
//        if (trapFound)
//            Debug.Log("trap found");
        
        return trapFound;
    }
}
