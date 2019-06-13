using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconAnimator : MonoBehaviour
{
    public GameObject icon1, icon2, icon3;
    
    private GameManager gameManager;
    private Vector3 percentages;
    private Animator animator1, animator2, animator3;
    private bool played1, played2, played3;
    private string ANIM_VAR_NAME = "animate";
    
    private void Start()
    {
        animator1 = icon1.GetComponent<Animator>();
        animator2 = icon2.GetComponent<Animator>();
        animator3 = icon3.GetComponent<Animator>();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        percentages = gameManager.completionPercentages;
    }

    private void Update()
    {
        var currentPercentage = gameManager.currentCompletionPercentage;

        if (!played1 && currentPercentage >= percentages.x)
        {
            animator1.SetBool(ANIM_VAR_NAME, true);
            played1 = true;
        }
        
        if (!played2 && currentPercentage >= percentages.y)
        {
            animator2.SetBool(ANIM_VAR_NAME, true);
            played2 = true;
        }
        
        if (!played3 && currentPercentage >= percentages.z)
        {
            animator3.SetBool(ANIM_VAR_NAME, true);
            played3 = true;
        }
    }
}
