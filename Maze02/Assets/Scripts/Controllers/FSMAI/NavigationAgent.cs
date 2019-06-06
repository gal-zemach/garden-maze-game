using System.Collections;
using System.Collections.Generic;
using NesScripts.Controls.PathFind;
using UnityEngine;

public class NavigationAgent : Controller
{
    [HideInInspector] public TileMap map;
    
    public Vector2 currentCell, currentPosition;
    public Vector2 destination, currentDestination;
    public bool reachedDestination;
    public int remainingDistance;

    public int lookRadius = 3;
    public int lookRadiusInPixels = 100;
    
    
    private PlayerScript playerScript;
    private bool stopped;
    private float horizontalDirection, verticalDirection;
    public List<Point> path;
    
    private const float PROXIMITY_EPSILON = 0.1f;
    
    void Start()
    {
        map = GameObject.Find("Tile Map").GetComponent<TileMap>();
        playerScript = GetComponent<PlayerScript>();
        
        horizontalDirection = 0;
        verticalDirection = 0;
        path = new List<Point>();
        
        destination = currentDestination = playerScript.gridCell;
    }
    
    void FixedUpdate()
    {
        if (!playerScript.playerIsMoving)
            Stop();
        else
        {
            if (stopped)
                currentDestination = playerScript.gridCell;
            Resume();
        }
        
        if (stopped)
        {
            horizontalDirection = 0;
            verticalDirection = 0;
            return;
        }
        
        // update position
        currentCell = playerScript.gridCell;
        currentPosition = playerScript.gridPosition;
        
        SetMoveParameters();
        
        ResetIfStuck();
        
        // if reached destination - choose a new one
        if (Mathf.Approximately(horizontalDirection, 0) && Mathf.Approximately(verticalDirection, 0))
        {
            GetNextDestination();
        }
        
//        ResetIfStuck();
    }
    
    public override float HorizontalAxis()
    {
        return horizontalDirection;
    }

    public override float VerticalAxis()
    {
        return verticalDirection;
    }
    
    public void Resume()
    {
        stopped = false;
    }

    public void Stop()
    {
        stopped = true;
    }

    public void UpdateDestination(Vector2 newDest)
    {
        Stop();
        destination = newDest;
        
        var startPos = new Point((int)currentCell.x, (int)currentCell.y);
        var endPos = new Point((int) newDest.x, (int)newDest.y);
        path = Pathfinding.FindPath(map.pGrid, startPos, endPos, Pathfinding.DistanceType.Manhattan);
        
        remainingDistance = path.Count;
        reachedDestination = (remainingDistance == 0);
        
        Resume();
    }

    public void UpdateDestination(Vector3 destPosition)
    {
        var newDest = IsoVectors.WorldToIsoRounded(destPosition, map.actualTileSize);
        UpdateDestination(newDest);
    }

    private void GetNextDestination()
    {
        if (path.Count > 0)
        {
            reachedDestination = false;
            var p = path[0];
            currentDestination = new Vector2(p.x, p.y);
            path.RemoveAt(0);
            remainingDistance--;
            return;
        }
        
        reachedDestination = true;
    }

    private void SetMoveParameters()
    {
        var offset = Vector2.zero;
        var delta = currentPosition - (currentDestination + offset);
        if (Mathf.Abs(delta.y) > PROXIMITY_EPSILON)
        {
            horizontalDirection = Mathf.Sign(delta.y);
        }
        else
        {
            horizontalDirection = 0;
            
            if (Mathf.Abs(delta.x) > PROXIMITY_EPSILON)
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
            IsoVectors.drawPoint(currentPosition, Color.green, playerScript.tileSize);
            IsoVectors.drawPoint(currentDestination, Color.yellow, playerScript.tileSize);
            IsoVectors.drawPoint(destination, Color.red, playerScript.tileSize);
        }
    }


    private float stuckDistance = 0.2f;
    private float lastCheckTime = 0f;
    private Vector2 lastCheckCell;
    private float stuckCheckTime = 1f;
    
    private void ResetIfStuck()
    {
        if (Time.time - lastCheckTime > stuckCheckTime)
        {
            if ((currentPosition - lastCheckCell).magnitude < stuckDistance)
            {
                playerScript.playerIsMoving = false;
                transform.position = IsoVectors.IsoToWorld(currentCell, map.actualTileSize);
                
//                Debug.Log("NavigationAgent: stuck for too long");
                currentDestination = currentCell;
                UpdateDestination(destination);

                playerScript.playerIsMoving = true;
            }

            lastCheckCell = currentPosition;
            lastCheckTime = Time.time;
        }
    }
}
