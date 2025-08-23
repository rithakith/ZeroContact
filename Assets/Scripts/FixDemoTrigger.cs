#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class FixDemoTrigger : EditorWindow
{
    [MenuItem("Tools/Fix Demo Trigger Issue")]
    static void FixTrigger()
    {
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null)
        {
            Debug.LogError("No ControlsTutorialManagerSimple found!");
            return;
        }
        
        // Check current state
        Debug.Log("=== CHECKING DEMO TRIGGER SETUP ===");
        
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Not in play mode. Some checks require play mode.");
        }
        
        // Check if ShowScreen is being called properly
        Debug.Log($"Current screen index: {GetPrivateField<int>(manager, "currentScreenIndex")}");
        
        // Check if demo coroutine exists
        var demoCoroutine = GetPrivateField<Coroutine>(manager, "demoCoroutine");
        Debug.Log($"Demo coroutine active: {demoCoroutine != null}");
        
        // Check if StartDemo is being called
        var isDemoActive = GetPrivateField<bool>(manager, "isDemoActive");
        Debug.Log($"Is demo active: {isDemoActive}");
        
        Debug.Log("\n=== MANUAL TRIGGER TEST ===");
        Debug.Log("1. Make sure you're in Play Mode");
        Debug.Log("2. Navigate to screen 3 (Jump demo)");
        Debug.Log("3. Run Tools > Force Trigger Demo");
    }
    
    [MenuItem("Tools/Force Trigger Demo")]
    static void ForceTriggerDemo()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Must be in Play Mode!");
            return;
        }
        
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null) return;
        
        // Get current screen
        var screens = manager.tutorialScreens;
        var currentIndex = GetPrivateField<int>(manager, "currentScreenIndex");
        
        if (currentIndex >= 0 && currentIndex < screens.Length)
        {
            var currentScreen = screens[currentIndex];
            Debug.Log($"Current screen: {currentScreen.title}");
            Debug.Log($"Has demo: {currentScreen.hasDemo}");
            
            if (currentScreen.hasDemo)
            {
                Debug.Log($"Manually triggering {currentScreen.demoType} demo...");
                
                // Call StartDemo directly
                var startDemoMethod = manager.GetType().GetMethod("StartDemo", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (startDemoMethod != null)
                {
                    startDemoMethod.Invoke(manager, new object[] { currentScreen.demoType });
                    Debug.Log("✓ Demo triggered manually!");
                }
                
                // Also try to show panels
                if (manager.leftPanel != null)
                {
                    manager.leftPanel.SetActive(true);
                    Debug.Log("✓ Left panel activated");
                }
                
                if (manager.demoCamera != null)
                {
                    manager.demoCamera.gameObject.SetActive(true);
                    Debug.Log("✓ Demo camera activated");
                }
            }
            else
            {
                Debug.Log("This screen doesn't have a demo. Try screen 3, 4, 6, 7, or 8");
            }
        }
    }
    
    [MenuItem("Tools/Debug Current Screen")]
    static void DebugCurrentScreen()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Must be in Play Mode!");
            return;
        }
        
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null) return;
        
        var currentIndex = GetPrivateField<int>(manager, "currentScreenIndex");
        var screens = manager.tutorialScreens;
        
        Debug.Log($"=== CURRENT SCREEN DEBUG ===");
        Debug.Log($"Screen Index: {currentIndex + 1} of {screens.Length}");
        
        if (currentIndex >= 0 && currentIndex < screens.Length)
        {
            var screen = screens[currentIndex];
            Debug.Log($"Title: {screen.title}");
            Debug.Log($"Has Demo: {screen.hasDemo}");
            Debug.Log($"Demo Type: {screen.demoType}");
            
            // Check UI state
            Debug.Log($"\n=== UI STATE ===");
            Debug.Log($"Left Panel Active: {manager.leftPanel?.activeSelf}");
            Debug.Log($"Right Panel Active: {manager.rightPanel?.activeSelf}");
            Debug.Log($"Demo Area Active: {manager.demoArea?.activeSelf}");
            Debug.Log($"Demo Camera Active: {manager.demoCamera?.gameObject.activeSelf}");
            Debug.Log($"Content Text Active: {manager.contentText?.gameObject.activeSelf}");
            Debug.Log($"Demo Instruction Text Active: {manager.demoInstructionText?.gameObject.activeSelf}");
        }
    }
    
    static T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            return (T)field.GetValue(obj);
        }
        return default(T);
    }
    
    [MenuItem("Tools/Test Jump Demo Directly")]
    static void TestJumpDemoDirectly()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Must be in Play Mode!");
            return;
        }
        
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null) return;
        
        // Manually set screen to jump demo
        Debug.Log("Forcing Jump Demo screen...");
        
        // Enable demo UI
        if (manager.leftPanel != null) manager.leftPanel.SetActive(true);
        if (manager.rightPanel != null) manager.rightPanel.SetActive(true);
        if (manager.demoArea != null) manager.demoArea.SetActive(true);
        if (manager.demoCamera != null) manager.demoCamera.gameObject.SetActive(true);
        
        // Hide center content
        if (manager.contentText != null) manager.contentText.gameObject.SetActive(false);
        
        // Show demo instruction
        if (manager.demoInstructionText != null)
        {
            manager.demoInstructionText.gameObject.SetActive(true);
            manager.demoInstructionText.text = "Press Space to JUMP\n\nEssential for dodging ground attacks\nand navigating vertical obstacles.";
        }
        
        // Update title
        if (manager.titleText != null)
        {
            manager.titleText.text = "BASIC MOVEMENT - JUMPING";
        }
        
        // Start the demo
        manager.StartCoroutine(SpawnJumpDemo(manager));
    }
    
    static System.Collections.IEnumerator SpawnJumpDemo(ControlsTutorialManagerSimple manager)
    {
        if (manager.playerPrefab != null && manager.demoSpawnPoint != null)
        {
            Debug.Log("Spawning player for jump demo...");
            GameObject player = GameObject.Instantiate(manager.playerPrefab, manager.demoSpawnPoint.position, Quaternion.identity);
            player.name = "JUMP_DEMO_PLAYER";
            
            // Try to make it jump
            PlayerDemonstration demo = player.GetComponent<PlayerDemonstration>();
            if (demo == null)
            {
                demo = player.AddComponent<PlayerDemonstration>();
            }
            
            yield return new WaitForSeconds(0.5f);
            
            // Keep making it jump
            while (player != null)
            {
                if (demo != null)
                {
                    demo.DemoJump();
                }
                yield return new WaitForSeconds(2f);
            }
        }
        else
        {
            Debug.LogError("Missing player prefab or spawn point!");
        }
    }
}
#endif