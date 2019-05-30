using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/Check Tools")]
public class CheckToolsDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return CheckTools(controller);
    }
    
    private bool CheckTools(StateController controller)
    {
        var index = controller.targetObject;
        var tileIndex = controller.navAgent.map.TileIndex((int)index.x, (int)index.y);
        var tile = controller.navAgent.map.tiles[tileIndex];
        if (tile.type != TileMap.TileType.trap)
            return false;

        var enemy = tile.gameObject.GetComponentInChildren<EnemyScript>();
        
//        Debug.Log("Player has " + enemy.defeatingItem + "? " + (controller.playerScript.HasItem(enemy.defeatingItem)));
        return (controller.playerScript.HasItem(enemy.defeatingItem));
    }
}
