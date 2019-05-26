using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector2 startTile, endTile;

    [HideInInspector]
    public GameObject player;

    private PlayerScript playerScript;
    private TileMap map;
    private Vector2 tileSize;

    private bool gameEnded;

    private GameObject camera;

    public int tilesLeft;
    
    void Start()
    {
        camera = GameObject.Find("Main Camera");
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        tileSize = new Vector2(map.tileSize.x, map.tileSize.y / 2);

//        SpawnPlayer();
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerScript>();

        gameEnded = false;
    }

    void Update()
    {
        if (gameEnded)
            return;
        
        var playerPosition = player.transform.position;
        Vector2 playerTile = IsoVectors.WorldToIso(playerPosition, tileSize);
        playerTile = new Vector2(Mathf.Round(playerTile.x), Mathf.Round(playerTile.y));

        if (tilesLeft == 0)
        {
            endGame(true);
        }
        else if (playerScript != null && playerScript.IsDead())
        {
            endGame(false);
        }
    }

    void endGame(bool win=false)
    {
        if (win)
        {
            Debug.Log("GameManager: you Win!");
        }
        else
        {
            Debug.Log("GameManager: you Lose!");
//            SpawnPlayer();
        }
        gameEnded = true;
        Time.timeScale = 0;
    }


//    private void SpawnPlayer()
//    {
//        if (player != null)
//        {
//            Destroy(player);
//        }
//        Vector3 playerStartPosition = IsoVectors.IsoToWorld(startTile, tileSize);
//        player = (GameObject)Instantiate(playerPrefab);
//        player.transform.position = playerStartPosition;
//        
//        player.GetComponent<PlayerScript>().StartMovement();
//
//        var cameraStartPosition = playerStartPosition;
//        cameraStartPosition.z = -10;
//        camera.transform.position = cameraStartPosition;
//
//        playerScript = player.GetComponent<PlayerScript>();
//    }

    public void AddTile()
    {
        tilesLeft++;
    }

    public void SubTile()
    {
        tilesLeft--;
    }
    
    private void OnDrawGizmos()
    {
        IsoVectors.drawPoint(startTile, Color.cyan, tileSize);
        IsoVectors.drawPoint(endTile, Color.blue, tileSize);
    }
}
