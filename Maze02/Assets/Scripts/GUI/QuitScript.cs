using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class QuitScript : MonoBehaviour
{
    private SceneLoader sceneLoader;
    private float idleSecondsToRestart = 300;
    private float lastInputTime;
    
    void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();
        lastInputTime = Time.time;
    }

    void Update()
    {
        if (Input.anyKey)
        {
            lastInputTime = Time.time;
        }
        
        if (Time.time - lastInputTime > idleSecondsToRestart)
        {
            Debug.Log("QuitScript: Game is idle for too long");
            sceneLoader.LoadScene(0);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
            sceneLoader.LoadScene(0);
    }
}
