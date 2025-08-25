using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HideBackgroundDuringDemo : MonoBehaviour
{
    private ControlsTutorialManagerEnhanced manager;
    private List<GameObject> hiddenObjects = new List<GameObject>();
    private bool wasInDemo = false;
    
    void Start()
    {
        manager = GetComponent<ControlsTutorialManagerEnhanced>();
    }
    
    void Update()
    {
        if (manager == null) return;
        
        // When entering demo mode
        if (manager.isDemoActive && !wasInDemo)
        {
            HideBackgroundLayers();
            wasInDemo = true;
        }
        // When exiting demo mode
        else if (!manager.isDemoActive && wasInDemo)
        {
            RestoreBackgroundLayers();
            wasInDemo = false;
        }
    }
    
    void HideBackgroundLayers()
    {
        Debug.Log("Hiding background layers for demo");
        hiddenObjects.Clear();
        
        // Find all GameObjects that might be background layers
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // Skip if it's part of the demo system
            if (obj == manager.demoArea || obj == manager.rightPanel || 
                obj.transform.IsChildOf(manager.demoArea.transform) ||
                (manager.currentDemoPlayer != null && obj.transform.IsChildOf(manager.currentDemoPlayer.transform)))
            {
                continue;
            }
            
            // Check if this looks like a background layer
            string nameLower = obj.name.ToLower();
            if (nameLower.Contains("backgroundlayer") || 
                nameLower.Contains("grass") && nameLower.Contains("background") ||
                nameLower.Contains("bg") && nameLower.Contains("layer"))
            {
                // Check if it has an Image component
                Image img = obj.GetComponent<Image>();
                if (img != null && img.enabled)
                {
                    Debug.Log($"Hiding background layer: {obj.name}");
                    obj.SetActive(false);
                    hiddenObjects.Add(obj);
                }
                
                // Also check for Canvas with sorting order
                Canvas canvas = obj.GetComponent<Canvas>();
                if (canvas != null && canvas.overrideSorting && canvas.sortingOrder > 0)
                {
                    Debug.Log($"Hiding canvas layer: {obj.name} (order: {canvas.sortingOrder})");
                    obj.SetActive(false);
                    hiddenObjects.Add(obj);
                }
            }
        }
        
        // Also look specifically in the background parent object
        GameObject background = GameObject.Find("Background");
        if (background != null)
        {
            // Hide any child layers with sorting
            foreach (Transform child in background.transform)
            {
                Canvas childCanvas = child.GetComponent<Canvas>();
                if (childCanvas != null && childCanvas.overrideSorting)
                {
                    Debug.Log($"Hiding background child: {child.name}");
                    child.gameObject.SetActive(false);
                    hiddenObjects.Add(child.gameObject);
                }
            }
        }
    }
    
    void RestoreBackgroundLayers()
    {
        Debug.Log("Restoring background layers");
        foreach (GameObject obj in hiddenObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
        hiddenObjects.Clear();
    }
    
    void OnDestroy()
    {
        // Make sure to restore everything when destroyed
        RestoreBackgroundLayers();
    }
}