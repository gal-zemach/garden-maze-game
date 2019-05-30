using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Timer Decision")]
public class TimerDecision : Decision
{
    public int timeToCount;
    
    public override bool Decide(StateController controller)
    {
        return (controller.ChackIfCountDownElapsed(timeToCount));
    }
}
