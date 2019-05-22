using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoCollider : MonoBehaviour
{
    public float tileSize;
    public Vector2 colliderSize, colliderCenter;
    public int rotation = 0;
    
    public bool rotateCW, rotateCCW;
    
    private BoxCollider2D baseCollider;
    private Vector2 tileDimensions, size, center;
//    private bool aligned;
    
    void Start()
    {
        baseCollider = gameObject.AddComponent<BoxCollider2D>();
        baseCollider.isTrigger = true;
        transform.rotation = Quaternion.Euler(60, 0, 45);
        transform.localScale = new Vector3(tileSize, tileSize);
        
        tileDimensions = new Vector2(tileSize, tileSize / 2);
    }

    void FixedUpdate()
    {
        if (rotateCW)
        {
            RotateCW();
            rotateCW = false;
        }
        if (rotateCCW)
        {
            RotateCCW();
            rotateCCW = false;
        }
        
        size = colliderSize;
        center = colliderCenter;
        if (size.x < 0)
        {
            size.x = -size.x;
            center.x = size.x - center.x - 1;
        }
        if (size.y < 0)
        {
            size.y = -size.y;
            center.y = size.y - center.y - 1;
        }
        
        baseCollider.size = size;
        baseCollider.offset = new Vector2(size.x - 1, size.y -1) * 0.5f - center;
    }

//    void alignPositionToGrid()
//    {
//        var isoPosition = IsoVectors.WorldToIso(transform.position, tileDimensions);
//        isoPosition = new Vector2(Mathf.Round(isoPosition.x), Mathf.Round(isoPosition.y));
//        var newWorldPosition = IsoVectors.IsoToWorld(isoPosition, tileDimensions);
//        Vector3 newWorldPosition3D = newWorldPosition;
//        newWorldPosition3D.z = newWorldPosition.x + newWorldPosition.y;
//        transform.position = newWorldPosition3D;
//    }

    public void RotateCW()
    {
        var temp = colliderSize.x;
        colliderSize.x = colliderSize.y;
        colliderSize.y = -temp;
        
        temp = colliderCenter.x;
        colliderCenter.x = colliderCenter.y;
        colliderCenter.y = temp;
        
        UpdateRotationValue();
    }
    
    public void RotateCCW()
    {
        var temp = colliderSize.x;
        colliderSize.x = -colliderSize.y;
        colliderSize.y = temp;
        
        temp = colliderCenter.x;
        colliderCenter.x = colliderCenter.y;
        colliderCenter.y = temp;
        
        UpdateRotationValue();
    }

    private void UpdateRotationValue()
    {
        if (colliderSize.x >= 0 && colliderSize.y >= 0)
        {
            rotation = 0;
        }
        else if (colliderSize.x >= 0 && colliderSize.y < 0)
        {
            rotation = 90;
        }
        else if (colliderSize.x < 0 && colliderSize.y < 0)
        {
            rotation = 180;
        }
        else
        {
            rotation = 270;
        }
    }
}
