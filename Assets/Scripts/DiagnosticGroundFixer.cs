using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public static class DiagnosticGroundFixer
{
    static DiagnosticGroundFixer()
    {
        Debug.Log("=== DiagnosticGroundFixer loaded ===");
        EditorApplication.delayCall += RunDiagnostics;
    }

    public static void RunDiagnostics()
    {
        Debug.Log("=== RUNNING GROUND DIAGNOSTICS ===");
        
        // Check if we're in the Controls scene
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.name != "MainMenu")
        {
            Debug.Log($"Not in MainMenu scene (current: {scene.name})");
            return;
        }

        // Find ground
        GameObject ground = GameObject.Find("DemoGround");
        if (ground == null)
        {
            Debug.LogWarning("DemoGround not found in scene!");
            return;
        }

        // Check components
        Debug.Log($"DemoGround found at position: {ground.transform.position}");
        
        var collider3D = ground.GetComponent<Collider>();
        var collider2D = ground.GetComponent<Collider2D>();
        
        if (collider3D != null)
        {
            Debug.LogError($"DemoGround has 3D collider: {collider3D.GetType().Name} - This won't work with 2D player!");
            Debug.Log("Fix: Remove the 3D collider and add BoxCollider2D");
        }
        
        if (collider2D != null)
        {
            Debug.Log($"✓ DemoGround has 2D collider: {collider2D.GetType().Name}");
        }
        
        if (collider3D == null && collider2D == null)
        {
            Debug.LogError("DemoGround has NO collider!");
        }

        // Check player prefab
        var manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager != null && manager.playerPrefab != null)
        {
            Debug.Log("\n=== PLAYER PREFAB CHECK ===");
            var rb2d = manager.playerPrefab.GetComponent<Rigidbody2D>();
            var col2d = manager.playerPrefab.GetComponent<Collider2D>();
            
            if (rb2d != null)
            {
                Debug.Log($"✓ Player has Rigidbody2D (Gravity: {rb2d.gravityScale})");
            }
            else
            {
                Debug.LogError("Player missing Rigidbody2D!");
            }
            
            if (col2d != null)
            {
                Debug.Log($"✓ Player has Collider2D: {col2d.GetType().Name}");
            }
            else
            {
                Debug.LogError("Player missing Collider2D!");
            }
        }
    }
}

// Component to add to ground for automatic fixing
public class GroundAutoFixer : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("GroundAutoFixer running...");
        
        // Remove any 3D colliders
        Collider col3D = GetComponent<Collider>();
        if (col3D != null)
        {
            Debug.Log("Removing 3D collider from ground...");
            Destroy(col3D);
        }
        
        // Ensure 2D collider exists
        Collider2D col2D = GetComponent<Collider2D>();
        if (col2D == null)
        {
            Debug.Log("Adding BoxCollider2D to ground...");
            gameObject.AddComponent<BoxCollider2D>();
        }
    }
}

// Alternative menu location
public class GroundFixerWindow : EditorWindow
{
    [MenuItem("Window/Ground Fixer")]
    static void ShowWindow()
    {
        GetWindow<GroundFixerWindow>("Ground Fixer");
    }

    void OnGUI()
    {
        GUILayout.Label("Ground Collision Fixer", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Fix Ground Collision"))
        {
            FixGroundCollision();
        }
        
        if (GUILayout.Button("Create 2D Ground"))
        {
            Create2DGround();
        }
        
        if (GUILayout.Button("Check Setup"))
        {
            DiagnosticGroundFixer.RunDiagnostics();
        }
    }

    void FixGroundCollision()
    {
        GameObject ground = GameObject.Find("DemoGround");
        if (ground == null)
        {
            EditorUtility.DisplayDialog("Error", "DemoGround not found!", "OK");
            return;
        }

        // Remove 3D collider
        Collider col3D = ground.GetComponent<Collider>();
        if (col3D != null)
        {
            DestroyImmediate(col3D);
        }

        // Add 2D collider
        if (ground.GetComponent<Collider2D>() == null)
        {
            ground.AddComponent<BoxCollider2D>();
        }

        EditorUtility.DisplayDialog("Success", "Ground collision fixed!", "OK");
    }

    void Create2DGround()
    {
        GameObject ground = GameObject.Find("DemoGround");
        if (ground != null)
        {
            DestroyImmediate(ground);
        }

        ground = new GameObject("DemoGround");
        ground.transform.position = new Vector3(-5, -2, 0);
        
        // Add sprite renderer
        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Create sprite
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        
        // Scale it
        ground.transform.localScale = new Vector3(8, 0.5f, 1);
        
        // Add 2D collider
        ground.AddComponent<BoxCollider2D>();
        
        Selection.activeGameObject = ground;
        EditorUtility.DisplayDialog("Success", "2D ground created!", "OK");
    }
}
#endif