using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingAttackingEnemy : EnemyScript
{
    public int turnsToRotation = 150;
    private int turnCount;
    
    void Start()
    {
        EnemyBaseStart();
        isoCollider.colliderSize = new Vector2(1, 2);
        turnCount = 0;
    }

    void FixedUpdate()
    {
        turnCount++;
        if (turnCount == turnsToRotation)
        {
            turnCount = 0;
            isoCollider.RotateCW();
        }
    }
}
