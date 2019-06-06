using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Go To Player")]
public class GoToPlayerAction : Action
{
    public override void Act(StateController controller)
    {
        GoToPlayer(controller);
    }

    private void GoToPlayer(StateController controller)
    {
        EnemyStateController eController = controller as EnemyStateController;
        if (eController == null)
            return;
        
        eController.targetObject = eController.playerScriptRef.gridCell;
        eController.enemyNavAgent.UpdateDestination(eController.targetObject);
    }
}
