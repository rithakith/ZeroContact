#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class FixMissingPanels : EditorWindow
{
    [MenuItem("Tools/Fix All Missing References")]
    static void FixAllReferences()
    {
        Debug.Log("=== FIXING ALL MISSING REFERENCES ===");
        
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null)
        {
            Debug.LogError("No ControlsTutorialManagerSimple found!");
            return;
        }
        
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found!");
            return;
        }
        
        int fixedCount = 0;
        
        // Find ScreenContainer
        if (manager.screenContainer == null)
        {
            Transform screenContainer = canvas.transform.Find("ScreenContainer");
            if (screenContainer != null)
            {
                manager.screenContainer = screenContainer.gameObject;
                Debug.Log("✓ Fixed: Screen Container");
                fixedCount++;
            }
        }
        
        Transform container = manager.screenContainer?.transform ?? canvas.transform.Find("ScreenContainer");
        if (container == null)
        {
            Debug.LogError("Can't find ScreenContainer!");
            return;
        }
        
        // Find Left Panel
        if (manager.leftPanel == null)
        {
            Transform leftPanel = container.Find("LeftPanel");
            if (leftPanel != null)
            {
                manager.leftPanel = leftPanel.gameObject;
                Debug.Log("✓ Fixed: Left Panel");
                fixedCount++;
            }
            else
            {
                Debug.LogWarning("✗ Left Panel not found - creating one");
                CreateLeftPanel(container, manager);
                fixedCount++;
            }
        }
        
        // Find Right Panel
        if (manager.rightPanel == null)
        {
            Transform rightPanel = container.Find("RightPanel");
            if (rightPanel != null)
            {
                manager.rightPanel = rightPanel.gameObject;
                Debug.Log("✓ Fixed: Right Panel");
                fixedCount++;
            }
            else
            {
                Debug.LogWarning("✗ Right Panel not found - creating one");
                CreateRightPanel(container, manager);
                fixedCount++;
            }
        }
        
        // Find Demo Area
        if (manager.demoArea == null && manager.leftPanel != null)
        {
            Transform demoArea = manager.leftPanel.transform.Find("DemoArea");
            if (demoArea != null)
            {
                manager.demoArea = demoArea.gameObject;
                Debug.Log("✓ Fixed: Demo Area");
                fixedCount++;
            }
            else
            {
                // Create demo area
                GameObject newDemoArea = new GameObject("DemoArea");
                newDemoArea.transform.SetParent(manager.leftPanel.transform, false);
                RectTransform demoRect = newDemoArea.AddComponent<RectTransform>();
                demoRect.anchorMin = new Vector2(0.1f, 0.1f);
                demoRect.anchorMax = new Vector2(0.9f, 0.9f);
                demoRect.offsetMin = Vector2.zero;
                demoRect.offsetMax = Vector2.zero;
                manager.demoArea = newDemoArea;
                Debug.Log("✓ Created: Demo Area");
                fixedCount++;
            }
        }
        
        // Find Demo Instruction Text
        if (manager.demoInstructionText == null && manager.rightPanel != null)
        {
            Transform demoInstText = manager.rightPanel.transform.Find("DemoInstructionText");
            if (demoInstText != null)
            {
                manager.demoInstructionText = demoInstText.GetComponent<TextMeshProUGUI>();
                Debug.Log("✓ Fixed: Demo Instruction Text");
                fixedCount++;
            }
            else
            {
                // Create demo instruction text
                GameObject textObj = new GameObject("DemoInstructionText");
                textObj.transform.SetParent(manager.rightPanel.transform, false);
                TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
                text.text = "Demo instructions will appear here";
                text.fontSize = 36;
                text.alignment = TextAlignmentOptions.Center;
                text.color = Color.white;
                
                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0.1f, 0.1f);
                textRect.anchorMax = new Vector2(0.9f, 0.9f);
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                manager.demoInstructionText = text;
                Debug.Log("✓ Created: Demo Instruction Text");
                fixedCount++;
            }
        }
        
        // Find other UI elements
        if (manager.titleText == null)
        {
            Transform titleText = container.Find("TitleText");
            if (titleText != null)
            {
                manager.titleText = titleText.GetComponent<TextMeshProUGUI>();
                Debug.Log("✓ Fixed: Title Text");
                fixedCount++;
            }
        }
        
        if (manager.contentText == null)
        {
            Transform contentText = container.Find("ContentText");
            if (contentText != null)
            {
                manager.contentText = contentText.GetComponent<TextMeshProUGUI>();
                Debug.Log("✓ Fixed: Content Text");
                fixedCount++;
            }
        }
        
        if (manager.nextButton == null)
        {
            Transform button = container.Find("NextButton");
            if (button != null)
            {
                manager.nextButton = button.GetComponent<Button>();
                Debug.Log("✓ Fixed: Next Button");
                fixedCount++;
            }
        }
        
        if (manager.buttonText == null && manager.nextButton != null)
        {
            Transform buttonText = manager.nextButton.transform.Find("ButtonText");
            if (buttonText != null)
            {
                manager.buttonText = buttonText.GetComponent<TextMeshProUGUI>();
                Debug.Log("✓ Fixed: Button Text");
                fixedCount++;
            }
        }
        
        // Make sure panels start inactive
        if (manager.leftPanel != null) manager.leftPanel.SetActive(false);
        if (manager.rightPanel != null) manager.rightPanel.SetActive(false);
        
        EditorUtility.SetDirty(manager);
        
        Debug.Log($"\n=== FIXED {fixedCount} REFERENCES ===");
        Debug.Log("Save the scene to keep these changes!");
        
        // Show current status
        ShowStatus(manager);
        
        EditorUtility.DisplayDialog("References Fixed", 
            $"Fixed {fixedCount} missing references!\n\nThe panels have been created/linked.\n\nDon't forget to:\n1. Save the scene\n2. Test in Play mode", 
            "OK");
    }
    
    static void CreateLeftPanel(Transform container, ControlsTutorialManagerSimple manager)
    {
        GameObject leftPanel = new GameObject("LeftPanel");
        leftPanel.transform.SetParent(container, false);
        RectTransform leftRect = leftPanel.AddComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0, 0.15f);
        leftRect.anchorMax = new Vector2(0.48f, 0.8f);
        leftRect.offsetMin = Vector2.zero;
        leftRect.offsetMax = Vector2.zero;
        
        Image leftBg = leftPanel.AddComponent<Image>();
        leftBg.color = new Color(0.1f, 0.1f, 0.15f, 0.3f);
        
        manager.leftPanel = leftPanel;
        leftPanel.SetActive(false);
    }
    
    static void CreateRightPanel(Transform container, ControlsTutorialManagerSimple manager)
    {
        GameObject rightPanel = new GameObject("RightPanel");
        rightPanel.transform.SetParent(container, false);
        RectTransform rightRect = rightPanel.AddComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(0.52f, 0.15f);
        rightRect.anchorMax = new Vector2(1, 0.8f);
        rightRect.offsetMin = Vector2.zero;
        rightRect.offsetMax = Vector2.zero;
        
        Image rightBg = rightPanel.AddComponent<Image>();
        rightBg.color = new Color(0.1f, 0.1f, 0.15f, 0.3f);
        
        manager.rightPanel = rightPanel;
        rightPanel.SetActive(false);
    }
    
    static void ShowStatus(ControlsTutorialManagerSimple manager)
    {
        Debug.Log("\n=== CURRENT STATUS ===");
        Debug.Log($"Screen Container: {(manager.screenContainer != null ? "✓" : "✗")}");
        Debug.Log($"Left Panel: {(manager.leftPanel != null ? "✓" : "✗")}");
        Debug.Log($"Right Panel: {(manager.rightPanel != null ? "✓" : "✗")}");
        Debug.Log($"Demo Area: {(manager.demoArea != null ? "✓" : "✗")}");
        Debug.Log($"Demo Instruction Text: {(manager.demoInstructionText != null ? "✓" : "✗")}");
        Debug.Log($"Title Text: {(manager.titleText != null ? "✓" : "✗")}");
        Debug.Log($"Content Text: {(manager.contentText != null ? "✓" : "✗")}");
        Debug.Log($"Next Button: {(manager.nextButton != null ? "✓" : "✗")}");
        Debug.Log($"Demo Camera: {(manager.demoCamera != null ? "✓" : "✗")}");
        Debug.Log($"Player Prefab: {(manager.playerPrefab != null ? "✓" : "✗")}");
    }
    
    [MenuItem("Tools/Show Tutorial Status")]
    static void ShowTutorialStatus()
    {
        ControlsTutorialManagerSimple manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager != null)
        {
            ShowStatus(manager);
        }
    }
}
#endif