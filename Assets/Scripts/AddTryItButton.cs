#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class AddTryItButton : EditorWindow
{
    [MenuItem("Tools/Add Try It Button to Controls")]
    static void AddButton()
    {
        Debug.Log("=== ADDING TRY IT BUTTON ===");
        
        // Find the manager
        ControlsTutorialManagerSimple oldManager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (oldManager == null)
        {
            Debug.LogError("No ControlsTutorialManagerSimple found!");
            return;
        }
        
        // Get references we need to preserve
        GameObject managerObj = oldManager.gameObject;
        var playerPrefab = oldManager.playerPrefab;
        var demoCamera = oldManager.demoCamera;
        var demoSpawnPoint = oldManager.demoSpawnPoint;
        var enemySpawnPoint = oldManager.enemySpawnPoint;
        var screenContainer = oldManager.screenContainer;
        var leftPanel = oldManager.leftPanel;
        var rightPanel = oldManager.rightPanel;
        var titleText = oldManager.titleText;
        var contentText = oldManager.contentText;
        var nextButton = oldManager.nextButton;
        var buttonText = oldManager.buttonText;
        var demoArea = oldManager.demoArea;
        var demoInstructionText = oldManager.demoInstructionText;
        var backgroundOverlay = oldManager.backgroundOverlay;
        
        // Remove old component
        DestroyImmediate(oldManager);
        
        // Add new enhanced component
        ControlsTutorialManagerEnhanced newManager = managerObj.AddComponent<ControlsTutorialManagerEnhanced>();
        
        // Transfer all references
        newManager.playerPrefab = playerPrefab;
        newManager.demoCamera = demoCamera;
        newManager.demoSpawnPoint = demoSpawnPoint;
        newManager.enemySpawnPoint = enemySpawnPoint;
        newManager.screenContainer = screenContainer;
        newManager.leftPanel = leftPanel;
        newManager.rightPanel = rightPanel;
        newManager.titleText = titleText;
        newManager.contentText = contentText;
        newManager.nextButton = nextButton;
        newManager.buttonText = buttonText;
        newManager.demoArea = demoArea;
        newManager.demoInstructionText = demoInstructionText;
        newManager.backgroundOverlay = backgroundOverlay;
        
        // Create Try It button
        if (screenContainer != null)
        {
            // Create button container
            GameObject buttonContainer = new GameObject("TryItButtonContainer");
            buttonContainer.transform.SetParent(screenContainer.transform, false);
            RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.7f, 0.02f);
            containerRect.anchorMax = new Vector2(0.95f, 0.12f);
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;
            
            // Create the button
            GameObject tryItButtonObj = new GameObject("TryItButton");
            tryItButtonObj.transform.SetParent(buttonContainer.transform, false);
            Image buttonImage = tryItButtonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.5f, 0.8f, 0.9f);
            
            Button tryItButton = tryItButtonObj.AddComponent<Button>();
            
            // Add button enhancer for hover effects
            ControlsButtonEnhancer enhancer = tryItButtonObj.AddComponent<ControlsButtonEnhancer>();
            enhancer.enableHoverScale = true;
            enhancer.hoverScale = 1.05f;
            enhancer.clickScale = 0.95f;
            enhancer.enableTextColorChange = true;
            enhancer.hoverTextColor = new Color(1f, 1f, 0.9f);
            
            RectTransform buttonRect = tryItButtonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = Vector2.zero;
            buttonRect.anchorMax = Vector2.one;
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            // Create button text
            GameObject textObj = new GameObject("TryItButtonText");
            textObj.transform.SetParent(tryItButtonObj.transform, false);
            TextMeshProUGUI tryItText = textObj.AddComponent<TextMeshProUGUI>();
            tryItText.text = "TRY IT MYSELF";
            tryItText.fontSize = 28;
            tryItText.alignment = TextAlignmentOptions.Center;
            tryItText.color = Color.white;
            
            // Try to use the same font as other text
            if (buttonText != null && buttonText.font != null)
            {
                tryItText.font = buttonText.font;
            }
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // No longer needed - always interactive mode
            // Just destroy the button we created
            DestroyImmediate(buttonContainer);
            
            Debug.Log("✓ Upgraded to enhanced manager (always interactive)");
        }
        
        EditorUtility.SetDirty(newManager);
        
        Debug.Log("\n=== UPGRADE COMPLETE ===");
        Debug.Log("The tutorial is now always interactive!");
        Debug.Log("- Players control the character directly");
        Debug.Log("- No automated demos");
        Debug.Log("- Use arrow keys/WASD to move, SPACE to jump");
        Debug.Log("\nDon't forget to save the scene!");
        
        EditorUtility.DisplayDialog("Upgraded to Interactive Mode", 
            "The controls tutorial has been upgraded!\n\n" +
            "Features:\n" +
            "• Always interactive - no demos\n" +
            "• Players control character directly\n" +
            "• Arrow keys/WASD to move\n" +
            "• SPACE to jump\n\n" +
            "Save the scene to keep these changes!", 
            "OK");
    }
    
    [MenuItem("Tools/Quick Fix Gravity")]
    static void FixGravity()
    {
        // Make sure project has proper gravity
        Physics2D.gravity = new Vector2(0, -9.81f);
        Debug.Log($"2D Gravity set to: {Physics2D.gravity}");
        
        // Check player prefab
        ControlsTutorialManagerEnhanced manager = GameObject.FindObjectOfType<ControlsTutorialManagerEnhanced>();
        if (manager == null)
        {
            manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>()?.GetComponent<ControlsTutorialManagerEnhanced>();
        }
        
        if (manager != null && manager.playerPrefab != null)
        {
            Rigidbody2D rb = manager.playerPrefab.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Debug.Log($"Player prefab gravity scale: {rb.gravityScale}");
                if (rb.gravityScale == 0)
                {
                    Debug.LogWarning("Player prefab has gravity scale 0! Should be around 1-2 for normal gravity.");
                }
            }
        }
        
        EditorUtility.DisplayDialog("Gravity Check", 
            $"2D Gravity: {Physics2D.gravity}\n\n" +
            "If player still floats, check the Rigidbody2D component on the Player prefab.\n" +
            "Gravity Scale should be 1 or 2 (not 0).", 
            "OK");
    }
}
#endif