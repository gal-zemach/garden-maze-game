using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor
{
    public TileMap map;

    private TileBrush brush;
    private Vector3 mouseHitPos;
    private Vector2 mouseGridPos = Vector2.zero;

    private TileMap.TileType currentDrawingType = TileMap.TileType.Floor;
    
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
            var oldDrawingType = currentDrawingType;
            currentDrawingType = (TileMap.TileType) EditorGUILayout.EnumPopup("Currently drawing:", currentDrawingType);
            if (currentDrawingType != oldDrawingType)
            {
                UpdateBrushColor();
            }
            
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
        
        EditorGUILayout.EndVertical();
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
        var path = AssetDatabase.GetAssetPath(map.texture2D);
        map.spriteReferences = AssetDatabase.LoadAllAssetsAtPath(path);

        var sprite = (Sprite) map.spriteReferences[1];
        var width = sprite.textureRect.width;
        var height = sprite.textureRect.height;
            
        map.tileSize = new Vector2(width, height);
        map.pixelsToUnits = (int) (sprite.rect.width / sprite.bounds.size.x);
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
            UpdateBrushColor();
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

    public void UpdateBrushColor()
    {
        switch (currentDrawingType)
        {
            case TileMap.TileType.Floor:
                brush.drawingColor = Color.cyan;
                break;
                
            case TileMap.TileType.moveableWall:
                brush.drawingColor = Color.green;
                break;
                
            case TileMap.TileType.constWall:
                brush.drawingColor = Color.red;
                break;
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
        if (tile == null)
        {
            tile = new GameObject("tile_" + id);
            tile.transform.SetParent(map.tilesParent.transform);
            tile.transform.position = new Vector3(posX, posY, mouseGridPos.x + mouseGridPos.y);
            
            tile.AddComponent<Tile>();
            tile.AddComponent<SpriteRenderer>();
            var tileAnimator = tile.AddComponent<Animator>();
            var animationController = Resources.Load("Animation/wall") as RuntimeAnimatorController;
            tileAnimator.runtimeAnimatorController = animationController;
            if (animationController == null)
            {
                Debug.Log("animation controller not found in Resources/Animation");
            }
        }

        var tileScript = tile.GetComponent<Tile>();
        tileScript.type = currentDrawingType;

        int column = brush.tileID / (int) map.mapSize.y;
        tileScript.index = new Vector2(column, brush.tileID - column * map.mapSize.y);
        
        var spriteRenderer = tile.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = brush.renderer2D.sprite;
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
