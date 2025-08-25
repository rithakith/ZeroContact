using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EmergencyDemoFix : MonoBehaviour
{
    private bool hasFixedDemo = false;
    
    void Update()
    {
        ControlsTutorialManagerEnhanced manager = FindObjectOfType<ControlsTutorialManagerEnhanced>();
        if (manager != null && manager.isDemoActive && !hasFixedDemo)
        {
            FixDemoNow();
        }
        
        // Reset when not in demo
        if (manager != null && !manager.isDemoActive)
        {
            hasFixedDemo = false;
        }
    }
    
    void FixDemoNow()
    {
        ControlsTutorialManagerEnhanced manager = FindObjectOfType<ControlsTutorialManagerEnhanced>();
        if (manager == null || !manager.isDemoActive || manager.currentDemoPlayer == null) return;
        
        hasFixedDemo = true;
        
        Debug.Log("EMERGENCY FIX: Attempting to make player visible");
        
        // 1. Move player to a better position
        manager.currentDemoPlayer.transform.position = new Vector3(0, 1, 0);
        
        // Fix sprite sorting order to appear above light rays
        SpriteRenderer[] playerSprites = manager.currentDemoPlayer.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in playerSprites)
        {
            sprite.sortingLayerName = "Default";
            sprite.sortingOrder = 100; // High order to be on top of light rays
            Debug.Log($"Set {sprite.name} sorting order to 100");
        }
        
        // 3. Check if the demo area is actually visible
        if (manager.demoArea != null)
        {
            RectTransform demoRect = manager.demoArea.GetComponent<RectTransform>();
            if (demoRect != null)
            {
                Debug.Log($"Demo area rect: {demoRect.rect}, anchored pos: {demoRect.anchoredPosition}");
                
                // Make sure it's not zero size
                if (demoRect.rect.width <= 0 || demoRect.rect.height <= 0)
                {
                    Debug.LogError("Demo area has zero size!");
                    demoRect.sizeDelta = new Vector2(400, 300);
                }
            }
            
            // Check ALL images in demo area hierarchy that might be blocking
            Image[] allDemoImages = manager.demoArea.GetComponentsInChildren<Image>();
            foreach (var img in allDemoImages)
            {
                // Check if this looks like a background image
                if (img.name.ToLower().Contains("background") || img.name.ToLower().Contains("bg") || 
                    img.name.ToLower().Contains("panel") || img.name.ToLower().Contains("overlay"))
                {
                    Debug.Log($"Found potential blocking image in demo area: {img.name}, color: {img.color}");
                    // Make it transparent
                    Color c = img.color;
                    c.a = 0;
                    img.color = c;
                    img.raycastTarget = false;
                }
                else if (img.enabled && img.color.a > 0.8f)
                {
                    // Any other opaque image
                    Debug.Log($"Making demo area image transparent: {img.name}");
                    Color c = img.color;
                    c.a = 0;
                    img.color = c;
                    img.raycastTarget = false;
                }
            }
        }
        
        // 4. Force demo camera to specific settings
        if (manager.demoCamera != null)
        {
            manager.demoCamera.orthographic = true;
            manager.demoCamera.orthographicSize = 5;
            manager.demoCamera.nearClipPlane = -100;
            manager.demoCamera.farClipPlane = 100;
            manager.demoCamera.transform.position = new Vector3(0, 0, -10);
            
            // Keep default clear flags
            
            Debug.Log($"Demo camera settings - Active: {manager.demoCamera.gameObject.activeInHierarchy}, Enabled: {manager.demoCamera.enabled}");
        }
        
        // 5. Check if there's a Canvas blocking the view
        if (manager.rightPanel != null)
        {
            // Look for any opaque images in the right panel
            Image[] images = manager.rightPanel.GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                // Skip if this is a text component background
                if (img.GetComponentInChildren<Text>() != null || img.GetComponentInChildren<TMPro.TextMeshProUGUI>() != null)
                    continue;
                    
                if (img.color.a > 0.5f && img.enabled)
                {
                    Debug.Log($"Found opaque image in right panel: {img.name}, making it transparent");
                    Color c = img.color;
                    c.a = 0f; // Make completely transparent
                    img.color = c;
                    img.raycastTarget = false; // Don't block input
                }
            }
            
            // Check if right panel itself has blocking components
            CanvasGroup cg = manager.rightPanel.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = manager.rightPanel.AddComponent<CanvasGroup>();
            }
            cg.alpha = 1f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
        
        Debug.Log("Emergency fix complete!");
    }
}