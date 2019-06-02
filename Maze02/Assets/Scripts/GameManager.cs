using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{    
    public GameObject playerPrefab;
    public Vector2 startTile, endTile;
    public int tilesLeft;
    [HideInInspector] public GameObject player;

    private GameObject camera;
    private TileClicker tileClicker;
    private PlayerScript playerScript;
    private TileMap map;
    private Vector2 tileSize;

    private bool gameEnded;

    private WaitForSeconds startWait = new WaitForSeconds(2.5f);
    private WaitForSeconds endWait;
    private WaitForSeconds reloadWait = new WaitForSeconds(3);
    
    void Start()
    {
        camera = GameObject.Find("Main Camera");
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        tileSize = new Vector2(map.tileSize.x, map.tileSize.y / 2);

        tileClicker = GetComponent<TileClicker>();
        
        gameEnded = false;

        StartCoroutine(GameLoop());
    }
    
    
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(LevelStart());
        
        yield return StartCoroutine(LevelPlaying());
        
        yield return StartCoroutine(LevelEnding());
    }

    private IEnumerator LevelStart()
    {
        SpawnPlayer();
        DisableControls();
        playerScript.AfterHit();
        
        yield return startWait;
    }
    
    private IEnumerator LevelPlaying()
    {        
        EnableControls();

        while (PlayerIsAlive() && !PlayerReachedEnd())
        {
            yield return null;
        }
    }
    
    private IEnumerator LevelEnding()
    {
        DisableControls();

        if (PlayerReachedEnd())
        {
            // winning option
            yield return winGame();
        }
        else
        {
            // losing option
            yield return LoseGame();
        }

        yield return endWait;
    }

    private IEnumerator winGame()
    {
        Debug.Log("GameManager: you Win!");
        gameEnded = true;
        Time.timeScale = 0;

        yield return null;
    }

    private IEnumerator LoseGame()
    {
        Debug.Log("GameManager: you Lose!");

        var currentScene = SceneManager.GetActiveScene();

        yield return reloadWait;
        SceneManager.LoadScene(currentScene.name);
    }

    private void EnableControls()
    {
        tileClicker.EnableControls();
        playerScript.EnableControls();
    }
    
    private void DisableControls()
    {
        tileClicker.DisableControls();
        playerScript.DisableControls();
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

        var cameraStartPosition = playerStartPosition;
        cameraStartPosition.z = -10;
        camera.transform.position = cameraStartPosition;

        playerScript = player.GetComponent<PlayerScript>();
    }

    private bool PlayerReachedEnd()
    {
        return playerScript.gridCell == endTile;
    }

    private bool PlayerIsAlive()
    {
        return !playerScript.IsDead();
    }
    
    
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
