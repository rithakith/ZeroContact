using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class SetupMultiLayerBackground : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Setup Multi-Layer Background in ControlsScene")]
    public static void SetupLayers()
    {
        // Find the Canvas
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Could not find Canvas in the scene");
            return;
        }
        
        // Find or create the Background parent object
        Transform backgroundTransform = canvas.transform.Find("Background");
        GameObject backgroundParent;
        
        if (backgroundTransform != null)
        {
            backgroundParent = backgroundTransform.gameObject;
            Debug.Log("Found existing Background object");
        }
        else
        {
            // Create a new Background parent if it doesn't exist
            backgroundParent = new GameObject("Background");
            backgroundParent.transform.SetParent(canvas.transform, false);
            
            // Add RectTransform and set to fill screen
            RectTransform parentRect = backgroundParent.AddComponent<RectTransform>();
            parentRect.anchorMin = Vector2.zero;
            parentRect.anchorMax = Vector2.one;
            parentRect.sizeDelta = Vector2.zero;
            parentRect.anchoredPosition = Vector2.zero;
        }
        
        // Check if it already has an Image component (single layer setup)
        Image existingImage = backgroundParent.GetComponent<Image>();
        if (existingImage != null)
        {
            Debug.Log("Converting single-layer background to multi-layer setup...");
            // Disable the image component on the parent
            existingImage.enabled = false;
        }
        
        // Create Layer 1 (Far background)
        GameObject layer1 = CreateBackgroundLayer(backgroundParent.transform, "BackgroundLayer1", -2);
        
        // Create Layer 2 (Near background)
        GameObject layer2 = CreateBackgroundLayer(backgroundParent.transform, "BackgroundLayer2", -1);
        
        // Add a script to manage the layers (optional)
        BackgroundLayerManager manager = backgroundParent.GetComponent<BackgroundLayerManager>();
        if (manager == null)
        {
            manager = backgroundParent.AddComponent<BackgroundLayerManager>();
        }
        
        // Set layer references
        manager.layer1 = layer1.GetComponent<Image>();
        manager.layer2 = layer2.GetComponent<Image>();
        
        Debug.Log("Multi-layer background setup complete!");
        Debug.Log("- Layer 1 (Far): " + layer1.name);
        Debug.Log("- Layer 2 (Near): " + layer2.name);
        Debug.Log("You can now assign sprites to each layer in the Inspector.");
        
        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        
        // Select the background parent to show it in Inspector
        Selection.activeGameObject = backgroundParent;
    }
    
    private static GameObject CreateBackgroundLayer(Transform parent, string layerName, int sortingOrder)
    {
        // Check if layer already exists
        Transform existing = parent.Find(layerName);
        if (existing != null)
        {
            Debug.Log($"Layer {layerName} already exists, updating settings...");
            return existing.gameObject;
        }
        
        // Create new layer
        GameObject layer = new GameObject(layerName);
        layer.transform.SetParent(parent, false);
        
        // Add RectTransform
        RectTransform rect = layer.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        
        // Add Image component
        Image image = layer.AddComponent<Image>();
        image.type = Image.Type.Simple;
        image.preserveAspect = false; // Fill the entire screen
        image.color = new Color(1, 1, 1, 0.8f); // Slightly transparent by default
        
        // Add Canvas component for sorting
        Canvas layerCanvas = layer.AddComponent<Canvas>();
        layerCanvas.overrideSorting = true;
        layerCanvas.sortingOrder = sortingOrder;
        
        // Add GraphicRaycaster (required for Canvas)
        layer.AddComponent<GraphicRaycaster>();
        
        return layer;
    }
#endif
}

// Simple manager component to hold references to the layers
public class BackgroundLayerManager : MonoBehaviour
{
    [Header("Background Layers")]
    [Tooltip("Far background layer (behind)")]
    public Image layer1;
    
    [Tooltip("Near background layer (front)")]
    public Image layer2;
    
    [Header("Layer Settings")]
    [Range(0f, 1f)]
    public float layer1Alpha = 1f;
    
    [Range(0f, 1f)]
    public float layer2Alpha = 0.8f;
    
    void OnValidate()
    {
        // Update alpha values in editor
        if (layer1 != null)
        {
            Color c = layer1.color;
            c.a = layer1Alpha;
            layer1.color = c;
        }
        
        if (layer2 != null)
        {
            Color c = layer2.color;
            c.a = layer2Alpha;
            layer2.color = c;
        }
    }
}