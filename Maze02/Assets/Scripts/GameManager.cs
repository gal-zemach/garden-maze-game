using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector2 startTile;
    [HideInInspector] public GameObject player;

    [Space(20)] 
    public float percentageToOpenGates;
    public Vector3 completionPercentages;
    public float currentCompletionPercentage;
    public Vector2Int GrassToTileRatio = new Vector2Int(3, 1);
    public int initialChangeableTiles = 0;
    public int score;
    public bool enemyExists = true;
    
    [Space(20)]
    public int grassTilesLeft;
    public int changeableTiles;

    public bool avoidInfectedTiles;
    
    [Space(20)] 
    public bool forceOpenGates;
    
    [HideInInspector] public List<Gate> gates;
    
    [HideInInspector] public int totalGrassToCut;
    [HideInInspector] public int grassCut;
    private bool gatesOpen = false;

    private GUIManager guiManager;
    private AudioManager audioManager;
    private GameObject camera;
    private TileClicker tileClicker;
    private PlayerScript playerScript;
    private TileMap map;
    private Vector2 tileSize;
    private TileSpreadingEnemy tileSpreadingEnemy;

    private bool gameEnded;
    private bool reachedGate;

    private WaitForSeconds startWait = new WaitForSeconds(2.5f);
    private WaitForSeconds endWait;
    private WaitForSeconds reloadWait = new WaitForSeconds(3);
    
    void Start()
    {
        guiManager = GetComponent<GUIManager>();
        audioManager = GetComponent<AudioManager>();
        camera = GameObject.Find("Main Camera");
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        tileSize = new Vector2(map.tileSize.x, map.tileSize.y / 2);

        tileClicker = GetComponent<TileClicker>();

        changeableTiles = initialChangeableTiles;
        gameEnded = false;

        map.avoidInfectedTiles = avoidInfectedTiles;

        if (enemyExists)
            tileSpreadingEnemy = GameObject.Find("TileSpreadingEnemy").GetComponent<TileSpreadingEnemy>();
        
        StartCoroutine(GameLoop());
    }

    private void Update()
    {
        if (!gatesOpen && forceOpenGates)
            OpenGates();
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
        score = 0;
        
        yield return startWait;
    }
    
    private IEnumerator LevelPlaying()
    {        
        totalGrassToCut = grassTilesLeft;
//        OpenGates();
        if (guiManager != null)
            guiManager.StartGUI();
        
        Debug.Log("total tiles: " + totalGrassToCut);
        
        EnableControls();

        while (PlayerIsAlive() && !PlayerReachedEnd())
        {
            if (guiManager != null)
                guiManager.UpdateGUI();
            
            yield return null;
        }
    }
    
    private IEnumerator LevelEnding()
    {
        StopPlayerAndEnemy();

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
        audioManager.PlayLevelClear();
        playerScript.PlayerWon();
        yield return new WaitForSeconds(1);
        
        gameEnded = true;
        guiManager.ShowEndGameMenu(currentCompletionPercentage);

        yield return null;
    }

    private IEnumerator LoseGame()
    {
        Debug.Log("GameManager: you Lose!");

        yield return reloadWait;
        RestartLevel();
    }

    public void RestartLevel()
    {
        var currentScene = SceneManager.GetActiveScene();
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
    
    private void StopPlayerAndEnemy()
    {
        tileClicker.DisableControls();
        playerScript.DisableControls();
        if (enemyExists)
            tileSpreadingEnemy.isSpreading = false;
    }

    private void SpawnPlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }
        Vector3 playerStartPosition = IsoVectors.IsoToWorld(startTile, tileSize);
        player = (GameObject)Instantiate(playerPrefab);
        playerStartPosition.z = startTile.x + startTile.y;
        player.transform.position = playerStartPosition;

        var currentCameraPos = camera.transform.position;
        var cameraStartPosition = playerStartPosition;
        cameraStartPosition.z = currentCameraPos.z;
        camera.transform.position = cameraStartPosition;
        camera.GetComponent<CameraController>().target = player;

        playerScript = player.GetComponent<PlayerScript>();
    }

    private bool PlayerReachedEnd()
    {
        return reachedGate;
    }

    public void PlayerReachedGate()
    {
        Debug.Log("GameManager: Player reached gate.");
        reachedGate = true;
    }

    private bool PlayerIsAlive()
    {
        return !playerScript.IsDead();
    }
    
    
    public void AddTile()
    {
        grassTilesLeft++;
    }

    public void SubTile()
    {
        grassTilesLeft--;
        
        grassCut++;
        if (grassCut % GrassToTileRatio.x == 0)
        {
            changeableTiles += GrassToTileRatio.y;
        }
        
        currentCompletionPercentage = ((float)grassCut) / ((float)totalGrassToCut);

        score += 5;

        if (!gatesOpen && grassCut >= totalGrassToCut * percentageToOpenGates)
        {
            OpenGates();
        }
    }

    public void OpenGates()
    {
        Debug.Log("GameManager: Gates Open");
        audioManager.PlayOpenGates();
        gatesOpen = true;
        foreach (var gate in gates)
        {
            gate.Open();
        }
    }

    private void OnDrawGizmos()
    {
        IsoVectors.drawPoint(startTile, Color.cyan, tileSize);
//        IsoVectors.drawPoint(endTile, Color.blue, tileSize);
    }
}
