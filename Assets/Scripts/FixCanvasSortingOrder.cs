using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FixCanvasSortingOrder : MonoBehaviour
{
    private ControlsTutorialManagerEnhanced manager;
    
    void Start()
    {
        manager = GetComponent<ControlsTutorialManagerEnhanced>();
        StartCoroutine(FixSortingOrders());
    }
    
    IEnumerator FixSortingOrders()
    {
        yield return new WaitForSeconds(0.5f);
        
        while (true)
        {
            if (manager != null && manager.isDemoActive)
            {
                // Find all canvases in the scene
                Canvas[] allCanvases = FindObjectsOfType<Canvas>();
                
                foreach (Canvas canvas in allCanvases)
                {
                    // Check if this is a background canvas
                    if (canvas.name.ToLower().Contains("background") || 
                        canvas.transform.parent != null && canvas.transform.parent.name.ToLower().Contains("background"))
                    {
                        Debug.Log($"Found background canvas: {canvas.name}, sorting order: {canvas.sortingOrder}");
                        
                        // Lower its sorting order
                        if (canvas.sortingOrder > -10)
                        {
                            canvas.sortingOrder = -10;
                            Debug.Log($"Lowered {canvas.name} sorting order to -10");
                        }
                    }
                }
                
                // Find all Images with Canvas components (UI elements that might have sorting order)
                GraphicRaycaster[] raycasters = FindObjectsOfType<GraphicRaycaster>();
                foreach (var raycaster in raycasters)
                {
                    Canvas canvas = raycaster.GetComponent<Canvas>();
                    if (canvas != null && canvas.overrideSorting)
                    {
                        // Check for background layers
                        if (raycaster.name.ToLower().Contains("backgroundlayer") || 
                            raycaster.name.ToLower().Contains("grass") ||
                            raycaster.name.ToLower().Contains("bg"))
                        {
                            Debug.Log($"Found background layer with override sorting: {raycaster.name}, order: {canvas.sortingOrder}");
                            canvas.sortingOrder = -20;
                        }
                    }
                }
                
                // Also check Images that might have sorting order set
                Image[] allImages = FindObjectsOfType<Image>();
                foreach (Image img in allImages)
                {
                    // Check if this image has a Canvas component with override sorting
                    Canvas imgCanvas = img.GetComponent<Canvas>();
                    if (imgCanvas != null && imgCanvas.overrideSorting)
                    {
                        if (img.name.ToLower().Contains("backgroundlayer") || 
                            img.name.ToLower().Contains("grass"))
                        {
                            Debug.Log($"Found background image with canvas: {img.name}, order: {imgCanvas.sortingOrder}");
                            imgCanvas.sortingOrder = -30;
                        }
                    }
                }
                
                // Ensure demo area has proper sorting
                if (manager.demoArea != null)
                {
                    Canvas demoCanvas = manager.demoArea.GetComponent<Canvas>();
                    if (demoCanvas == null)
                    {
                        demoCanvas = manager.demoArea.AddComponent<Canvas>();
                    }
                    
                    // Set high sorting order for demo area
                    demoCanvas.overrideSorting = true;
                    demoCanvas.sortingOrder = 100;
                    
                    GraphicRaycaster demoRaycaster = manager.demoArea.GetComponent<GraphicRaycaster>();
                    if (demoRaycaster == null)
                    {
                        demoRaycaster = manager.demoArea.AddComponent<GraphicRaycaster>();
                    }
                    
                    Debug.Log("Set demo area canvas sorting order to 100");
                }
                
                // Also boost the demo camera's depth
                if (manager.demoCamera != null)
                {
                    manager.demoCamera.depth = 10;
                }
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
}