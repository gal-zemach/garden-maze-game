using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadDirectionAttackingEnemy : EnemyScript
{
    public int turnsToAttack = 150;

    private IsoCollider isoCollider2;
    private int turnCount;
    private bool attacking;
    
    void Start()
    {
        EnemyBaseStart();
        isoCollider.colliderSize = new Vector2(1, 1);
        
        isoCollider2 = gameObject.AddComponent<IsoCollider>();
        isoCollider2.tileSize = 35;
        isoCollider2.colliderSize = new Vector2(3, 1);
        isoCollider2.colliderCenter = new Vector2(1, 0);
        isoCollider2.RotateCW();
        isoCollider2.enabled = false;

//        gameObject.AddComponent<CompositeCollider2D>();
        
        turnCount = 0;
        attacking = false;
    }

    void FixedUpdate()
    {
        turnCount++;
        if (turnCount == turnsToAttack)
        {
            turnCount = 0;
            ToggleAttack();
        }
    }

    private void ToggleAttack()
    {
        if (attacking)
        {
            attacking = false;
            isoCollider2.enabled = false;
            
            isoCollider.colliderSize = new Vector2(1, 1);
            isoCollider.colliderCenter = new Vector2(0, 0);
        }
        else
        {
            attacking = true;
            isoCollider2.enabled = true;
            
            isoCollider.colliderSize = new Vector2(3, 1);
            isoCollider.colliderCenter = new Vector2(1, 0);
        }
    }
}
