using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpreadingEnemyElement : MonoBehaviour
{
    private TileSpreadingEnemy parentEnemy;
    
    void Start()
    {
        parentEnemy = transform.parent.parent.GetComponent<TileSpreadingEnemy>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (parentEnemy == null)
            return;
        
        parentEnemy.OnCollisionEnter2D(other);
    }
}
