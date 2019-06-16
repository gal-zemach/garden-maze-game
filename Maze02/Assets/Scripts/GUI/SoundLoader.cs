using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SoundLoader : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public int sceneToLoad;
    public bool currentScene;
    
    [Space(20)]
    public AudioSource audioSource;
    public AudioClip highlightSound, clickSound;
    
    private SceneLoader sceneLoader;
    
    void Start()
    {
        sceneLoader = GameObject.Find("Game").GetComponent<SceneLoader>();
        if (currentScene)
            sceneToLoad = SceneManager.GetActiveScene().buildIndex;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        audioSource.clip = highlightSound;
        audioSource.Play();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        audioSource.clip = clickSound;
        audioSource.Play();
        StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad()
    {
        Debug.Log("loading scene " + sceneToLoad);
        yield return new WaitForSeconds(0.75f);
        sceneLoader.LoadScene(sceneToLoad);
    }
}
