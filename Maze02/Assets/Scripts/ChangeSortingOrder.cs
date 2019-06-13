using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSortingOrder : MonoBehaviour
{
    public int destinationLayer;
    
    void Start()
    {
        changeLayer(transform);
    }

    private void changeLayer(Transform parent)
    {
        foreach (Transform t in parent)
        {
            changeLayer(t);
        }

        var sr = parent.gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = destinationLayer;
        }
    }
}
