using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    protected Vector2 gridCell, tileSize;
    protected TileMap map;
    protected IsoCollider isoCollider;
    protected GameObject colliderChild;

    protected void EnemyBaseStart()
    {
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        tileSize = new Vector2(map.tileSize.x, map.tileSize.y / 2);
        
        colliderChild = new GameObject("collider");
        colliderChild.transform.SetParent(gameObject.transform);
        colliderChild.transform.localPosition = Vector3.zero;
        colliderChild.layer = gameObject.layer;
        
        isoCollider = colliderChild.AddComponent<IsoCollider>();
        isoCollider.tileSize = 35; // 64 makes for larger than map tiles' tiles, this is smaller than the map tiles
        
        // todo: align z position
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("EnemyScript: hit player");
            var playerScript = other.gameObject.GetComponentInParent<PlayerScript>();
//            playerScript.ReduceHealth();
        }
    }
}
