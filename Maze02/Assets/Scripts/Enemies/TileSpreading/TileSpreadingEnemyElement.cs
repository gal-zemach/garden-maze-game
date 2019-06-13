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
    private Transform spriteTransform;
    
    
    void Start()
    {
        parentEnemy = transform.parent.parent.GetComponent<TileSpreadingEnemy>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteTransform = transform.Find("Sprite");
    }

    private void LateUpdate()
    {
        spriteRenderer.sprite = tileUpdater.UpdateSprite(map, index);
    }


//    private float animStep = .05f;
//        
//    private IEnumerator Animate()
//    {
//        var scale = new Vector3(0, 1, 1);
//        while (scale.x <= 1)
//        {
//            spriteTransform.localScale = scale;
//            scale += new Vector3(animStep, 0, 0);
//            yield return new WaitForSeconds(.02f);
//        }
//        scale = Vector3.one;
//        spriteTransform.localScale = scale;
//    }
}
