using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class ShootingEnemy : EnemyScript
{
    public enum Direction {Up, Down, Left, Right};
    
    public GameObject projectilePrefab;
    public int turnsToAttack = 150;
    public Direction direction;
    public Transform bulletSpawn;

    private int turnCount;
    private BulletScript bullet;
    private Vector2 shootingDirection;
    
    void Start()
    {
        EnemyBaseStart();
        isoCollider.colliderSize = new Vector2(1, 1);
        
        turnCount = 0;

        switch (direction)
        {
            case Direction.Up:
                shootingDirection = IsoVectors.UP;
                break;

            case Direction.Down:
                shootingDirection = IsoVectors.DOWN;
                break;
            
            case Direction.Left:
                shootingDirection = IsoVectors.LEFT;
                break;
            
            case Direction.Right:
                shootingDirection = IsoVectors.RIGHT;
                break;
        }
    }

    
    void FixedUpdate()
    {
        turnCount++;
        if (turnCount >= turnsToAttack)
        {
            turnCount = 0;
            Fire();
        }
    }

    private void Fire()
    {
        bullet = Instantiate(projectilePrefab, transform).GetComponent<BulletScript>();
        bullet.transform.position = bulletSpawn.position;
        bullet.shooter = gameObject;
        bullet.Shoot(shootingDirection);
            
    }
}
