using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class FixDemoAreaRaycast : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Fix Demo Area Raycast Blocking")]
    public static void FixRaycastBlocking()
    {
        // Find all UI elements that might be blocking raycasts
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Could not find Canvas in the scene");
            return;
        }
        
        int fixCount = 0;
        
        // Fix Background
        Transform background = canvas.transform.Find("Background");
        if (background != null)
        {
            DisableRaycastOnElement(background, "Background", ref fixCount);
        }
        
        // Fix panels that shouldn't block raycasts
        Transform screenContainer = FindDeepChild(canvas.transform, "ScreenContainer");
        if (screenContainer != null)
        {
            // Fix left panel
            Transform leftPanel = FindDeepChild(screenContainer, "LeftPanel");
            if (leftPanel != null)
            {
                // Keep the panel itself non-blocking
                Image panelImage = leftPanel.GetComponent<Image>();
                if (panelImage != null && panelImage.raycastTarget)
                {
                    panelImage.raycastTarget = false;
                    fixCount++;
                    Debug.Log("Disabled raycast blocking on LeftPanel background");
                }
                
                // But ensure text remains interactive if needed
                PreserveInteractiveChildren(leftPanel);
            }
            
            // Fix right panel
            Transform rightPanel = FindDeepChild(screenContainer, "RightPanel");
            if (rightPanel != null)
            {
                Image panelImage = rightPanel.GetComponent<Image>();
                if (panelImage != null && panelImage.raycastTarget)
                {
                    panelImage.raycastTarget = false;
                    fixCount++;
                    Debug.Log("Disabled raycast blocking on RightPanel background");
                }
                
                PreserveInteractiveChildren(rightPanel);
            }
            
            // Ensure demo area doesn't block raycasts unnecessarily
            Transform demoArea = FindDeepChild(screenContainer, "DemoArea");
            if (demoArea != null)
            {
                Image demoImage = demoArea.GetComponent<Image>();
                if (demoImage != null && demoImage.raycastTarget)
                {
                    demoImage.raycastTarget = false;
                    fixCount++;
                    Debug.Log("Disabled raycast blocking on DemoArea background");
                }
            }
        }
        
        // Add a script to manage raycast blocking dynamically
        ControlsTutorialManagerEnhanced tutorialManager = GameObject.FindObjectOfType<ControlsTutorialManagerEnhanced>();
        if (tutorialManager != null)
        {
            DemoAreaRaycastManager raycastManager = tutorialManager.GetComponent<DemoAreaRaycastManager>();
            if (raycastManager == null)
            {
                raycastManager = tutorialManager.gameObject.AddComponent<DemoAreaRaycastManager>();
                Debug.Log("Added DemoAreaRaycastManager to handle dynamic raycast blocking");
            }
        }
        
        Debug.Log($"Fixed {fixCount} UI elements that were blocking raycasts");
        Debug.Log("Demo area should now be fully interactive without triggering navigation");
        
        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
    
    private static void DisableRaycastOnElement(Transform element, string elementName, ref int fixCount)
    {
        Image image = element.GetComponent<Image>();
        if (image != null && image.raycastTarget)
        {
            image.raycastTarget = false;
            fixCount++;
            Debug.Log($"Disabled raycast blocking on {elementName}");
        }
        
        // Also check for other Graphic components
        Graphic[] graphics = element.GetComponents<Graphic>();
        foreach (var graphic in graphics)
        {
            if (graphic.raycastTarget && !(graphic is Button))
            {
                graphic.raycastTarget = false;
                fixCount++;
            }
        }
    }
    
    private static void PreserveInteractiveChildren(Transform parent)
    {
        // Ensure buttons and other interactive elements remain clickable
        Button[] buttons = parent.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            Graphic graphic = button.GetComponent<Graphic>();
            if (graphic != null)
            {
                graphic.raycastTarget = true;
            }
        }
    }
    
    private static Transform FindDeepChild(Transform parent, string name)
    {
        // Search recursively for a child with the given name
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            
            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
#endif
}

// Runtime component to manage raycast blocking
public class DemoAreaRaycastManager : MonoBehaviour
{
    private ControlsTutorialManagerEnhanced tutorialManager;
    private Image[] panelImages;
    
    void Start()
    {
        tutorialManager = GetComponent<ControlsTutorialManagerEnhanced>();
        CacheUIElements();
    }
    
    void CacheUIElements()
    {
        // Cache references to panels that might block raycasts
        if (tutorialManager != null)
        {
            var allImages = new System.Collections.Generic.List<Image>();
            
            if (tutorialManager.leftPanel != null)
            {
                Image img = tutorialManager.leftPanel.GetComponent<Image>();
                if (img != null) allImages.Add(img);
            }
            
            if (tutorialManager.rightPanel != null)
            {
                Image img = tutorialManager.rightPanel.GetComponent<Image>();
                if (img != null) allImages.Add(img);
            }
            
            if (tutorialManager.demoArea != null)
            {
                Image img = tutorialManager.demoArea.GetComponent<Image>();
                if (img != null) allImages.Add(img);
            }
            
            panelImages = allImages.ToArray();
        }
    }
    
    void Update()
    {
        // Ensure panels don't block raycasts when they're visible
        foreach (var image in panelImages)
        {
            if (image != null && image.gameObject.activeInHierarchy)
            {
                // If it's not a button, it shouldn't block raycasts
                if (image.GetComponent<Button>() == null)
                {
                    image.raycastTarget = false;
                }
            }
        }
    }
}