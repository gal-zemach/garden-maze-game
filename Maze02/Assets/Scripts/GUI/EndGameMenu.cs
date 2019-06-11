using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour
{
    public float timeBetweenAnimations;
    public Vector3 percentages;
    
    [Space(20)]
    public Text messageText;
    public GameObject icon1, icon2, icon3;
    
    private Animator animator1, animator2, animator3;
    private WaitForSeconds timeToNextAnim;
    
    private const string MSG1 = "You can do better";
    private const string MSG2 = "Meh...";
    private const string MSG3 = "Well Done!";
    private const string MSG4 = "Mowing Genius!";

    private void Start()
    {
        animator1 = icon1.GetComponent<Animator>();
        animator2 = icon2.GetComponent<Animator>();
        animator3 = icon3.GetComponent<Animator>();
        
        timeToNextAnim = new WaitForSeconds(timeBetweenAnimations);
    }

    public void StartAnimation(float completionPercentage)
    {
        ChooseMessage(completionPercentage);
        StartCoroutine(ScoreAnimations(completionPercentage));
    }

    private IEnumerator ScoreAnimations(float percentage)
    {        
        yield return timeToNextAnim;

        if (percentage < percentages.x)
            yield break;      
        
        animator1.SetBool("animate", true);
        yield return timeToNextAnim;
        
        if (percentage < percentages.y)
            yield break;
        
        animator2.SetBool("animate", true);
        yield return timeToNextAnim;
        
        if (percentage < percentages.z)
            yield break;
        
        animator3.SetBool("animate", true);
        yield return null;
    }

    private void ChooseMessage(float percentage)
    {
        if (percentage < percentages.x)
            messageText.text = MSG1;

        else if (percentage < percentages.y)
            messageText.text = MSG2;
        
        else if (percentage < percentages.z)
            messageText.text = MSG3;
        
        else
            messageText.text = MSG4;
    }
}
