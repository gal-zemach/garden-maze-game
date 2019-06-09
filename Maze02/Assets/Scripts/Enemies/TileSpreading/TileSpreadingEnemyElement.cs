using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpreadingEnemyElement : MonoBehaviour
{
    public MultiSidedTile tileUpdater;
    public Vector2 index;
    public TileMap map;
    
    private TileSpreadingEnemy parentEnemy;
    private SpriteRenderer spriteRenderer;
    
    
    void Start()
    {
        parentEnemy = transform.parent.parent.GetComponent<TileSpreadingEnemy>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        spriteRenderer.sprite = tileUpdater.UpdateSprite(map, index);
    }
}
