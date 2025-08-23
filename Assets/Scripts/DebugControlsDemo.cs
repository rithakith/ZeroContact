#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class DebugControlsDemo : EditorWindow
{
    [MenuItem("Tools/Debug Controls Demo")]
    static void DebugDemo()
    {
        Debug.Log("=== CONTROLS DEMO DEBUG ===");
        
        // Find the manager
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        
        if (manager == null)
        {
            Debug.LogError("No ControlsTutorialManagerSimple found!");
            return;
        }
        
        // Check Player Prefab
        if (manager.playerPrefab == null)
        {
            Debug.LogError("❌ PLAYER PREFAB NOT ASSIGNED! This is why demos aren't working!");
            Debug.LogError("SOLUTION: Drag your Player prefab to the 'Player Prefab' field in ControlsTutorialManager");
            
            // Try to find and suggest the player prefab
            string[] guids = AssetDatabase.FindAssets("t:Prefab Player");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Player.prefab"))
                {
                    Debug.Log($"Found Player prefab at: {path}");
                    Debug.Log("Drag this to the Player Prefab field!");
                }
            }
        }
        else
        {
            Debug.Log($"✓ Player Prefab assigned: {manager.playerPrefab.name}");
        }
        
        // Check Demo Camera
        if (manager.demoCamera == null)
        {
            Debug.LogError("❌ Demo Camera not linked!");
        }
        else
        {
            Debug.Log($"✓ Demo Camera found: {manager.demoCamera.name}");
            Debug.Log($"  - Position: {manager.demoCamera.transform.position}");
            Debug.Log($"  - Active: {manager.demoCamera.gameObject.activeSelf}");
            Debug.Log($"  - Viewport Rect: {manager.demoCamera.rect}");
        }
        
        // Check Demo Spawn Point
        if (manager.demoSpawnPoint == null)
        {
            Debug.LogError("❌ Demo Spawn Point not linked!");
        }
        else
        {
            Debug.Log($"✓ Demo Spawn Point found at: {manager.demoSpawnPoint.position}");
        }
        
        // Check Demo Area
        if (manager.demoArea == null)
        {
            Debug.LogWarning("⚠ Demo Area not linked");
        }
        else
        {
            Debug.Log($"✓ Demo Area found: {manager.demoArea.name}");
            Debug.Log($"  - Active: {manager.demoArea.activeSelf}");
        }
        
        // Check Left Panel
        if (manager.leftPanel == null)
        {
            Debug.LogWarning("⚠ Left Panel not linked");
        }
        else
        {
            Debug.Log($"✓ Left Panel found: {manager.leftPanel.name}");
            Debug.Log($"  - Active: {manager.leftPanel.activeSelf}");
        }
        
        // Show current screen info
        var screens = manager.tutorialScreens;
        if (screens != null && screens.Length > 0)
        {
            Debug.Log($"\nTotal Screens: {screens.Length}");
            Debug.Log("Demo Screens:");
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i].hasDemo)
                {
                    Debug.Log($"  - Screen {i + 1}: {screens[i].title} ({screens[i].demoType})");
                }
            }
        }
        
        Debug.Log("\n=== QUICK FIX GUIDE ===");
        if (manager.playerPrefab == null)
        {
            Debug.LogError("1. ASSIGN PLAYER PREFAB - This is the main issue!");
        }
        Debug.Log("2. Make sure you're testing screens 3, 4, 6, 7, or 8 (these have demos)");
        Debug.Log("3. The demo camera should activate automatically on demo screens");
    }
    
    [MenuItem("Tools/Auto-Fix Player Prefab")]
    static void AutoFixPlayerPrefab()
    {
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        
        if (manager == null)
        {
            Debug.LogError("No ControlsTutorialManagerSimple found!");
            return;
        }
        
        if (manager.playerPrefab != null)
        {
            Debug.Log("Player prefab already assigned!");
            return;
        }
        
        // Try to find the player prefab
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
                    Debug.Log("Save the scene to keep this change!");
                    
                    EditorUtility.DisplayDialog("Player Prefab Fixed", 
                        "Player prefab has been assigned!\n\nDemos should now work on screens 3, 4, 6, 7, and 8.\n\nDon't forget to save the scene!", 
                        "OK");
                    return;
                }
            }
        }
        
        Debug.LogError("Could not find Player.prefab automatically. Please assign it manually.");
    }
    
    [MenuItem("Tools/Test Demo Camera")]
    static void TestDemoCamera()
    {
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
            Debug.LogError("No Demo Camera found!");
            return;
        }
        
        // Toggle camera to test
        demoCamera.gameObject.SetActive(!demoCamera.gameObject.activeSelf);
        Debug.Log($"Demo Camera is now: {(demoCamera.gameObject.activeSelf ? "ACTIVE" : "INACTIVE")}");
        
        if (demoCamera.gameObject.activeSelf)
        {
            Debug.Log($"Camera Position: {demoCamera.transform.position}");
            Debug.Log($"Camera Rect: {demoCamera.rect}");
            Debug.Log("You should see a small viewport on the left side of the screen");
        }
    }
}
#endif