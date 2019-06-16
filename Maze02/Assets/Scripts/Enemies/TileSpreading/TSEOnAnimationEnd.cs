using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSEOnAnimationEnd : MonoBehaviour
{
    public bool doNothing;
    private TileSpreadingEnemyElement parentScript;
    
    void Start()
    {
        if (doNothing)
            return;
        
        parentScript = GetComponentInParent<TileSpreadingEnemyElement>();
    }

    public void OnAnimationEnd()
    {
        if (doNothing)
            return;
        
        parentScript.OnAnimationEnd();
    }
}
