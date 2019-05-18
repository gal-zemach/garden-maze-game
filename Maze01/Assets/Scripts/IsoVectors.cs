using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoVectors : MonoBehaviour
{
    private const float ROT_ANGLE = 26.57f;
    
    public static Vector3 UP = Quaternion.Euler(new Vector3(0, 0, ROT_ANGLE)) * Vector3.right;
    public static Vector3 DOWN = -UP;
    public static Vector3 RIGHT = Quaternion.Euler(new Vector3(0, 0, -ROT_ANGLE)) * Vector3.right;
    public static Vector3 LEFT = -RIGHT;

    public static Vector2 Opposite(Vector2 dir)
    {
        return -dir;
    }
    
    public static Vector2 WorldToIso(Vector3 worldPosition, Vector2 tileSize)
    {
        var column = worldPosition.x / tileSize.x + worldPosition.y / tileSize.y;
        var row = worldPosition.y / tileSize.y - worldPosition.x / tileSize.x;

        var gridPos = new Vector2(column, row);
        return gridPos;
    }
    
    public static Vector2 WorldToIsoRounded(Vector3 worldPosition, Vector2 tileSize)
    {
        var iso = WorldToIso(worldPosition, tileSize);
        return new Vector2(Mathf.Round(iso.x), Mathf.Round(iso.y));
    }
    
    public static Vector2 WorldToIsoFloored(Vector3 worldPosition, Vector2 tileSize)
    {
        var iso = WorldToIso(worldPosition, tileSize);
        return new Vector2(Mathf.Floor(iso.x), Mathf.Floor(iso.y));
    }

    public static Vector2 IsoToWorld(Vector2 gridPos, Vector2 tileSize)
    {
        var x = gridPos.x * (tileSize.x / 2) - gridPos.y * (tileSize.x / 2);
        var y = gridPos.x * (tileSize.y / 2) + gridPos.y * (tileSize.y / 2);
        return new Vector2(x, y);
    }

    public static Vector2 WorldToTileCenter(Vector3 worldPosition, Vector2 tileSize)
    {
        var gridCell = WorldToIsoRounded(worldPosition, tileSize);
        return IsoToWorld(gridCell, tileSize);
    }

    private void OnDrawGizmosSelected()
    {
        var pos = transform.position;
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(pos, UP * 1000);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(pos, RIGHT * 1000);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(pos, DOWN * 1000);
        Gizmos.DrawRay(pos, LEFT * 1000);
    }
}
