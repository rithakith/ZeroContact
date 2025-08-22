using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MenuBackgroundLayers : MonoBehaviour
{
    [Header("Background Layers")]
    [Tooltip("Add your background sprites here in order from back to front")]
    public List<Sprite> backgroundLayers = new List<Sprite>();
    
    [Header("Layer Settings")]
    public float layerSpacing = 10f; // Z-spacing between layers for depth
    public bool useParallax = true;
    public float parallaxStrength = 0.1f;
    
    [Header("Layer Order")]
    [Tooltip("Order in hierarchy - lower numbers appear behind")]
    public int backgroundStartOrder = 0;
    public int charactersOrder = 10;
    public int uiTextOrder = 20;
    
    [Header("Canvas Reference")]
    public Canvas targetCanvas;
    
    [Header("Debug")]
    public bool regenerateLayers = false;
    
    private GameObject layerContainer;
    private List<RectTransform> layerTransforms = new List<RectTransform>();
    
    void Start()
    {
        if (Application.isPlaying)
        {
            SetupLayers();
        }
    }
    
    #if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && regenerateLayers)
        {
            regenerateLayers = false;
            SetupLayers();
        }
    }
    #endif
    
    void SetupLayers()
    {
        if (targetCanvas == null)
        {
            targetCanvas = GetComponentInParent<Canvas>();
            if (targetCanvas == null)
            {
                targetCanvas = FindObjectOfType<Canvas>();
            }
        }
        
        if (targetCanvas == null)
        {
            Debug.LogError("MenuBackgroundLayers: No Canvas found!");
            return;
        }
        
        CreateBackgroundLayers();
        OrganizeHierarchy();
    }
    
    void CreateBackgroundLayers()
    {
        // Clear existing layers
        if (layerContainer != null)
        {
            if (Application.isPlaying)
                Destroy(layerContainer);
            else
                DestroyImmediate(layerContainer);
        }
        
        layerTransforms.Clear();
        
        // Create container
        layerContainer = new GameObject("BackgroundLayers");
        layerContainer.transform.SetParent(targetCanvas.transform, false);
        layerContainer.transform.SetSiblingIndex(backgroundStartOrder);
        
        // Create each background layer
        for (int i = 0; i < backgroundLayers.Count; i++)
        {
            if (backgroundLayers[i] == null) continue;
            
            GameObject layer = new GameObject($"Background_Layer_{i}");
            layer.transform.SetParent(layerContainer.transform, false);
            
            // Add Image component
            Image img = layer.AddComponent<Image>();
            img.sprite = backgroundLayers[i];
            img.preserveAspect = true;
            
            // Setup RectTransform to fill screen
            RectTransform rect = layer.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            // Add to list for parallax
            layerTransforms.Add(rect);
            
            // Set layer order
            layer.transform.SetSiblingIndex(i);
            
            // Add parallax component if enabled
            if (Application.isPlaying && useParallax && i > 0)
            {
                ParallaxLayer parallax = layer.AddComponent<ParallaxLayer>();
                parallax.parallaxMultiplier = parallaxStrength * (i + 1);
            }
        }
    }
    
    void OrganizeHierarchy()
    {
        // Find and organize existing UI elements
        
        // Move character images to proper order
        GameObject[] characters = new GameObject[] 
        {
            GameObject.Find("Player"),
            GameObject.Find("Girl"),
            GameObject.Find("player"),
            GameObject.Find("girl")
        };
        
        foreach (var character in characters)
        {
            if (character != null && character.transform.parent == targetCanvas.transform)
            {
                character.transform.SetSiblingIndex(charactersOrder);
            }
        }
        
        // Move menu texts to front
        string[] menuTexts = new string[] 
        {
            "TITLE_Container",
            "START_Container", 
            "SETTINGS_Container",
            "QUICK GAME_Container",
            "TUTORIAL_Container",
            "MENU_Container"
        };
        
        foreach (var textName in menuTexts)
        {
            Transform text = targetCanvas.transform.Find(textName);
            if (text != null)
            {
                text.SetSiblingIndex(uiTextOrder);
            }
        }
        
        // Move any MenuManager texts
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager != null)
        {
            Transform menuTransform = menuManager.transform;
            if (menuTransform.parent == targetCanvas.transform)
            {
                menuTransform.SetSiblingIndex(uiTextOrder + 1);
            }
        }
    }
    
    public void RefreshLayers()
    {
        SetupLayers();
    }
}

// Simple parallax effect for background layers
public class ParallaxLayer : MonoBehaviour
{
    public float parallaxMultiplier = 0.1f;
    private RectTransform rectTransform;
    private Vector2 startPosition;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }
    
    void Update()
    {
        if (Input.mousePresent)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 offset = (mousePos - screenCenter) * parallaxMultiplier;
            
            rectTransform.anchoredPosition = startPosition + offset;
        }
    }
}