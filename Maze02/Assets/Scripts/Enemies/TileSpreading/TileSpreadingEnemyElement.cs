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
    private Animator animator;

    private bool ranAnimation;
    private int updateCycles;
    
    void Start()
    {
        parentEnemy = transform.parent.parent.GetComponent<TileSpreadingEnemy>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
//        spriteTransform = transform.Find("Sprite");

        animator = GetComponentInChildren<Animator>();
        ranAnimation = false;
//        updateCycles = 0;

        var newAnimator = tileUpdater.UpdateAnimator(map, index);
        animator.runtimeAnimatorController = newAnimator;
    }

    private void LateUpdate()
    {
//        updateCycles++;
//        if (ranAnimation && updateCycles < 60)
//            return;
                
          if (!ranAnimation)
            return;
          
          animator.enabled = false;
          
//        var newAnimator = tileUpdater.UpdateAnimator(map, index);
//        animator.enabled = false;
//        animator.runtimeAnimatorController = newAnimator;
//        if (ranAnimation)
//        {
//            animator.Play(tileUpdater.SPRITE_NAME);
//        }
//
//        ranAnimation = true;
//        animator.enabled = true;

        spriteRenderer.sprite = tileUpdater.UpdateSprite(map, index);
    }

    public void OnAnimationEnd()
    {
//        Debug.Log("animation event called");
        ranAnimation = true;
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
