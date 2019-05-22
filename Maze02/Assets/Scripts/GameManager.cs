using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector2 startTile, endTile;

    [HideInInspector]
    public GameObject player;
    private TileMap map;
    private Vector2 tileSize;

    private bool gameEnded;

    private GameObject camera;
    
    void Start()
    {
        camera = GameObject.Find("Main Camera");
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        tileSize = new Vector2(map.tileSize.x, map.tileSize.y / 2);

        SpawnPlayer();

        gameEnded = false;
    }

    void Update()
    {
        if (gameEnded)
            return;
        
        var playerPosition = player.transform.position;
        Vector2 playerTile = IsoVectors.WorldToIso(playerPosition, tileSize);
        playerTile = new Vector2(Mathf.Round(playerTile.x), Mathf.Round(playerTile.y));

        if (playerTile == endTile)
        {
            endGame(true);
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
            Debug.Log("GameManager: you Lose! Respawning...");
            camera.transform.position = new Vector3(1, 103, -10); // todo: remove this
            SpawnPlayer();
        }
        gameEnded = true;
    }


    private void SpawnPlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }
        Vector3 playerStartPosition = IsoVectors.IsoToWorld(startTile, tileSize);
        player = (GameObject)Instantiate(playerPrefab);
        player.transform.position = playerStartPosition;
        
        player.GetComponent<PlayerScript>().StartMovement();
    }

    public void PlayerDied()
    {
        endGame();
    }

    
    private void OnDrawGizmos()
    {
        drawPoint(startTile, Color.white);
        drawPoint(endTile, Color.green);
    }

    private void drawPoint(Vector2 position, Color color)
    {
        Vector3 destinationPosition = IsoVectors.IsoToWorld(position, tileSize);
        destinationPosition.z = -100;
        Gizmos.color = color;
        Gizmos.DrawSphere(destinationPosition, 5f);
    }
}
