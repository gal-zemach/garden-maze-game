using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class AnyKeyToContinue : MonoBehaviour
{
    public int nextScene;

    private SceneLoader sceneLoader;
    
    void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();
    }

    void Update()
    {
        if (Input.anyKey)
        {
            sceneLoader.LoadScene(nextScene);
        }
    }
}
