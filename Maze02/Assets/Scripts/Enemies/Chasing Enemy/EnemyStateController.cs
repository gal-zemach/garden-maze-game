using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateController : StateController
{
//    public bool aiActive;
//    public State currentState;
//    public State remainState;

    [HideInInspector] public EnemyNavigationAgent enemyNavAgent;
    [HideInInspector] public ChasingEnemy chasingEnemy;
    
    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public PlayerScript playerScriptRef;
    
//    public List<Transform> wayPointList;
//    public Transform runAwayPoint;
//    [HideInInspector] public int nextWayPoint;
//    public Vector2 targetObject;
//    [HideInInspector] public float stateTimeElapsed;

    
    void Start()
    {
        enemyNavAgent = GetComponent<EnemyNavigationAgent>();
        chasingEnemy = GetComponent<ChasingEnemy>();
        targetObject = enemyNavAgent.currentCell;
        
        gameManager =  GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerScriptRef = gameManager.player.GetComponent<PlayerScript>();
        if (playerScriptRef == null)
        {
            Debug.Log("Chasing Enemy can't find player");
        }
        
        wayPointList = new List<Transform>();
        var waypointsParent = GameObject.Find("Waypoints Parent").transform;
        for (int i = 0; i < waypointsParent.childCount - 1; i++)
        {
            wayPointList.Add(waypointsParent.GetChild(i));
        }

        runAwayPoint = waypointsParent.GetChild(waypointsParent.childCount - 1);
    }

    void Update()
    {
        if (chasingEnemy.playerIsMoving)
            aiActive = true;
        
        if (!aiActive)
            return;
        
        currentState.UpdateState(this);
    }

    private void OnDrawGizmos()
    {
        if (currentState != null) // && eyes != null
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position, 16);
        }
    }

//    public void TransitionToState(State nextState)
//    {
//        if (nextState != remainState)
//        {
//            currentState = nextState;
//            OnExitState();
//        }
//    }
//
//    public bool ChackIfCountDownElapsed(float duration)
//    {
//        stateTimeElapsed += Time.deltaTime;
//        return (stateTimeElapsed >= duration);
//    }

//    private void OnExitState()
//    {
//        stateTimeElapsed = 0;
//    }
//
//    public void SetTargetObject(Vector2 tileIndex)
//    {
//        targetObject = tileIndex;
//    }
//    
//    public void SetTargetObject(Vector3 objectPosition)
//    {
//        targetObject = IsoVectors.WorldToIsoRounded(objectPosition, navAgent.map.actualTileSize);
//    }
}
