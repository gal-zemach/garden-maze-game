using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return GoToMouseClick(controller);
    }

    private bool GoToMouseClick(StateController controller)
    {
        if (Input.GetMouseButtonDown(1))
        {
            
        }
        
//        tileIndex = ;
//        controller.SetTargetObject(tileIndex);
        
        return false;
    }
}
