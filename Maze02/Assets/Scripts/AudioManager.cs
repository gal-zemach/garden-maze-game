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
    public AudioClip cantGrowSound;
    public AudioClip newChangeableTile;
    
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
    
    public void PlayCantGrow(AudioSource audioSource=null)
    {
        if (audioSource == null)
            audioSource = ownAudioSource;

        audioSource.clip = cantGrowSound;
        audioSource.Play();
    }
    
    public void PlayNewChangeableTile(AudioSource audioSource=null)
    {
        if (audioSource == null)
            audioSource = ownAudioSource;

        audioSource.clip = newChangeableTile;
        audioSource.Play();
    }
}
