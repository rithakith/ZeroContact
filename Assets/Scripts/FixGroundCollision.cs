#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class FixGroundCollision : EditorWindow
{
    [MenuItem("Tools/Fix Ground Collision")]
    static void FixGround()
    {
        Debug.Log("=== FIXING GROUND COLLISION ===");
        
        // Find demo ground
        GameObject ground = GameObject.Find("DemoGround");
        if (ground == null)
        {
            Debug.LogError("No DemoGround found! Creating one...");
            CreateProperGround();
            return;
        }
        
        // Make sure it has a collider
        Collider groundCollider = ground.GetComponent<Collider>();
        Collider2D groundCollider2D = ground.GetComponent<Collider2D>();
        
        if (groundCollider == null && groundCollider2D == null)
        {
            Debug.Log("Ground has no collider! Adding BoxCollider2D...");
            BoxCollider2D box = ground.AddComponent<BoxCollider2D>();
            Debug.Log("✓ Added BoxCollider2D to ground");
        }
        
        // Check layer
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1)
        {
            ground.layer = groundLayer;
            Debug.Log("✓ Set ground to Ground layer");
        }
        else
        {
            // Try Default layer
            ground.layer = 0;
            Debug.Log("✓ Set ground to Default layer (Ground layer doesn't exist)");
        }
        
        // Make sure ground is at correct position
        ground.transform.position = new Vector3(-5, -2, 0);
        ground.transform.localScale = new Vector3(8, 0.5f, 1);
        
        Debug.Log($"Ground position: {ground.transform.position}");
        Debug.Log($"Ground scale: {ground.transform.localScale}");
        
        // Check player prefab
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null)
        {
            manager = GameObject.FindObjectOfType<ControlsTutorialManagerEnhanced>()?.GetComponent<ControlsTutorialManagerSimple>();
        }
        
        if (manager != null && manager.playerPrefab != null)
        {
            CheckPlayerCollision(manager.playerPrefab);
        }
        
        EditorUtility.DisplayDialog("Ground Fixed", 
            "Ground collision has been fixed!\n\n" +
            "The ground now has:\n" +
            "• BoxCollider2D component\n" +
            "• Proper position and scale\n\n" +
            "If player still falls through:\n" +
            "1. Check player has Collider2D\n" +
            "2. Check player Rigidbody2D settings", 
            "OK");
    }
    
    static void CreateProperGround()
    {
        // Create ground with 2D components
        GameObject ground = new GameObject("DemoGround");
        ground.transform.position = new Vector3(-5, -2, 0);
        ground.transform.localScale = new Vector3(8, 0.5f, 1);
        
        // Add sprite renderer for visibility
        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Create a simple white sprite
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        
        // Add 2D collider
        BoxCollider2D collider = ground.AddComponent<BoxCollider2D>();
        
        Debug.Log("✓ Created proper 2D ground with collider");
    }
    
    static void CheckPlayerCollision(GameObject playerPrefab)
    {
        Debug.Log("\n=== CHECKING PLAYER PREFAB ===");
        
        // Check for Rigidbody2D
        Rigidbody2D rb = playerPrefab.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Debug.Log($"✓ Rigidbody2D found");
            Debug.Log($"  - Gravity Scale: {rb.gravityScale}");
            Debug.Log($"  - Body Type: {rb.bodyType}");
            
            if (rb.bodyType != RigidbodyType2D.Dynamic)
            {
                Debug.LogWarning("⚠ Body Type is not Dynamic! Should be Dynamic for physics.");
            }
        }
        else
        {
            Debug.LogError("✗ No Rigidbody2D on player!");
        }
        
        // Check for Collider2D
        Collider2D col = playerPrefab.GetComponent<Collider2D>();
        if (col != null)
        {
            Debug.Log($"✓ Collider2D found: {col.GetType().Name}");
            Debug.Log($"  - Is Trigger: {col.isTrigger}");
            
            if (col.isTrigger)
            {
                Debug.LogWarning("⚠ Collider is a trigger! Should NOT be trigger for collision.");
            }
        }
        else
        {
            Debug.LogError("✗ No Collider2D on player!");
        }
        
        // Check child objects for colliders
        Collider2D[] childColliders = playerPrefab.GetComponentsInChildren<Collider2D>();
        Debug.Log($"Total colliders (including children): {childColliders.Length}");
    }
    
    [MenuItem("Tools/Create 2D Platform")]
    static void Create2DPlatform()
    {
        // Create a proper 2D platform at spawn point
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Quad);
        platform.name = "Demo2DPlatform";
        platform.transform.position = new Vector3(-5, -2, 0);
        platform.transform.localScale = new Vector3(10, 1, 1);
        platform.transform.rotation = Quaternion.identity;
        
        // Remove 3D collider, add 2D
        DestroyImmediate(platform.GetComponent<MeshCollider>());
        BoxCollider2D box2d = platform.AddComponent<BoxCollider2D>();
        
        // Set material color
        Renderer rend = platform.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        }
        
        Debug.Log("✓ Created 2D platform with BoxCollider2D");
        
        Selection.activeGameObject = platform;
    }
    
    [MenuItem("Tools/Test Spawn Height")]
    static void TestSpawnHeight()
    {
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null)
        {
            manager = GameObject.FindObjectOfType<ControlsTutorialManagerEnhanced>()?.GetComponent<ControlsTutorialManagerSimple>();
        }
        
        if (manager != null && manager.demoSpawnPoint != null)
        {
            Debug.Log($"Demo spawn point: {manager.demoSpawnPoint.position}");
            
            GameObject ground = GameObject.Find("DemoGround");
            if (ground != null)
            {
                Debug.Log($"Ground position: {ground.transform.position}");
                float distance = manager.demoSpawnPoint.position.y - (ground.transform.position.y + ground.transform.localScale.y/2);
                Debug.Log($"Distance from spawn to ground top: {distance}");
                
                if (distance > 5)
                {
                    Debug.LogWarning("Spawn point might be too high! Player will fall for a while.");
                    Debug.Log("Consider moving spawn point to (-5, -1, 0)");
                }
            }
        }
    }
}
#endif