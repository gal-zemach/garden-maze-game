using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleEnemy : EnemyScript
{
    private Trap parentTrapTile;
    
    void Start()
    {
        EnemyBaseStart();
        isoCollider.colliderSize = new Vector2(1, 1);
        var rb2d = gameObject.AddComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        
        parentTrapTile = transform.parent.GetComponentInParent<Trap>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var playerScript = other.gameObject.GetComponentInParent<PlayerScript>();

            if (playerScript.HasItem(Item.ItemType.Shovel))
            {
                parentTrapTile.changeToFloorTile();                
            }
            else
            {
                Debug.Log("HoleEnemy: player fell to Hole");
                playerScript.ReduceLives();
            }
        }
    }
}
