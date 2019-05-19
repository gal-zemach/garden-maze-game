using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleEnemy : EnemyScript
{
    void Start()
    {
        EnemyBaseStart();
        isoCollider.colliderSize = new Vector2(1, 1);
        var rb2d = gameObject.AddComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("HoleEnemy: player fell to Hole");
            var playerScript = other.gameObject.GetComponentInParent<PlayerScript>();
            playerScript.FellToHole();
        }
    }
}
