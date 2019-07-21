using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour
{
    public float timeBetweenAnimations;
    public Vector3 scoreLevels;
    
    [Space(20)]
    public Text messageText;
    public GameObject icon1, icon2, icon3;
    public AudioClip scoreCountSound, grassSound, textSound;

    [HideInInspector] 
    public bool immediateScore;
    
    private GameManager gameManager;
    private AudioSource audioSource1, audioSource2;
    private Animator animator1, animator2, animator3;
    private float totalScoreAnimTime = 2f;
    private float timeBetweenIncs = 0.005f;
    
    private const string MSG1 = "You can do better";
    private const string MSG2 = "Meh...";
    private const string MSG3 = "Well Done!";
    private const string MSG4 = "Mowing Genius!";

    private bool anim1Ran, anim2Ran, anim3Ran;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        animator1 = icon1.GetComponent<Animator>();
        animator2 = icon2.GetComponent<Animator>();
        animator3 = icon3.GetComponent<Animator>();
        audioSource1 = transform.Find("AudioSource1").GetComponent<AudioSource>();
        audioSource2 = transform.Find("AudioSource2").GetComponent<AudioSource>();
    }

    public void StartAnimation(int finalScore)
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();  // here so this go can start inactive
        audioSource1 = transform.Find("AudioSource1").GetComponent<AudioSource>();
        audioSource2 = transform.Find("AudioSource2").GetComponent<AudioSource>();
        
        animator1 = icon1.GetComponent<Animator>();
        animator2 = icon2.GetComponent<Animator>();
        animator3 = icon3.GetComponent<Animator>();
        
        scoreLevels = gameManager.completionPercentages * gameManager.totalGrassToCut * gameManager.scorePerGrassTile;
        Debug.Log(scoreLevels);
        
        ChooseMessage(finalScore);
        messageText.gameObject.SetActive(false);
        StartCoroutine(ScoreAnimation(finalScore));
    }

    public Text scoreText;
    
    private IEnumerator ScoreAnimation(int finalScore)
    {
        if (immediateScore)
        {
            scoreText.text = finalScore.ToString();
            messageText.gameObject.SetActive(true);
            StartCoroutine(OnlyGrassAnimations(finalScore));
            yield break;
        }
        audioSource2.clip = scoreCountSound;
        var currentScore = 0;
        while (currentScore < finalScore)
        {
            yield return new WaitForSeconds(timeBetweenIncs);
            audioSource2.Play();
            currentScore += gameManager.scorePerGrassTile;
            scoreText.text = currentScore.ToString();

            UpdateGrassAnim(currentScore);
        }
        audioSource2.clip = textSound;
        audioSource2.Play();
        messageText.gameObject.SetActive(true);
    }

    private void UpdateGrassAnim(float score)
    {
        if (score < scoreLevels.x)
            return;

        if (!animator1.GetBool("animate"))
        {
            animator1.SetBool("animate", true);
            audioSource1.clip = grassSound;
            audioSource1.Play();
        }
        
        if (score < scoreLevels.y)
            return;
        
        if (!animator2.GetBool("animate"))
        {
            animator2.SetBool("animate", true);
            audioSource1.clip = grassSound;
            audioSource1.Play();
        }
        
        if (score < scoreLevels.z)
            return;
        
        if (!animator3.GetBool("animate"))
        {
            animator3.SetBool("animate", true);
            audioSource1.clip = grassSound;
            audioSource1.Play();
        }
    }

    private void ChooseMessage(float percentage)
    {
        if (percentage < scoreLevels.x)
            messageText.text = MSG1;

        else if (percentage < scoreLevels.y)
            messageText.text = MSG2;
        
        else if (percentage < scoreLevels.z)
            messageText.text = MSG3;
        
        else
            messageText.text = MSG4;
    }
    
    private IEnumerator OnlyGrassAnimations(float score)
    {        
        var timeToNextAnim = new WaitForSeconds(0.45f);
        yield return timeToNextAnim;

        if (score < scoreLevels.x)
            yield break;      
        
        animator1.SetBool("animate", true);
        yield return timeToNextAnim;
        
        if (score < scoreLevels.y)
            yield break;
        
        animator2.SetBool("animate", true);
        yield return timeToNextAnim;
        
        if (score < scoreLevels.z)
            yield break;
        
        animator3.SetBool("animate", true);
        yield return null;
    }
}
