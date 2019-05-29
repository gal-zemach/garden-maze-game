using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FSMAIController : Controller
{
    public Vector2 currentCell, currentPosition, currentDestination, oldPosition;
    public float horizontalDirection, verticalDirection;
    
    private PlayerScript playerScript;
    private TileMap map;
    private Vector2 mapSize;
    private StateController stateController;
    
    private const float proximityEpsilon = 0.1f;
    private Vector2 EMPTY_CELL = new Vector2(-1, -1);
    
    void Start()
    {
        playerScript = GetComponent<PlayerScript>();
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        mapSize = map.mapSize;
        stateController = GetComponent<StateController>();
        currentDestination = EMPTY_CELL;
        horizontalDirection = 0;
        verticalDirection = 0;
    }

    void FixedUpdate()
    {
        if (!playerScript.movementStarted)
            return;
        
        currentCell = playerScript.gridCell;
        currentPosition = playerScript.gridPosition;
        
        if (currentDestination == EMPTY_CELL)
        {
            currentDestination = currentCell;
        }
        
        // go to currentDestination
        SetMoveParameters();
        
        // if reached destination - choose a new one
        if (Mathf.Approximately(horizontalDirection, 0) && 
            Mathf.Approximately(verticalDirection, 0))
        {
//            reachedDestination = true;
        }
    }

    public override float HorizontalAxis()
    {
        return horizontalDirection;
    }

    public override float VerticalAxis()
    {
        return verticalDirection;
    }

    private void SetMoveParameters()
    {
        var offset = Vector2.zero;
        var delta = Vector2.zero;
        delta = currentPosition - (currentDestination + offset);
        if (Mathf.Abs(delta.y) > proximityEpsilon)
        {
            horizontalDirection = Mathf.Sign(delta.y);
        }
        else
        {
            horizontalDirection = 0;
            
            if (Mathf.Abs(delta.x) > proximityEpsilon)
            {
                verticalDirection = -Mathf.Sign(delta.x);
            }
            else
            {
                verticalDirection = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (playerScript != null)
        {
            IsoVectors.drawPoint(currentPosition, Color.yellow, playerScript.tileSize);
            IsoVectors.drawPoint(currentDestination, Color.green, playerScript.tileSize);
        }
    }
}