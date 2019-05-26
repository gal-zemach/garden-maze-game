using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Random = UnityEngine.Random;

public class ShootingEnemy : EnemyScript
{
    public enum Direction {Up, Down, Left, Right};

    public Sprite UpLeftSprite, DownRightSprite;
    
    public GameObject projectilePrefab;
    public int turnsToAttack = 150;
    public Direction direction;
    public Transform bulletSpawn;

    private SpriteRenderer spriteRenderer;
    private Transform bulletsParent;
    private int turnCount;
    private BulletScript bullet;
    private Vector2 shootingDirection;
    
    void Start()
    {
        EnemyBaseStart();
        isoCollider.colliderSize = new Vector2(1, 1);
        bulletsParent = transform.Find("Bullets Parent");

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        turnCount = Random.Range(0, 30);

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
        UpdateSprite();
        
        turnCount++;
        if (turnCount >= turnsToAttack)
        {
            turnCount = 0;
            Fire();
        }
    }

    private void Fire()
    {
        bullet = Instantiate(projectilePrefab, bulletsParent).GetComponent<BulletScript>();
        bullet.transform.position = bulletSpawn.position;
        bullet.shooter = gameObject;
        bullet.Shoot(shootingDirection);
            
    }

    private void UpdateSprite()
    {
        switch (direction)
        {
            case Direction.Up:
                spriteRenderer.sprite = UpLeftSprite;
                spriteRenderer.flipX = true;
                break;
            
            case Direction.Down:
                spriteRenderer.sprite = DownRightSprite;
                spriteRenderer.flipX = false;
                break;
            
            case Direction.Left:
                spriteRenderer.sprite = UpLeftSprite;
                spriteRenderer.flipX = false;
                break;
            
            case Direction.Right:
                spriteRenderer.sprite = DownRightSprite;
                spriteRenderer.flipX = true;
                break;
            
        }
    }
}
