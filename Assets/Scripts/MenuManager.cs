using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class MenuManager : MonoBehaviour
{
    [Header("Canvas Reference")]
    public Canvas targetCanvas;

    [Header("Font Settings")]
    public TMP_FontAsset kaphFont;

    [Header("Color Settings")]
    public Color frontColor = new Color(0.2f, 0.8f, 1f); // Cyan gaming color
    public Color shadowColor = new Color(0.1f, 0.4f, 0.5f); // Darker cyan for shadow

    [Header("Layout Settings")]
    public float shadowOffset = 3f;
    public float verticalSpacing = 80f;
    
    [Header("Editor Tools")]
    public bool regenerateTexts = false;

    void Start()
    {
        if (targetCanvas == null)
        {
            targetCanvas = GetComponentInParent<Canvas>();
            if (targetCanvas == null)
            {
                targetCanvas = FindObjectOfType<Canvas>();
            }
        }
        
        if (targetCanvas != null)
        {
            CreateMenuTexts();
        }
        else
        {
            Debug.LogError("MenuManager: No Canvas found! Please assign a Canvas or ensure this GameObject is a child of a Canvas.");
        }
    }
    
    #if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying && regenerateTexts)
        {
            regenerateTexts = false;
            if (targetCanvas == null)
            {
                targetCanvas = GetComponentInParent<Canvas>();
                if (targetCanvas == null)
                {
                    targetCanvas = FindObjectOfType<Canvas>();
                }
            }
            
            if (targetCanvas != null)
            {
                CreateMenuTexts();
            }
        }
    }
    #endif

    void CreateMenuTexts()
    {
        // Clear existing texts first
        ClearExistingTexts();
        
        // Create Title
        CreateTextWithShadow("TITLE", 72f, new Vector2(0, 200));

        // Create Start
        CreateTextWithShadow("START", 48f, new Vector2(0, 80));

        // Create Settings
        CreateTextWithShadow("SETTINGS", 48f, new Vector2(0, 0));

        // Create Quick Game
        CreateTextWithShadow("QUICK GAME", 48f, new Vector2(0, -80));

        // Create Tutorial
        CreateTextWithShadow("TUTORIAL", 48f, new Vector2(0, -160));

        // Create Menu
        CreateTextWithShadow("MENU", 48f, new Vector2(0, -240));
    }

    void ClearExistingTexts()
    {
        // Find and destroy existing menu text containers
        string[] menuTexts = {"TITLE_Container", "START_Container", "SETTINGS_Container", 
                             "QUICK GAME_Container", "TUTORIAL_Container", "MENU_Container"};
        
        foreach (string textName in menuTexts)
        {
            Transform existing = targetCanvas.transform.Find(textName);
            if (existing != null)
            {
                if (Application.isPlaying)
                    Destroy(existing.gameObject);
                else
                    DestroyImmediate(existing.gameObject);
            }
        }
    }
    
    void CreateTextWithShadow(string text, float fontSize, Vector2 position)
    {
        // Create parent container
        GameObject container = new GameObject(text + "_Container");
        container.transform.SetParent(targetCanvas.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(800, 100);
        
        // Ensure the container is properly positioned in the center
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = position;
        
        // Move to front by setting as last sibling
        container.transform.SetAsLastSibling();

        // Create shadow text
        GameObject shadowObj = new GameObject(text + "_Shadow");
        shadowObj.transform.SetParent(container.transform, false);
        TextMeshProUGUI shadowText = shadowObj.AddComponent<TextMeshProUGUI>();
        shadowText.text = text;
        shadowText.font = kaphFont;
        shadowText.fontSize = fontSize;
        shadowText.color = shadowColor;
        shadowText.alignment = TextAlignmentOptions.Center;
        
        RectTransform shadowRect = shadowObj.GetComponent<RectTransform>();
        shadowRect.anchoredPosition = new Vector2(shadowOffset, -shadowOffset);
        shadowRect.sizeDelta = new Vector2(800, 100);

        // Create front text
        GameObject frontObj = new GameObject(text + "_Front");
        frontObj.transform.SetParent(container.transform, false);
        TextMeshProUGUI frontText = frontObj.AddComponent<TextMeshProUGUI>();
        frontText.text = text;
        frontText.font = kaphFont;
        frontText.fontSize = fontSize;
        frontText.color = frontColor;
        frontText.alignment = TextAlignmentOptions.Center;
        
        RectTransform frontRect = frontObj.GetComponent<RectTransform>();
        frontRect.anchoredPosition = Vector2.zero;
        frontRect.sizeDelta = new Vector2(800, 100);
    }
}