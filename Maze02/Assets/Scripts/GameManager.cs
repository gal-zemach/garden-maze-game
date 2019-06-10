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
    public float percentageToCompletion = 0.75f;
    public Vector2Int GrassToTileRatio = new Vector2Int(3, 1);
    public int initialChangeableTiles = 0;
    
    [Space(20)]
    public int grassTilesLeft;
    public int changeableTiles;

    public bool avoidInfectedTiles;
    
    [Space(20)] 
    public bool forceOpenGates;
    
    [HideInInspector] public List<Gate> gates;
    
    [HideInInspector] public int totalGrassToCut;
    private int grassCut;
    private bool gatesOpen = false;

    private GUIManager guiManager;
    private GameObject camera;
    private TileClicker tileClicker;
    private PlayerScript playerScript;
    private TileMap map;
    private Vector2 tileSize;

    private bool gameEnded;
    private bool reachedGate;

    private WaitForSeconds startWait = new WaitForSeconds(2.5f);
    private WaitForSeconds endWait;
    private WaitForSeconds reloadWait = new WaitForSeconds(3);
    
    void Start()
    {
        guiManager = GetComponent<GUIManager>();
        camera = GameObject.Find("Main Camera");
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        tileSize = new Vector2(map.tileSize.x, map.tileSize.y / 2);

        tileClicker = GetComponent<TileClicker>();

        changeableTiles = initialChangeableTiles;
        gameEnded = false;

        map.avoidInfectedTiles = avoidInfectedTiles;

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
        
        yield return startWait;
    }
    
    private IEnumerator LevelPlaying()
    {        
        totalGrassToCut = grassTilesLeft;
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
        playerStartPosition.z = startTile.x + startTile.y;
        player.transform.position = playerStartPosition;

        var cameraStartPosition = playerStartPosition;
        cameraStartPosition.z = -10;
        camera.transform.position = cameraStartPosition;

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

        if (!gatesOpen && grassCut >= totalGrassToCut * percentageToCompletion)
        {
            OpenGates();
        }
    }

    public void OpenGates()
    {
        Debug.Log("GameManager: Gates Open");
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
