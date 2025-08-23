#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class DiagnoseDemoIssue : EditorWindow
{
    [MenuItem("Tools/Diagnose Demo Issue")]
    static void DiagnoseIssue()
    {
        Debug.Log("=== DEMO DIAGNOSTIC TEST ===");
        
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null)
        {
            Debug.LogError("No ControlsTutorialManagerSimple found!");
            return;
        }
        
        // Force show screen 3 (Jump demo)
        Debug.Log("Forcing screen 3 (Jump Demo) for testing...");
        
        // Make sure we're in play mode
        if (!Application.isPlaying)
        {
            Debug.LogError("You must be in PLAY MODE to test demos!");
            Debug.Log("1. Enter Play Mode");
            Debug.Log("2. Run this diagnostic again");
            return;
        }
        
        // Get current screen info
        var screens = manager.tutorialScreens;
        if (screens != null && screens.Length > 2)
        {
            var jumpScreen = screens[2]; // Screen 3 (index 2)
            Debug.Log($"Screen 3 info:");
            Debug.Log($"  Title: {jumpScreen.title}");
            Debug.Log($"  Has Demo: {jumpScreen.hasDemo}");
            Debug.Log($"  Demo Type: {jumpScreen.demoType}");
        }
        
        // Check all components
        Debug.Log("\n=== Component Status ===");
        Debug.Log($"Player Prefab: {(manager.playerPrefab != null ? "✓ " + manager.playerPrefab.name : "✗")}");
        Debug.Log($"Demo Camera: {(manager.demoCamera != null ? "✓" : "✗")}");
        Debug.Log($"Demo Spawn Point: {(manager.demoSpawnPoint != null ? "✓ at " + manager.demoSpawnPoint.position : "✗")}");
        Debug.Log($"Left Panel: {(manager.leftPanel != null ? "✓ Active: " + manager.leftPanel.activeSelf : "✗")}");
        Debug.Log($"Demo Area: {(manager.demoArea != null ? "✓ Active: " + manager.demoArea.activeSelf : "✗")}");
        
        if (manager.demoCamera != null)
        {
            Debug.Log($"\n=== Demo Camera Info ===");
            Debug.Log($"Active: {manager.demoCamera.gameObject.activeSelf}");
            Debug.Log($"Position: {manager.demoCamera.transform.position}");
            Debug.Log($"Viewport Rect: {manager.demoCamera.rect}");
            Debug.Log($"Culling Mask: {manager.demoCamera.cullingMask}");
            Debug.Log($"Depth: {manager.demoCamera.depth}");
        }
    }
    
    [MenuItem("Tools/Force Test Demo Spawn")]
    static void ForceTestDemo()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Must be in PLAY MODE!");
            return;
        }
        
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null || manager.playerPrefab == null || manager.demoSpawnPoint == null)
        {
            Debug.LogError("Missing required components!");
            return;
        }
        
        // Force spawn a player
        Debug.Log("Force spawning player at demo point...");
        GameObject testPlayer = GameObject.Instantiate(manager.playerPrefab, manager.demoSpawnPoint.position, Quaternion.identity);
        testPlayer.name = "TEST_DEMO_PLAYER";
        
        Debug.Log($"✓ Spawned player at: {testPlayer.transform.position}");
        Debug.Log($"Player scale: {testPlayer.transform.localScale}");
        Debug.Log($"Player active: {testPlayer.activeSelf}");
        
        // Check if it has a renderer
        SpriteRenderer sr = testPlayer.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Debug.Log($"Sprite Renderer found: {sr.sprite?.name}");
            Debug.Log($"Sorting Layer: {sr.sortingLayerName}");
            Debug.Log($"Order in Layer: {sr.sortingOrder}");
        }
        else
        {
            Debug.LogError("No SpriteRenderer found on player!");
        }
        
        // Force enable demo camera
        if (manager.demoCamera != null)
        {
            manager.demoCamera.gameObject.SetActive(true);
            Debug.Log("✓ Demo camera activated");
            
            // Make sure camera can see the player
            float distance = Vector3.Distance(manager.demoCamera.transform.position, testPlayer.transform.position);
            Debug.Log($"Distance from camera to player: {distance}");
            
            if (distance > 20f)
            {
                Debug.LogWarning("Camera might be too far from player!");
            }
        }
        
        Debug.Log("\n=== WHAT YOU SHOULD SEE ===");
        Debug.Log("1. A small viewport on the LEFT side of screen");
        Debug.Log("2. The player character visible in that viewport");
        Debug.Log("3. If you don't see it, check the Game view (not Scene view)");
    }
    
    [MenuItem("Tools/Fix Demo Visibility")]
    static void FixDemoVisibility()
    {
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null) return;
        
        // Make sure demo camera has correct settings
        if (manager.demoCamera != null)
        {
            // Fix culling mask - should see everything
            manager.demoCamera.cullingMask = -1; // All layers
            
            // Make sure depth is higher than main camera
            manager.demoCamera.depth = 1;
            
            // Fix clear flags
            manager.demoCamera.clearFlags = CameraClearFlags.SolidColor;
            manager.demoCamera.backgroundColor = new Color(0.2f, 0.2f, 0.3f, 1f);
            
            Debug.Log("✓ Fixed demo camera settings");
        }
        
        // Make sure panels are set up correctly
        if (manager.leftPanel != null)
        {
            RectTransform rect = manager.leftPanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                // Make sure it's visible
                CanvasGroup group = manager.leftPanel.GetComponent<CanvasGroup>();
                if (group == null)
                {
                    group = manager.leftPanel.AddComponent<CanvasGroup>();
                }
                group.alpha = 1f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }
        
        EditorUtility.SetDirty(manager);
        Debug.Log("Applied visibility fixes. Save the scene!");
    }
    
    [MenuItem("Tools/Simple Demo Test")]
    static void SimpleDemoTest()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            Debug.Log("Starting play mode... Run this command again once in play mode.");
            return;
        }
        
        // Create a simple test setup
        Debug.Log("Creating simple demo test...");
        
        // Find or create a camera for the demo
        Camera demoCamera = GameObject.Find("DemoCamera")?.GetComponent<Camera>();
        if (demoCamera == null)
        {
            GameObject camObj = new GameObject("SimpleDemoCamera");
            demoCamera = camObj.AddComponent<Camera>();
            demoCamera.orthographic = true;
            demoCamera.orthographicSize = 5;
            demoCamera.transform.position = new Vector3(0, 0, -10);
            demoCamera.clearFlags = CameraClearFlags.SolidColor;
            demoCamera.backgroundColor = Color.blue;
            demoCamera.rect = new Rect(0, 0, 0.5f, 0.5f); // Left half of screen
            Debug.Log("Created simple demo camera");
        }
        
        // Create a simple sprite
        GameObject testObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testObj.name = "DEMO_TEST_CUBE";
        testObj.transform.position = Vector3.zero;
        
        Debug.Log("You should see:");
        Debug.Log("- A blue background on the LEFT half of the screen");
        Debug.Log("- A white cube in that area");
        Debug.Log("If you see this, the camera viewport is working!");
    }
}
#endif