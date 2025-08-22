using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[ExecuteInEditMode]
public class ControlsDisplay : MonoBehaviour
{
    [Header("Canvas Reference")]
    public Canvas targetCanvas;
    
    [Header("Font Settings")]
    public TMP_FontAsset kaphFont;
    
    [Header("Color Settings")]
    public Color frontColor = new Color(0.2f, 0.8f, 1f); // Cyan gaming color
    public Color shadowColor = new Color(0.1f, 0.4f, 0.5f); // Darker cyan for shadow
    public Color promptColor = new Color(1f, 1f, 0.5f); // Yellow for prompts
    
    [Header("Layout Settings")]
    public float shadowOffset = 3f;
    public float verticalSpacing = 50f;
    public float sectionSpacing = 120f;
    
    [Header("Editor Tools")]
    public bool regenerateDisplay = false;
    
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
            CreateControlsDisplay();
        }
    }
    
    #if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying && regenerateDisplay)
        {
            regenerateDisplay = false;
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
                CreateControlsDisplay();
            }
        }
    }
    #endif
    
    void CreateControlsDisplay()
    {
        // Clear existing controls
        ClearExistingControls();
        
        // Create title
        CreateTextWithShadow("CONTROLS", 72f, new Vector2(0, 300));
        
        // Movement Section
        float yPos = 150f;
        CreateSectionTitle("MOVEMENT", new Vector2(-300, yPos));
        CreateControlItem("W A S D / Arrow Keys", "Move", new Vector2(-300, yPos - 40));
        CreateControlItem("SHIFT", "Sprint", new Vector2(-300, yPos - 80));
        CreateControlItem("SPACE", "Jump", new Vector2(-300, yPos - 120));
        CreateControlItem("CTRL", "Dodge Roll", new Vector2(-300, yPos - 160));
        
        // Shield Section
        CreateSectionTitle("SHIELD", new Vector2(300, yPos));
        CreateControlItem("E", "Toggle Shield", new Vector2(300, yPos - 40));
        CreateControlItem("1", "Physical Mode", new Vector2(300, yPos - 80));
        CreateControlItem("2", "Fire Mode", new Vector2(300, yPos - 120));
        CreateControlItem("3", "Electric Mode", new Vector2(300, yPos - 160));
        
        // Combat Section
        yPos = -100f;
        CreateSectionTitle("COMBAT", new Vector2(0, yPos));
        CreateControlItem("F / Right Click", "Deflect Attack", new Vector2(0, yPos - 40));
        CreateControlItem("Q", "Bypass Mode", new Vector2(0, yPos - 80));
        CreateControlItem("Match Shield to Enemy Type", "Strategy Tip", new Vector2(0, yPos - 140), true);
        
        // Back button
        CreateBackButton();
    }
    
    void ClearExistingControls()
    {
        // Find and destroy existing control displays
        string[] controlNames = {"CONTROLS_Container", "MOVEMENT_Section", "SHIELD_Section", 
                                "COMBAT_Section", "BackToMenu_Container"};
        
        foreach (string controlName in controlNames)
        {
            Transform existing = targetCanvas.transform.Find(controlName);
            if (existing != null)
            {
                if (Application.isPlaying)
                    Destroy(existing.gameObject);
                else
                    DestroyImmediate(existing.gameObject);
            }
        }
        
        // Also clear any section containers
        foreach (Transform child in targetCanvas.transform)
        {
            if (child.name.EndsWith("_Section") || child.name.EndsWith("_Container"))
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
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
        shadowRect.anchorMin = Vector2.zero;
        shadowRect.anchorMax = Vector2.one;
        
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
        frontRect.anchorMin = Vector2.zero;
        frontRect.anchorMax = Vector2.one;
    }
    
    void CreateSectionTitle(string text, Vector2 position)
    {
        GameObject container = new GameObject(text + "_Section");
        container.transform.SetParent(targetCanvas.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(400, 60);
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = position;
        
        // Create underline
        GameObject underline = new GameObject("Underline");
        underline.transform.SetParent(container.transform, false);
        Image underlineImg = underline.AddComponent<Image>();
        underlineImg.color = frontColor * 0.5f;
        RectTransform underlineRect = underline.GetComponent<RectTransform>();
        underlineRect.sizeDelta = new Vector2(300, 2);
        underlineRect.anchoredPosition = new Vector2(0, -25);
        
        // Create text
        GameObject textObj = new GameObject(text + "_Text");
        textObj.transform.SetParent(container.transform, false);
        TextMeshProUGUI sectionText = textObj.AddComponent<TextMeshProUGUI>();
        sectionText.text = text;
        sectionText.font = kaphFont;
        sectionText.fontSize = 36;
        sectionText.fontStyle = FontStyles.Bold;
        sectionText.color = frontColor;
        sectionText.alignment = TextAlignmentOptions.Center;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(400, 60);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
    }
    
    void CreateControlItem(string key, string action, Vector2 position, bool isPrompt = false)
    {
        GameObject container = new GameObject(key.Replace(" ", "") + "_Control");
        container.transform.SetParent(targetCanvas.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(400, 40);
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = position;
        
        // Key text (left side)
        GameObject keyObj = new GameObject("Key");
        keyObj.transform.SetParent(container.transform, false);
        TextMeshProUGUI keyText = keyObj.AddComponent<TextMeshProUGUI>();
        keyText.text = key;
        keyText.font = kaphFont;
        keyText.fontSize = isPrompt ? 20 : 24;
        keyText.fontStyle = isPrompt ? FontStyles.Italic : FontStyles.Bold;
        keyText.color = isPrompt ? promptColor : Color.white;
        keyText.alignment = TextAlignmentOptions.MidlineRight;
        
        RectTransform keyRect = keyObj.GetComponent<RectTransform>();
        keyRect.anchorMin = new Vector2(0, 0);
        keyRect.anchorMax = new Vector2(0.45f, 1);
        keyRect.sizeDelta = Vector2.zero;
        keyRect.offsetMin = Vector2.zero;
        keyRect.offsetMax = Vector2.zero;
        
        // Separator
        if (!isPrompt)
        {
            GameObject separator = new GameObject("Separator");
            separator.transform.SetParent(container.transform, false);
            TextMeshProUGUI sepText = separator.AddComponent<TextMeshProUGUI>();
            sepText.text = "-";
            sepText.font = kaphFont;
            sepText.fontSize = 24;
            sepText.color = frontColor * 0.7f;
            sepText.alignment = TextAlignmentOptions.Center;
            
            RectTransform sepRect = separator.GetComponent<RectTransform>();
            sepRect.anchorMin = new Vector2(0.45f, 0);
            sepRect.anchorMax = new Vector2(0.55f, 1);
            sepRect.sizeDelta = Vector2.zero;
        }
        
        // Action text (right side)
        GameObject actionObj = new GameObject("Action");
        actionObj.transform.SetParent(container.transform, false);
        TextMeshProUGUI actionText = actionObj.AddComponent<TextMeshProUGUI>();
        actionText.text = action;
        actionText.font = kaphFont;
        actionText.fontSize = isPrompt ? 20 : 24;
        actionText.fontStyle = isPrompt ? FontStyles.Italic : FontStyles.Normal;
        actionText.color = isPrompt ? promptColor : frontColor;
        actionText.alignment = isPrompt ? TextAlignmentOptions.Center : TextAlignmentOptions.MidlineLeft;
        
        RectTransform actionRect = actionObj.GetComponent<RectTransform>();
        actionRect.anchorMin = new Vector2(isPrompt ? 0 : 0.55f, 0);
        actionRect.anchorMax = new Vector2(1, 1);
        actionRect.sizeDelta = Vector2.zero;
        actionRect.offsetMin = Vector2.zero;
        actionRect.offsetMax = Vector2.zero;
    }
    
    void CreateBackButton()
    {
        // Create button similar to menu style
        GameObject container = new GameObject("BackToMenu_Container");
        container.transform.SetParent(targetCanvas.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(200, 80);
        containerRect.anchorMin = new Vector2(0.5f, 0.1f);
        containerRect.anchorMax = new Vector2(0.5f, 0.1f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;
        
        // Add button component
        Button button = container.AddComponent<Button>();
        
        // Add hover effect
        ButtonHoverEffect hoverEffect = container.AddComponent<ButtonHoverEffect>();
        
        // Create shadow text
        GameObject shadowObj = new GameObject("BACK_Shadow");
        shadowObj.transform.SetParent(container.transform, false);
        TextMeshProUGUI shadowText = shadowObj.AddComponent<TextMeshProUGUI>();
        shadowText.text = "BACK";
        shadowText.font = kaphFont;
        shadowText.fontSize = 36;
        shadowText.color = shadowColor;
        shadowText.alignment = TextAlignmentOptions.Center;
        
        RectTransform shadowRect = shadowObj.GetComponent<RectTransform>();
        shadowRect.anchoredPosition = new Vector2(shadowOffset, -shadowOffset);
        shadowRect.sizeDelta = new Vector2(200, 80);
        shadowRect.anchorMin = Vector2.zero;
        shadowRect.anchorMax = Vector2.one;
        
        // Create front text
        GameObject frontObj = new GameObject("BACK_Front");
        frontObj.transform.SetParent(container.transform, false);
        TextMeshProUGUI frontText = frontObj.AddComponent<TextMeshProUGUI>();
        frontText.text = "BACK";
        frontText.font = kaphFont;
        frontText.fontSize = 36;
        frontText.color = frontColor;
        frontText.alignment = TextAlignmentOptions.Center;
        
        RectTransform frontRect = frontObj.GetComponent<RectTransform>();
        frontRect.anchoredPosition = Vector2.zero;
        frontRect.sizeDelta = new Vector2(200, 80);
        frontRect.anchorMin = Vector2.zero;
        frontRect.anchorMax = Vector2.one;
        
        // Set button colors
        ColorBlock colors = button.colors;
        colors.normalColor = frontColor;
        colors.highlightedColor = frontColor * 1.2f;
        colors.pressedColor = frontColor * 0.8f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;
        
        // Add click action
        button.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });
    }
}