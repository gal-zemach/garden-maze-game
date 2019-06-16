using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class QuitScript : MonoBehaviour
{
    private SceneLoader sceneLoader;
    
    void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            sceneLoader.LoadScene(0);
    }
}
