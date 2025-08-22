#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SimpleGroundFix : EditorWindow
{
    [MenuItem("Tools/Simple Ground Fix")]
    static void FixGround()
    {
        Debug.Log("=== FIXING GROUND ===");
        
        // Find and fix ground
        GameObject ground = GameObject.Find("DemoGround");
        if (ground != null)
        {
            // Remove any 3D colliders
            Collider col3D = ground.GetComponent<Collider>();
            if (col3D != null)
            {
                DestroyImmediate(col3D);
                Debug.Log("Removed 3D collider");
            }
            
            // Add 2D collider if missing
            BoxCollider2D col2D = ground.GetComponent<BoxCollider2D>();
            if (col2D == null)
            {
                col2D = ground.AddComponent<BoxCollider2D>();
                Debug.Log("Added BoxCollider2D");
            }
            
            // Make sure it's positioned correctly
            ground.transform.position = new Vector3(-5, -2, 0);
            
            Debug.Log("Ground fixed!");
        }
        else
        {
            Debug.LogError("No DemoGround found!");
        }
    }
    
    [MenuItem("Tools/Create New Demo Ground")]
    static void CreateGround()
    {
        // Delete old ground if exists
        GameObject oldGround = GameObject.Find("DemoGround");
        if (oldGround != null)
        {
            DestroyImmediate(oldGround);
        }
        
        // Create new ground
        GameObject ground = new GameObject("DemoGround");
        ground.transform.position = new Vector3(-5, -2, 0);
        
        // Add sprite renderer
        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        
        // Create a simple sprite
        Texture2D tex = new Texture2D(100, 10);
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                tex.SetPixel(x, y, Color.gray);
            }
        }
        tex.Apply();
        
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, 100, 10), new Vector2(0.5f, 0.5f), 10);
        sr.sprite = sprite;
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = new Vector2(10, 1);
        
        // Add 2D collider
        BoxCollider2D col = ground.AddComponent<BoxCollider2D>();
        col.size = new Vector2(10, 1);
        
        Debug.Log("Created new 2D ground!");
        Selection.activeGameObject = ground;
    }
    
    [MenuItem("Tools/Move Spawn Point Lower")]
    static void MoveSpawnLower()
    {
        // Find spawn point
        GameObject spawn = GameObject.Find("PlayerDemoSpawn");
        if (spawn != null)
        {
            spawn.transform.position = new Vector3(-5, -0.5f, 0);
            Debug.Log("Moved spawn point to (-5, -0.5, 0)");
        }
        else
        {
            Debug.LogError("No PlayerDemoSpawn found!");
        }
    }
}
#endif