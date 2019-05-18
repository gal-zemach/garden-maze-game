using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : EnemyScript
{

    public GameObject projectilePrefab;
    public int turnsToAttack = 150;

    private int turnCount;
    private GameObject projectile;
    
    void Start()
    {
        EnemyBaseStart();
        isoCollider.colliderSize = new Vector2(1, 1);
        
        turnCount = 0;
        projectile = (GameObject) Instantiate(projectilePrefab, transform);
    }

    
    void FixedUpdate()
    {
        turnCount++;
        if (turnCount >= turnsToAttack)
        {
            if (!projectile.activeSelf)
            {
                turnCount = 0;
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        projectile.SetActive(enabled);
    }
}
