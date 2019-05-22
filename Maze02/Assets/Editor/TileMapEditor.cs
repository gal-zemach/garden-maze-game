using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor
{
    public TileMap map;
    public SerializedObject tileMapSerialized;
    public SerializedProperty tilePrefabsList;

    private TileBrush brush;
    private Vector3 mouseHitPos;
    private Vector2 mouseGridPos = Vector2.zero;
    
    private bool mouseOnMap
    {
        get
        {
            return mouseGridPos.x >= 0 && mouseGridPos.x < map.mapSize.x &&
                   mouseGridPos.y >= 0 && mouseGridPos.y < map.mapSize.y;
        }
    }
    
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();

        var oldSize = map.mapSize;
        map.mapSize = EditorGUILayout.Vector2Field("Map Size:", map.mapSize);
        if (map.mapSize != oldSize)
        {
            UpdateCalculations();
        }
        
        map.tileSize = EditorGUILayout.Vector2Field("Tile Size:", map.tileSize);
        
        var oldTexture = map.texture2D;
        map.texture2D = (Texture2D) EditorGUILayout.ObjectField("Texture2D:", map.texture2D, typeof(Texture2D), false);
        if (map.texture2D != oldTexture)
        {
            UpdateCalculations();
            map.tileID = 1;
            NewBrush();
        }

        if (map.texture2D == null)
        {
            EditorGUILayout.HelpBox("You have not selected a texture 2D yet.", MessageType.Warning);
        }
        else
        {            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tile SIze:", map.tileSize.x + "x" + map.tileSize.y);
            map.tilePadding = EditorGUILayout.Vector2Field("Tile Padding", map.tilePadding);
            EditorGUILayout.LabelField("Grid SIze in Units:", map.gridSize.x + "x" + map.gridSize.y);
            EditorGUILayout.LabelField("Pixels To Units:", map.pixelsToUnits.ToString());
            UpdateBrush(map.currentTileBrush);
            
            if (GUILayout.Button("Clear Tiles"))
            {
                if (EditorUtility.DisplayDialog("Clear map's tiles?", "Are you sure?", "Clear", "Do not clear"))
                {
                    ClearMap();
                }
            }            
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(tilePrefabsList, new GUIContent("Tile Prefabs:"), true);
        
        EditorGUILayout.EndVertical();

        tileMapSerialized.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        map = target as TileMap;
        Tools.current = Tool.View;

        if (map.tilesParent == null)
        {
            var go = new GameObject("Tiles");
            go.transform.SetParent(map.transform);
            go.transform.position = Vector3.zero;
            map.tilesParent = go;
        }
        
        tileMapSerialized = new SerializedObject(target);
        tilePrefabsList = tileMapSerialized.FindProperty("tilePrefabs");
        
        if (map.texture2D != null)
        {
            UpdateCalculations();
            NewBrush();
        }
    }
    
    private void OnDisable()
    {
        DestroyBrush();
    }

    private void OnSceneGUI()
    {
        if (brush != null)
        {
            UpdateHitPosition();
            MoveBrush();

            if (map.texture2D != null && mouseOnMap)
            {
                Event current = Event.current;
                if (current.shift)
                {
                    Draw();
                }
                else if (current.alt)
                {
                    RemoveTile();
                }
            }
        }
    }

    private void UpdateCalculations()
    {
        var oldSpriteReferences = map.spriteReferences;
        
        var path = AssetDatabase.GetAssetPath(map.texture2D);
        map.spriteReferences = AssetDatabase.LoadAllAssetsAtPath(path);

        if (oldSpriteReferences == null)
        {
            map.tilePrefabs = new List<GameObject>(map.spriteReferences.Length - 1);
        }

//        var sprite = (Sprite) map.spriteReferences[1];
//        var width = sprite.textureRect.width;
//        var height = sprite.textureRect.height;

        var width = map.tileSize.x;
        var height = map.tileSize.y;

        map.tileSize = new Vector2(width, height);
        map.actualTileSize = new Vector2(width, height / 2);
        
//        map.pixelsToUnits = (int) (sprite.rect.width / sprite.bounds.size.x);
        map.pixelsToUnits = (int) (map.spriteReferences[1] as Sprite).pixelsPerUnit;
        map.gridSize = new Vector2((width / map.pixelsToUnits) * map.mapSize.x, 
                                   (height / map.pixelsToUnits) * (0.5f * map.mapSize.y));
    }
    
    void createBrush()
    {
        var sprite = map.currentTileBrush;
        if (sprite != null)
        {
            GameObject go = new GameObject("Brush");
            go.transform.SetParent(map.transform);

            brush = go.AddComponent<TileBrush>();
            brush.renderer2D = go.AddComponent<SpriteRenderer>();
            brush.renderer2D.sortingOrder = 1000;
            
            var pixelsToUnits = map.pixelsToUnits;
            brush.brushSize = new Vector2(sprite.textureRect.width / pixelsToUnits, 
                                          sprite.textureRect.height / pixelsToUnits);
            brush.UpdateBrush(sprite);
            brush.drawingColor = Color.green;
        }
    }

    void NewBrush()
    {
        if (brush == null)
        {
            createBrush();
        }
    }
    
    void DestroyBrush()
    {
        if (brush != null)
        {
            DestroyImmediate(brush.gameObject);
        }
    }

    public void UpdateBrush(Sprite sprite)
    {
        if (brush != null)
        {
            brush.UpdateBrush(sprite);
        }
    }

    private void UpdateHitPosition()
    {
        var p = new Plane(map.transform.TransformDirection(Vector3.forward), Vector3.zero);
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        var hit = Vector3.zero;
        var dist = 0f;

        if (p.Raycast(ray, out dist))
        {
            hit = ray.origin + ray.direction.normalized * dist;
        }

        mouseHitPos = map.transform.InverseTransformPoint(hit);
    }
    
    private void MoveBrush()
    {
        var tileSizeX = map.tileSize.x / map.pixelsToUnits;
        var tileSizeY = map.tileSize.y / 2 / map.pixelsToUnits;

        var offset = new Vector3(tileSizeY, tileSizeY);
        mouseHitPos += offset;
        
        var column = Mathf.Floor(mouseHitPos.x / tileSizeX + mouseHitPos.y / tileSizeY) - 1;
        var row = Mathf.Floor(mouseHitPos.y / tileSizeY - mouseHitPos.x / tileSizeX);
        mouseGridPos = new Vector2(column, row);

        if (!mouseOnMap)
            return;
        
        var id = (int) ((column * map.mapSize.y) + row);
        brush.tileID = id;

        var x = column * (tileSizeX / 2) - row * (tileSizeX / 2);
        var y = column * (tileSizeY / 2) + row * (tileSizeY / 2);
        
        x += map.transform.position.x;
        y += map.transform.position.y + tileSizeY / 2;
        
        brush.transform.position = new Vector3(x, y, map.transform.position.z);
    }

    private void Draw()
    {
        var id = brush.tileID.ToString();

        var posX = brush.transform.position.x;
        var posY = brush.transform.position.y;
        
        GameObject tile = GameObject.Find(map.name + "/Tiles/tile_" + id);

        if (tile != null)
        {
            DestroyImmediate(tile);
        }
        
        // create the new tile
        GameObject prefab = null;
        if (map.tileID - 1 < map.tilePrefabs.Count)
        {
            prefab = map.tilePrefabs[map.tileID - 1];
        }
        if (prefab != null)
        {
            tile = (GameObject) Instantiate(prefab, map.tilesParent.transform);
        }
        else
        {
            // defining constWall
            tile = new GameObject();
            tile.transform.SetParent(map.tilesParent.transform);
            var tempTileScript = (Tile) tile.AddComponent<ConstWall>();
            tempTileScript.type = TileMap.TileType.constWall;
            
            var spriteRenderer = tile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = brush.renderer2D.sprite;
        }
        
        tile.name = "tile_" + id;
        tile.transform.position = new Vector3(posX, posY, mouseGridPos.x + mouseGridPos.y);

        var tileScript = tile.GetComponent<Tile>();
        int column = brush.tileID / (int) map.mapSize.y;
        tileScript.index = new Vector2(column, brush.tileID - column * map.mapSize.y);
    }

    private void RemoveTileCollider(GameObject tile)
    {
        var collider = tile.GetComponent<Collider2D>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
    }
    
    private void RemoveTile()
    {
        var id = brush.tileID.ToString();
        
        GameObject tile = GameObject.Find(map.name + "/Tiles/tile_" + id);
        if (tile != null)
        {
            DestroyImmediate(tile);
        }
    }
    
    void ClearMap()
    {
        while (map.tilesParent.transform.childCount > 0)
        {
            Transform t = map.tilesParent.transform.GetChild(0);
            DestroyImmediate(t.gameObject);
        }
    }
}
