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
    
    public List<Transform> wayPointList;
    [HideInInspector] public int nextWayPoint;
    [HideInInspector] public Transform targetObject;
    [HideInInspector] public float stateTimeElapsed;

    private PlayerScript playerScript;
    
    void Start()
    {
        navAgent = GetComponent<NavigationAgent>();
        playerScript = GetComponent<PlayerScript>();
    }

    void Update()
    {
        if (playerScript.movementStarted)
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
            Gizmos.DrawWireSphere(transform.position, 5);
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
}
