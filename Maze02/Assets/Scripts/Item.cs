using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        None,
        Shovel,
        Shears
    }

    public ItemType type;

    private TileMap map;

    private void Start()
    {
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        var pos = transform.parent.position;
        var newPos = IsoVectors.WorldToTileCenter(pos, map.actualTileSize);
        pos.z = newPos.z;
        transform.parent.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var playerScript = other.gameObject.GetComponentInParent<PlayerScript>();
            playerScript.AddItem(type);
            transform.parent.gameObject.SetActive(false);
        }
    }
}
