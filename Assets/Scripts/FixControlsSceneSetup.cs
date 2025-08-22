#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class FixControlsSceneSetup : EditorWindow
{
    [MenuItem("Tools/Fix Controls Scene Setup")]
    static void FixSetup()
    {
        // Find the ControlsTutorialManager GameObject
        GameObject managerObj = GameObject.Find("ControlsTutorialManager");
        
        if (managerObj == null)
        {
            Debug.LogError("Can't find ControlsTutorialManager GameObject! Make sure you're in the Controls scene.");
            return;
        }
        
        // Check if it has the correct component
        ControlsTutorialManagerSimple manager = managerObj.GetComponent<ControlsTutorialManagerSimple>();
        
        if (manager == null)
        {
            Debug.Log("Adding ControlsTutorialManagerSimple component...");
            manager = managerObj.AddComponent<ControlsTutorialManagerSimple>();
        }
        
        // Now find and link all the UI elements
        Debug.Log("Linking UI elements...");
        
        // Find Canvas
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene!");
            return;
        }
        
        // Find ScreenContainer
        Transform screenContainer = canvas.transform.Find("ScreenContainer");
        if (screenContainer != null)
        {
            manager.screenContainer = screenContainer.gameObject;
            
            // Find Title Text
            Transform titleTransform = screenContainer.Find("TitleText");
            if (titleTransform != null)
            {
                manager.titleText = titleTransform.GetComponent<TextMeshProUGUI>();
                Debug.Log("✓ Title Text linked");
            }
            
            // Find Content Text
            Transform contentTransform = screenContainer.Find("ContentText");
            if (contentTransform != null)
            {
                manager.contentText = contentTransform.GetComponent<TextMeshProUGUI>();
                Debug.Log("✓ Content Text linked");
            }
            
            // Find Next Button
            Transform buttonTransform = screenContainer.Find("NextButton");
            if (buttonTransform != null)
            {
                manager.nextButton = buttonTransform.GetComponent<Button>();
                
                // Find button text
                Transform buttonTextTransform = buttonTransform.Find("ButtonText");
                if (buttonTextTransform != null)
                {
                    manager.buttonText = buttonTextTransform.GetComponent<TextMeshProUGUI>();
                    Debug.Log("✓ Button and Button Text linked");
                }
            }
            
            // Find Left Panel
            Transform leftPanel = screenContainer.Find("LeftPanel");
            if (leftPanel != null)
            {
                manager.leftPanel = leftPanel.gameObject;
                
                // Find Demo Area
                Transform demoArea = leftPanel.Find("DemoArea");
                if (demoArea != null)
                {
                    manager.demoArea = demoArea.gameObject;
                    Debug.Log("✓ Left Panel and Demo Area linked");
                }
            }
            
            // Find Right Panel
            Transform rightPanel = screenContainer.Find("RightPanel");
            if (rightPanel != null)
            {
                manager.rightPanel = rightPanel.gameObject;
                
                // Find Demo Instruction Text
                Transform demoInstructionTransform = rightPanel.Find("DemoInstructionText");
                if (demoInstructionTransform != null)
                {
                    manager.demoInstructionText = demoInstructionTransform.GetComponent<TextMeshProUGUI>();
                    Debug.Log("✓ Right Panel and Demo Instruction Text linked");
                }
            }
        }
        
        // Find Background
        Transform bgTransform = canvas.transform.Find("Background");
        if (bgTransform != null)
        {
            manager.backgroundOverlay = bgTransform.GetComponent<Image>();
            Debug.Log("✓ Background linked");
        }
        
        // Find Demo Camera
        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.name == "DemoCamera")
            {
                manager.demoCamera = cam;
                Debug.Log("✓ Demo Camera linked");
                break;
            }
        }
        
        // Find Demo Spawn Points
        GameObject demoSpawns = GameObject.Find("DemoSpawnPoints");
        if (demoSpawns != null)
        {
            Transform playerSpawn = demoSpawns.transform.Find("PlayerDemoSpawn");
            if (playerSpawn != null)
            {
                manager.demoSpawnPoint = playerSpawn;
                Debug.Log("✓ Player Demo Spawn linked");
            }
            
            Transform enemySpawn = demoSpawns.transform.Find("EnemyDemoSpawn");
            if (enemySpawn != null)
            {
                manager.enemySpawnPoint = enemySpawn;
                Debug.Log("✓ Enemy Demo Spawn linked");
            }
        }
        
        // Mark the object as dirty so changes are saved
        EditorUtility.SetDirty(manager);
        
        Debug.Log("Setup complete! Don't forget to:");
        Debug.Log("1. Assign your Player prefab to the 'Player Prefab' field");
        Debug.Log("2. Save the scene");
        
        EditorUtility.DisplayDialog("Setup Complete", 
            "Controls scene setup is fixed!\n\nDon't forget to:\n1. Assign Player prefab\n2. Save the scene\n3. Test in Play mode", 
            "OK");
    }
    
    [MenuItem("Tools/Quick Test Controls Scene")]
    static void QuickTest()
    {
        // Find the manager
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        
        if (manager == null)
        {
            Debug.LogError("No ControlsTutorialManagerSimple found! Run 'Fix Controls Scene Setup' first.");
            return;
        }
        
        // Log current state
        Debug.Log("=== Controls Scene Status ===");
        Debug.Log($"Screen Container: {(manager.screenContainer != null ? "✓" : "✗")}");
        Debug.Log($"Title Text: {(manager.titleText != null ? "✓" : "✗")}");
        Debug.Log($"Content Text: {(manager.contentText != null ? "✓" : "✗")}");
        Debug.Log($"Next Button: {(manager.nextButton != null ? "✓" : "✗")}");
        Debug.Log($"Button Text: {(manager.buttonText != null ? "✓" : "✗")}");
        Debug.Log($"Demo Area: {(manager.demoArea != null ? "✓" : "✗")}");
        Debug.Log($"Left Panel: {(manager.leftPanel != null ? "✓" : "✗")}");
        Debug.Log($"Right Panel: {(manager.rightPanel != null ? "✓" : "✗")}");
        Debug.Log($"Demo Instruction Text: {(manager.demoInstructionText != null ? "✓" : "✗")}");
        Debug.Log($"Demo Camera: {(manager.demoCamera != null ? "✓" : "✗")}");
        Debug.Log($"Player Prefab: {(manager.playerPrefab != null ? "✓ ASSIGNED" : "✗ NOT ASSIGNED - ASSIGN THIS!")}");
        Debug.Log($"Demo Spawn Point: {(manager.demoSpawnPoint != null ? "✓" : "✗")}");
        
        if (manager.titleText != null)
        {
            Debug.Log($"Current Title: {manager.titleText.text}");
        }
        
        if (manager.contentText != null)
        {
            Debug.Log($"Current Content: {manager.contentText.text}");
        }
    }
}
#endif