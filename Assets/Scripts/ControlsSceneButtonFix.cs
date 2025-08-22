using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class ControlsSceneButtonFix : MonoBehaviour
{
    [Header("Diagnostic Mode")]
    public bool runDiagnostics = true;
    public bool autoFix = true;
    
    void Start()
    {
        StartCoroutine(DiagnoseAndFix());
    }
    
    IEnumerator DiagnoseAndFix()
    {
        yield return new WaitForSeconds(0.1f); // Wait for scene to initialize
        
        Debug.Log("=== CONTROLS SCENE BUTTON DIAGNOSTICS ===");
        
        // 1. Check EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ No EventSystem found! UI won't work without it.");
            if (autoFix)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
                Debug.Log("✅ Created EventSystem");
            }
        }
        else
        {
            Debug.Log("✅ EventSystem found");
        }
        
        // 2. Check Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ No Canvas found!");
            yield break;
        }
        
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogError("❌ Canvas missing GraphicRaycaster!");
            if (autoFix)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log("✅ Added GraphicRaycaster to Canvas");
            }
        }
        else
        {
            Debug.Log("✅ Canvas has GraphicRaycaster");
        }
        
        // 3. Check Buttons
        Button nextButton = GameObject.Find("NextButton")?.GetComponent<Button>();
        Button backButton = GameObject.Find("BackButton")?.GetComponent<Button>();
        Button menuButton = GameObject.Find("MenuButton")?.GetComponent<Button>();
        
        CheckButton("NextButton", nextButton);
        CheckButton("BackButton", backButton);
        CheckButton("MenuButton", menuButton);
        
        // 4. Check Tutorial Controller
        ControlsTutorial tutorial = FindObjectOfType<ControlsTutorial>();
        if (tutorial == null)
        {
            Debug.LogError("❌ No ControlsTutorial found!");
            if (autoFix)
            {
                GameObject tutorialObj = new GameObject("TutorialController");
                tutorial = tutorialObj.AddComponent<ControlsTutorial>();
                Debug.Log("✅ Created ControlsTutorial");
            }
        }
        else
        {
            Debug.Log("✅ ControlsTutorial found");
        }
        
        // 5. Fix button connections
        if (autoFix && tutorial != null)
        {
            Debug.Log("\n=== FIXING BUTTON CONNECTIONS ===");
            
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(() => {
                    Debug.Log("Next button clicked via fix!");
                    tutorial.NextStep();
                });
                tutorial.nextButton = nextButton;
                Debug.Log("✅ Fixed NextButton connection");
            }
            
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(() => {
                    Debug.Log("Back button clicked via fix!");
                    tutorial.PreviousStep();
                });
                tutorial.backButton = backButton;
                Debug.Log("✅ Fixed BackButton connection");
            }
            
            if (menuButton != null)
            {
                menuButton.onClick.RemoveAllListeners();
                menuButton.onClick.AddListener(() => {
                    Debug.Log("Menu button clicked via fix!");
                    tutorial.ReturnToMenu();
                });
                tutorial.menuButton = menuButton;
                Debug.Log("✅ Fixed MenuButton connection");
            }
        }
        
        // 6. Test raycast blocking
        yield return new WaitForSeconds(0.5f);
        TestRaycastBlocking();
        
        Debug.Log("\n=== DIAGNOSTICS COMPLETE ===");
        Debug.Log("Try clicking the Next button now. Check console for 'clicked via fix' messages.");
    }
    
    void CheckButton(string buttonName, Button button)
    {
        Debug.Log($"\n--- Checking {buttonName} ---");
        
        if (button == null)
        {
            Debug.LogError($"❌ {buttonName} not found!");
            return;
        }
        
        Debug.Log($"✅ {buttonName} found");
        Debug.Log($"   Interactable: {button.interactable}");
        Debug.Log($"   Active: {button.gameObject.activeInHierarchy}");
        
        // Check Image component
        Image image = button.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogWarning($"⚠️ {buttonName} has no Image component (needed for clicking)");
            if (autoFix)
            {
                image = button.gameObject.AddComponent<Image>();
                image.color = new Color(1, 1, 1, 0.5f);
                Debug.Log("✅ Added Image component");
            }
        }
        else
        {
            Debug.Log($"   Has Image: Yes (Raycast Target: {image.raycastTarget})");
            if (!image.raycastTarget && autoFix)
            {
                image.raycastTarget = true;
                Debug.Log("✅ Enabled raycast target");
            }
        }
        
        // Check for blocking UI
        RectTransform rect = button.GetComponent<RectTransform>();
        if (rect != null)
        {
            Vector3 worldPos = rect.position;
            Debug.Log($"   World Position: {worldPos}");
        }
    }
    
    void TestRaycastBlocking()
    {
        Debug.Log("\n=== TESTING RAYCAST BLOCKING ===");
        
        // Get all UI elements
        GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();
        if (raycaster == null) return;
        
        Canvas canvas = raycaster.GetComponent<Canvas>();
        if (canvas == null) return;
        
        // Find button positions
        Button nextButton = GameObject.Find("NextButton")?.GetComponent<Button>();
        if (nextButton == null) return;
        
        RectTransform buttonRect = nextButton.GetComponent<RectTransform>();
        Vector3 buttonWorldPos = buttonRect.position;
        
        // Convert to screen space
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, buttonWorldPos);
        
        // Create pointer event data
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = screenPos;
        
        // Raycast
        var results = new System.Collections.Generic.List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        
        Debug.Log($"Raycast at NextButton position hit {results.Count} objects:");
        foreach (var result in results)
        {
            Debug.Log($"   - {result.gameObject.name} (depth: {result.depth})");
        }
        
        if (results.Count > 0 && results[0].gameObject != nextButton.gameObject)
        {
            Debug.LogWarning($"⚠️ NextButton is blocked by: {results[0].gameObject.name}");
            
            if (autoFix)
            {
                // Try to fix by adjusting sibling order
                nextButton.transform.SetAsLastSibling();
                Debug.Log("✅ Moved NextButton to front");
            }
        }
    }
}