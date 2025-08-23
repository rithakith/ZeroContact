#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AddDemoCamera : EditorWindow
{
    [MenuItem("Tools/Add Demo Camera to Scene")]
    static void AddCamera()
    {
        // Check if demo camera already exists
        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.name == "DemoCamera")
            {
                Debug.Log("Demo Camera already exists!");
                Selection.activeGameObject = cam.gameObject;
                return;
            }
        }
        
        // Create Demo Camera
        GameObject demoCameraObj = new GameObject("DemoCamera");
        Camera demoCamera = demoCameraObj.AddComponent<Camera>();
        
        // Configure camera for demo viewport
        demoCamera.clearFlags = CameraClearFlags.SolidColor;
        demoCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
        demoCamera.orthographic = true;
        demoCamera.orthographicSize = 3;
        
        // Position camera to view demo area
        demoCameraObj.transform.position = new Vector3(-5, 0, -10);
        
        // Set viewport rect to left side of screen
        demoCamera.rect = new Rect(0.05f, 0.2f, 0.43f, 0.5f);
        
        // Set camera depth
        demoCamera.depth = 1; // Render on top of main camera
        
        // Add URP camera component if needed
        System.Type urpCameraType = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");
        if (urpCameraType != null)
        {
            demoCameraObj.AddComponent(urpCameraType);
            Debug.Log("Added URP camera component");
        }
        
        // Start with camera disabled (will be enabled during demos)
        demoCameraObj.SetActive(false);
        
        // Link to ControlsTutorialManager
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager != null)
        {
            manager.demoCamera = demoCamera;
            EditorUtility.SetDirty(manager);
            Debug.Log("✓ Demo Camera linked to ControlsTutorialManager");
        }
        
        // Select the camera
        Selection.activeGameObject = demoCameraObj;
        
        Debug.Log("✓ Demo Camera created successfully!");
        Debug.Log("Position: (-5, 0, -10)");
        Debug.Log("Viewport: Left side of screen");
        Debug.Log("The camera will activate automatically during demo screens");
        
        EditorUtility.DisplayDialog("Demo Camera Added", 
            "Demo Camera has been added to the scene!\n\nIt will activate automatically on demo screens (3, 4, 6, 7, 8).\n\nDon't forget to save the scene!", 
            "OK");
    }
    
    [MenuItem("Tools/Fix Demo Setup Complete")]
    static void FixComplete()
    {
        Debug.Log("=== FIXING COMPLETE DEMO SETUP ===");
        
        // Step 1: Add Demo Camera if missing
        Camera demoCamera = null;
        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.name == "DemoCamera")
            {
                demoCamera = cam;
                break;
            }
        }
        
        if (demoCamera == null)
        {
            Debug.Log("Creating Demo Camera...");
            AddCamera();
            
            // Find it again after creation
            cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                if (cam.name == "DemoCamera")
                {
                    demoCamera = cam;
                    break;
                }
            }
        }
        
        // Step 2: Ensure spawn points exist
        GameObject demoSpawns = GameObject.Find("DemoSpawnPoints");
        if (demoSpawns == null)
        {
            Debug.Log("Creating Demo Spawn Points...");
            demoSpawns = new GameObject("DemoSpawnPoints");
            demoSpawns.transform.position = new Vector3(-5, 0, 0);
            
            GameObject playerSpawn = new GameObject("PlayerDemoSpawn");
            playerSpawn.transform.SetParent(demoSpawns.transform);
            playerSpawn.transform.position = new Vector3(-5, 0, 0);
            
            GameObject enemySpawn = new GameObject("EnemyDemoSpawn");
            enemySpawn.transform.SetParent(demoSpawns.transform);
            enemySpawn.transform.position = new Vector3(-2, 0, 0);
        }
        
        // Step 3: Create demo ground if missing
        GameObject ground = GameObject.Find("DemoGround");
        if (ground == null)
        {
            Debug.Log("Creating Demo Ground...");
            ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "DemoGround";
            ground.transform.position = new Vector3(-5, -2, 0);
            ground.transform.localScale = new Vector3(8, 0.5f, 1);
            
            // Set layer if exists
            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer != -1)
            {
                ground.layer = groundLayer;
            }
        }
        
        // Step 4: Link everything to manager
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager != null)
        {
            manager.demoCamera = demoCamera;
            
            Transform playerSpawn = demoSpawns.transform.Find("PlayerDemoSpawn");
            if (playerSpawn != null)
            {
                manager.demoSpawnPoint = playerSpawn;
            }
            
            Transform enemySpawn = demoSpawns.transform.Find("EnemyDemoSpawn");
            if (enemySpawn != null)
            {
                manager.enemySpawnPoint = enemySpawn;
            }
            
            EditorUtility.SetDirty(manager);
        }
        
        // Step 5: Try to auto-assign player prefab
        if (manager != null && manager.playerPrefab == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab Player");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith("Player.prefab") && path.Contains("Characters"))
                {
                    GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (playerPrefab != null)
                    {
                        manager.playerPrefab = playerPrefab;
                        EditorUtility.SetDirty(manager);
                        Debug.Log($"✓ Auto-assigned Player prefab from: {path}");
                        break;
                    }
                }
            }
        }
        
        Debug.Log("\n=== SETUP COMPLETE ===");
        Debug.Log("✓ Demo Camera created/verified");
        Debug.Log("✓ Demo spawn points created/verified");
        Debug.Log("✓ Demo ground created/verified");
        
        if (manager != null && manager.playerPrefab != null)
        {
            Debug.Log("✓ Player prefab assigned");
        }
        else
        {
            Debug.LogWarning("⚠ Still need to assign Player prefab manually!");
        }
        
        Debug.Log("\nDon't forget to SAVE THE SCENE!");
        
        EditorUtility.DisplayDialog("Demo Setup Complete", 
            "All demo components have been added!\n\n" +
            "✓ Demo Camera\n" +
            "✓ Spawn Points\n" +
            "✓ Demo Ground\n\n" +
            "Make sure to:\n" +
            "1. Assign Player prefab if not auto-assigned\n" +
            "2. Save the scene\n" +
            "3. Test screens 3, 4, 6, 7, 8 for demos", 
            "OK");
    }
}
#endif