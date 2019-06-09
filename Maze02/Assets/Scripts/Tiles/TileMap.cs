using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class TileMap : MonoBehaviour
{
    
    public Vector2 mapSize = new Vector2(20, 10);
    public Texture2D texture2D;
    public Vector2 tileSize = new Vector2();
    public Vector2 actualTileSize;
    public Vector2 tilePadding = new Vector2();
    public Object[] spriteReferences;
    public Vector2 gridSize;
    public int pixelsToUnits = 100;
    public int tileID = 0;
    public GameObject tilesParent;

    public List<GameObject> tilePrefabs;
    
    public Tile[] tiles;
    public NesScripts.Controls.PathFind.Grid pGrid, pCutGrassGrid;
    public byte[,] pCutGrassRefGrid;
    
    [HideInInspector]public byte GRID_BLOCKED = 0;
    [HideInInspector]public byte GRID_WALKABLE = 1;

    public enum TileType
    {
        Floor, constWall, moveableWall, trap, Gate
    }
    
    public Sprite currentTileBrush
    {
        get {return spriteReferences[tileID] as Sprite;}
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        tiles = new Tile[(int)(mapSize.x * mapSize.y)];
        UpdateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateTiles()
    {
        var grid = new float[(int)mapSize.x, (int)mapSize.y];

        for (int i = 0; i < tilesParent.transform.childCount; i++)
        {
            var tile = tilesParent.transform.GetChild(i);
            var tileIndexString = tile.name.Substring(5);
            int tileIndex;
            if (!int.TryParse(tileIndexString, out tileIndex))
            {
                tileIndex = -1;
            }

            if (tileIndex != -1)
            {
                tiles[tileIndex] = tile.GetComponent<Tile>(); 
            }
            else
            {
                Debug.LogError("TileMap/UpdateTiles: got tile index " + tileIndexString);
            }
        }
        
        pGrid = new NesScripts.Controls.PathFind.Grid(grid);
        pCutGrassGrid = new NesScripts.Controls.PathFind.Grid(grid);
        pCutGrassRefGrid = new byte[(int)mapSize.x, (int)mapSize.y];
    }

    public int TileIndex(int column, int row)
    {
        return (int) (column * mapSize.y + row);
    }
    
    public TileType GetTileType(int column, int row)
    {
        if (!IsValidIndex(column, row))
        {
            return TileType.constWall;
        }
        return tiles[TileIndex(column, row)].type;
    }

    public Tile GetTile(Vector2 index)
    {
        if (!IsValidIndex(index))
            return null;

        return tiles[TileIndex((int) index.x, (int) index.y)];
    }

    public bool IsWalkable(Vector2 tile)
    {
        var tileType = GetTileType((int) tile.x, (int) tile.y);

        var isOpenGate = false;
        if (tileType == TileType.Gate)
        {
            var tileScript = tiles[TileIndex((int) tile.x, (int) tile.y)];
            var gate = tileScript as Gate;
            if (gate != null)
                isOpenGate = gate.open;
        }
        
        return tileType == TileType.Floor || tileType == TileType.trap || isOpenGate;
    }

    public bool IsValidIndex(Vector2 index)
    {
        return IsValidIndex((int)index.x, (int)index.y);
    }
    
    public bool IsValidIndex(int column, int row)
    {
        return column >= 0 && column < mapSize.x && row >= 0 && row < mapSize.y;
    }

    public Vector3 GetZPosition(Transform otherTransform)
    {
        var pos = otherTransform.position;
        var gridCell = IsoVectors.WorldToIsoFloored(pos, actualTileSize);
        pos.z = gridCell.x + gridCell.y;
        return pos;
    }

    public void UpdateWalkabilityGrid(Vector2Int index, bool walkable)
    {
        if (walkable)
        {
            pGrid.nodes[index.x, index.y].Update(GRID_WALKABLE, index.x, index.y);
            return;
        }
        pGrid.nodes[index.x, index.y].Update(GRID_BLOCKED, index.x, index.y);
    }
    
    public void UpdateCutGrassGrid(Vector2Int index, bool isCut)
    {
        if (isCut)
        {
            pCutGrassRefGrid[index.x, index.y] = GRID_WALKABLE;
            pCutGrassGrid.nodes[index.x, index.y].Update(GRID_WALKABLE, index.x, index.y);
            return;
        }
        pCutGrassRefGrid[index.x, index.y] = GRID_BLOCKED;
        pCutGrassGrid.nodes[index.x, index.y].Update(GRID_BLOCKED, index.x, index.y);
    } 
    
    private void OnDrawGizmosSelected()
    {
        var pos = transform.position;

        if (texture2D != null)
        {
            Gizmos.color = Color.grey;
            var row = 0;
            var maxColumns = (int) mapSize.x;
            var total = mapSize.x * mapSize.y;
            var tile = new Vector3(tileSize.x / pixelsToUnits, tileSize.y / pixelsToUnits);

            for (var i = 0; i < total; i++)
            {
                var column = i % maxColumns;

                var newX = column * (tile.x / 2) - row * (tile.x / 2);
                var newY = column * (tile.y / 4) + row * (tile.y / 4);
                
                var p1 = new Vector3(newX, newY - (tileSize.y / 4));
                var p2 = new Vector3(newX + (tileSize.x / 2), newY);
                var p3 = new Vector3(newX, newY + (tileSize.y / 4));
                var p4 = new Vector3(newX - (tileSize.x / 2), newY);

                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p4);
                Gizmos.DrawLine(p4, p1);
                
                if (column == maxColumns - 1)
                {
                    row++;
                }
            }
            
            Gizmos.color = Color.white;
            
            var downX = 0 * (tile.x / 2) - 0 * (tile.x / 2);
            var downY = 0 * (tile.y / 4) + 0 * (tile.y / 4);
            
            var upX = mapSize.x * (tile.x / 2) - mapSize.y * (tile.x / 2);
            var upY = mapSize.x * (tile.y / 4) + mapSize.y * (tile.y / 4);
            
            var leftX = 0 * (tile.x / 2) - mapSize.y * (tile.x / 2);
            var leftY = 0 * (tile.y / 4) + mapSize.y * (tile.y / 4);
            
            var rightX = mapSize.x * (tile.x / 2) - 0 * (tile.x / 2);
            var rightY = mapSize.x * (tile.y / 4) + 0 * (tile.y / 4);
            
            var offset = new Vector3(0, -tile.y / 4);
            
            var b1 = new Vector3(downX, downY) + offset;
            var b2 = new Vector3(rightX, rightY) + offset;
            var b3 = new Vector3(upX, upY) + offset;
            var b4 = new Vector3(leftX, leftY) + offset;
            
            Gizmos.DrawLine(b1, b2);
            Gizmos.DrawLine(b2, b3);
            Gizmos.DrawLine(b3, b4);
            Gizmos.DrawLine(b4, b1);
        }
    }
}
