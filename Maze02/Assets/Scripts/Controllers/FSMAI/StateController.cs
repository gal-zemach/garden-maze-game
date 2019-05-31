using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public bool aiActive;
    public State currentState;
    public State remainState;

    [HideInInspector] public NavigationAgent navAgent;
    [HideInInspector] public PlayerScript playerScript;
    
    public List<Transform> wayPointList;
    public Transform runAwayPoint;
    [HideInInspector] public int nextWayPoint;
    public Vector2 targetObject;
    [HideInInspector] public float stateTimeElapsed;

    
    void Start()
    {
        navAgent = GetComponent<NavigationAgent>();
        playerScript = GetComponent<PlayerScript>();
        targetObject = navAgent.currentCell;
        
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
        if (playerScript.playerIsMoving)
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

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool ChackIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }

    public void SetTargetObject(Vector2 tileIndex)
    {
        targetObject = tileIndex;
    }
    
    public void SetTargetObject(Vector3 objectPosition)
    {
        targetObject = IsoVectors.WorldToIsoRounded(objectPosition, navAgent.map.actualTileSize);
    }
}
