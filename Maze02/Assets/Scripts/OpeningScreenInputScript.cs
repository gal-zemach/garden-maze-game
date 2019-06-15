using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class OpeningScreenInputScript : MonoBehaviour
{
    public AudioSource soundsAudioSource;
    public int nextSceneIndex = 1;
    
    private SceneLoader sceneLoader;
    
    void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();
    }

    void Update()
    {
        if (Input.anyKey || Input.GetMouseButtonDown(0))
        {
            soundsAudioSource.Play();
            StartCoroutine(WaitThenLoad(0.75f));
        }
    }

    private IEnumerator WaitThenLoad(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        sceneLoader.LoadScene(nextSceneIndex);
    }
}
