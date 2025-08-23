#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class FixDemoCameraComplete : EditorWindow
{
    [MenuItem("Tools/Fix Demo Camera Complete")]
    static void FixDemoCamera()
    {
        Debug.Log("=== FIXING DEMO CAMERA ===");
        
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null)
        {
            Debug.LogError("No ControlsTutorialManagerSimple found!");
            return;
        }
        
        // Find or create demo camera
        Camera demoCamera = GameObject.Find("DemoCamera")?.GetComponent<Camera>();
        
        if (demoCamera == null)
        {
            Debug.Log("Creating new Demo Camera...");
            
            // Create Demo Camera
            GameObject demoCameraObj = new GameObject("DemoCamera");
            demoCamera = demoCameraObj.AddComponent<Camera>();
            
            // Configure camera for demo viewport
            demoCamera.clearFlags = CameraClearFlags.SolidColor;
            demoCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
            demoCamera.orthographic = true;
            demoCamera.orthographicSize = 3;
            demoCamera.cullingMask = -1; // See all layers
            demoCamera.depth = 1; // Render on top
            
            // Position camera to view demo area
            demoCameraObj.transform.position = new Vector3(-5, 0, -10);
            
            // IMPORTANT: Set camera to render to texture or specific viewport
            // For split screen, we'll use viewport rect
            demoCamera.rect = new Rect(0.05f, 0.2f, 0.43f, 0.5f);
            
            Debug.Log("✓ Created Demo Camera");
        }
        else
        {
            Debug.Log("✓ Found existing Demo Camera");
            // Make sure it has correct settings
            demoCamera.orthographic = true;
            demoCamera.orthographicSize = 3;
            demoCamera.cullingMask = -1;
            demoCamera.clearFlags = CameraClearFlags.SolidColor;
            demoCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
        }
        
        // Assign to manager
        manager.demoCamera = demoCamera;
        Debug.Log("✓ Assigned Demo Camera to manager");
        
        // Make sure camera starts disabled (will be enabled during demos)
        demoCamera.gameObject.SetActive(false);
        
        // Also ensure demo spawn point exists
        if (manager.demoSpawnPoint == null)
        {
            GameObject demoSpawns = GameObject.Find("DemoSpawnPoints");
            if (demoSpawns == null)
            {
                demoSpawns = new GameObject("DemoSpawnPoints");
                demoSpawns.transform.position = new Vector3(-5, 0, 0);
            }
            
            Transform playerSpawn = demoSpawns.transform.Find("PlayerDemoSpawn");
            if (playerSpawn == null)
            {
                GameObject spawn = new GameObject("PlayerDemoSpawn");
                spawn.transform.SetParent(demoSpawns.transform);
                spawn.transform.position = new Vector3(-5, 0, 0);
                playerSpawn = spawn.transform;
            }
            
            manager.demoSpawnPoint = playerSpawn;
            Debug.Log("✓ Created/assigned demo spawn point");
        }
        
        // Create a visual ground for demos
        GameObject ground = GameObject.Find("DemoGround");
        if (ground == null)
        {
            ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "DemoGround";
            ground.transform.position = new Vector3(-5, -2, 0);
            ground.transform.localScale = new Vector3(8, 0.5f, 1);
            
            // Make it visible
            Renderer renderer = ground.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            }
            
            Debug.Log("✓ Created demo ground");
        }
        
        EditorUtility.SetDirty(manager);
        
        Debug.Log("\n=== CAMERA SETUP COMPLETE ===");
        Debug.Log($"Camera Position: {demoCamera.transform.position}");
        Debug.Log($"Camera Viewport: {demoCamera.rect}");
        Debug.Log($"Spawn Point: {manager.demoSpawnPoint?.position}");
        Debug.Log("\nThe camera will render to the left panel area during demos.");
        Debug.Log("IMPORTANT: Save the scene now!");
        
        EditorUtility.DisplayDialog("Demo Camera Fixed", 
            "Demo camera has been created and configured!\n\n" +
            "The camera will show demos in the left panel.\n\n" +
            "Don't forget to:\n" +
            "1. Save the scene\n" +
            "2. Make sure Player Prefab is assigned\n" +
            "3. Test in Play mode on screen 3", 
            "OK");
    }
    
    [MenuItem("Tools/Test Demo Camera View")]
    static void TestCameraView()
    {
        Camera demoCamera = GameObject.Find("DemoCamera")?.GetComponent<Camera>();
        if (demoCamera == null)
        {
            Debug.LogError("No Demo Camera found! Run 'Fix Demo Camera Complete' first.");
            return;
        }
        
        // Toggle camera to see what it sees
        bool wasActive = demoCamera.gameObject.activeSelf;
        demoCamera.gameObject.SetActive(!wasActive);
        
        if (!wasActive)
        {
            // Camera is now on - create a test object
            GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testCube.name = "TEST_CUBE_DELETE_ME";
            testCube.transform.position = new Vector3(-5, 0, 0);
            testCube.transform.localScale = Vector3.one * 0.5f;
            
            // Make it red
            Renderer renderer = testCube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }
            
            Debug.Log("Demo Camera ON - You should see:");
            Debug.Log("- A viewport on the LEFT side of Game view");
            Debug.Log("- A red cube in that viewport");
            Debug.Log("- Dark background");
            Debug.Log("\nRun this command again to turn off.");
        }
        else
        {
            // Camera is now off - clean up test objects
            GameObject testCube = GameObject.Find("TEST_CUBE_DELETE_ME");
            if (testCube != null)
            {
                DestroyImmediate(testCube);
            }
            
            Debug.Log("Demo Camera OFF");
        }
    }
    
    [MenuItem("Tools/Complete Demo Setup Check")]
    static void CompleteDemoCheck()
    {
        Debug.Log("=== COMPLETE DEMO SETUP CHECK ===");
        
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null)
        {
            Debug.LogError("✗ No ControlsTutorialManagerSimple found!");
            return;
        }
        
        bool allGood = true;
        
        // Check all required components
        if (manager.playerPrefab == null)
        {
            Debug.LogError("✗ Player Prefab NOT assigned!");
            allGood = false;
        }
        else
        {
            Debug.Log("✓ Player Prefab assigned");
        }
        
        if (manager.demoCamera == null)
        {
            Debug.LogError("✗ Demo Camera NOT assigned!");
            allGood = false;
        }
        else
        {
            Debug.Log("✓ Demo Camera assigned");
            Debug.Log($"  Position: {manager.demoCamera.transform.position}");
            Debug.Log($"  Viewport: {manager.demoCamera.rect}");
        }
        
        if (manager.demoSpawnPoint == null)
        {
            Debug.LogError("✗ Demo Spawn Point NOT assigned!");
            allGood = false;
        }
        else
        {
            Debug.Log("✓ Demo Spawn Point assigned");
            Debug.Log($"  Position: {manager.demoSpawnPoint.position}");
        }
        
        if (manager.leftPanel == null)
        {
            Debug.LogError("✗ Left Panel NOT assigned!");
            allGood = false;
        }
        else
        {
            Debug.Log("✓ Left Panel assigned");
        }
        
        if (manager.rightPanel == null)
        {
            Debug.LogError("✗ Right Panel NOT assigned!");
            allGood = false;
        }
        else
        {
            Debug.Log("✓ Right Panel assigned");
        }
        
        if (allGood)
        {
            Debug.Log("\n✅ ALL COMPONENTS ARE SET UP!");
            Debug.Log("Demos should work properly now.");
            Debug.Log("Test by playing the scene and going to screen 3.");
        }
        else
        {
            Debug.LogError("\n❌ SOME COMPONENTS ARE MISSING!");
            Debug.Log("Run the appropriate fix tools for the missing components.");
        }
    }
}
#endif