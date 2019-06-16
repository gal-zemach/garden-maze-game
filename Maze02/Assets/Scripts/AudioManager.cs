using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip cutGrassSound;
    public AudioClip tileUpSound;
    public AudioClip speedUpSound;
    public AudioClip hitPlayerSound;
    public AudioClip levelClearSound;
    public AudioClip openGatesSound;
    
    private AudioSource ownAudioSource;

    private void Start()
    {
        ownAudioSource = GetComponent<AudioSource>();
    }

    public void PlayGrassCut(AudioSource audioSource=null)
    {
        if (audioSource == null)
            audioSource = ownAudioSource;
        
        audioSource.clip = cutGrassSound;
        audioSource.Play();
    }
    
    public void PlayTileUp(AudioSource audioSource=null)
    {
        if (audioSource == null)
            audioSource = ownAudioSource;

        audioSource.clip = tileUpSound;
        audioSource.Play();
    }
    
    public void PlaySpeedUp(AudioSource audioSource=null)
    {
        if (audioSource == null)
            audioSource = ownAudioSource;

        audioSource.clip = speedUpSound;
        audioSource.Play();
    }
    
    public void PlayHitPlayer(AudioSource audioSource=null)
    {
        if (audioSource == null)
            audioSource = ownAudioSource;

        audioSource.clip = hitPlayerSound;
        audioSource.Play();
    }
    
    public void PlayLevelClear(AudioSource audioSource=null)
    {
        if (audioSource == null)
            audioSource = ownAudioSource;

        audioSource.clip = levelClearSound;
        audioSource.Play();
    }
    
    public void PlayOpenGates(AudioSource audioSource=null)
    {
        if (audioSource == null)
            audioSource = ownAudioSource;

        audioSource.clip = openGatesSound;
        audioSource.Play();
    }
}
