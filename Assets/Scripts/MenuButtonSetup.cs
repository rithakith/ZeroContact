using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class MenuButtonSetup : MonoBehaviour
{
    [Header("Button References")]
    public Button playButton;
    public Button settingsButton;
    public Button controlsButton;
    public Button quitButton;
    
    [Header("Create Buttons from Text")]
    public bool convertTextsToButtons = false;
    
    [Header("Apply Hover Effects")]
    public bool applyHoverEffects = false;
    
    [Header("Debug")]
    public bool showButtonStatus = false;
    [TextArea(3, 10)]
    public string buttonStatusInfo = "Click 'Show Button Status' to check";
    
    [Header("Button Appearance")]
    public Color normalColor = new Color(0.2f, 0.8f, 1f);
    public Color highlightedColor = new Color(0.4f, 0.9f, 1f);
    public Color pressedColor = new Color(0.1f, 0.6f, 0.8f);
    public float colorTransitionDuration = 0.1f;
    
    void Start()
    {
        if (Application.isPlaying)
        {
            SetupButtons();
        }
    }
    
    #if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && convertTextsToButtons)
        {
            convertTextsToButtons = false;
            ConvertTextsToButtons();
        }
        
        if (!Application.isPlaying && showButtonStatus)
        {
            showButtonStatus = false;
            CheckButtonStatus();
        }
        
        if (!Application.isPlaying && applyHoverEffects)
        {
            applyHoverEffects = false;
            ApplyHoverEffectsToButtons();
        }
    }
    #endif
    
    void ApplyHoverEffectsToButtons()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        string[] containerNames = { "START_Container", "SETTINGS_Container", "TUTORIAL_Container", "MENU_Container", "QUICK GAME_Container" };
        int effectsAdded = 0;
        
        foreach (string containerName in containerNames)
        {
            Transform container = canvas.transform.Find(containerName);
            if (container != null)
            {
                // Check if it has a Button component
                Button button = container.GetComponent<Button>();
                if (button != null)
                {
                    // Add hover effect if not present
                    ButtonHoverEffect hoverEffect = container.GetComponent<ButtonHoverEffect>();
                    if (hoverEffect == null)
                    {
                        hoverEffect = container.gameObject.AddComponent<ButtonHoverEffect>();
                        effectsAdded++;
                        Debug.Log($"Added hover effect to {containerName}");
                    }
                    else
                    {
                        Debug.Log($"{containerName} already has hover effect");
                    }
                }
                else
                {
                    Debug.LogWarning($"{containerName} doesn't have a Button component. Run 'Convert Texts To Buttons' first.");
                }
            }
        }
        
        Debug.Log($"Hover effects check complete. Added {effectsAdded} new effects.");
    }
    
    void CheckButtonStatus()
    {
        string status = "BUTTON STATUS CHECK:\n\n";
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            status += "❌ No Canvas found!\n";
            buttonStatusInfo = status;
            return;
        }
        
        // Check each text container
        Transform startText = canvas.transform.Find("START_Container");
        status += CheckTextContainer(startText, "START", ref playButton);
        
        Transform settingsText = canvas.transform.Find("SETTINGS_Container");
        status += CheckTextContainer(settingsText, "SETTINGS", ref settingsButton);
        
        Transform tutorialText = canvas.transform.Find("TUTORIAL_Container");
        status += CheckTextContainer(tutorialText, "TUTORIAL/CONTROLS", ref controlsButton);
        
        Transform menuText = canvas.transform.Find("MENU_Container");
        status += CheckTextContainer(menuText, "MENU/QUIT", ref quitButton);
        
        // Check MainMenuController
        MainMenuController controller = FindObjectOfType<MainMenuController>();
        if (controller != null)
        {
            status += "\n✅ MainMenuController found\n";
            status += $"  - Play Button: {(controller.playButton != null ? "✅ Connected" : "❌ Not Connected")}\n";
            status += $"  - Settings Button: {(controller.settingsButton != null ? "✅ Connected" : "❌ Not Connected")}\n";
            status += $"  - Controls Button: {(controller.controlsButton != null ? "✅ Connected" : "❌ Not Connected")}\n";
            status += $"  - Quit Button: {(controller.quitButton != null ? "✅ Connected" : "❌ Not Connected")}\n";
        }
        else
        {
            status += "\n❌ MainMenuController NOT found!\n";
        }
        
        buttonStatusInfo = status;
        Debug.Log(status);
    }
    
    string CheckTextContainer(Transform container, string name, ref Button buttonRef)
    {
        string result = $"\n{name}:\n";
        
        if (container == null)
        {
            result += "  ❌ Container not found\n";
            return result;
        }
        
        result += "  ✅ Container found\n";
        
        Button button = container.GetComponent<Button>();
        if (button != null)
        {
            result += "  ✅ Button component exists\n";
            buttonRef = button;
            
            // Check if it's interactable
            if (button.interactable)
            {
                result += "  ✅ Button is interactable\n";
            }
            else
            {
                result += "  ⚠️ Button is not interactable\n";
            }
        }
        else
        {
            result += "  ❌ No Button component\n";
        }
        
        return result;
    }
    
    void ConvertTextsToButtons()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        // Find and convert START text to Play button
        Transform startText = canvas.transform.Find("START_Container");
        if (startText != null)
        {
            playButton = ConvertToButton(startText.gameObject, "Play");
        }
        
        // Find and convert SETTINGS text to Settings button
        Transform settingsText = canvas.transform.Find("SETTINGS_Container");
        if (settingsText != null)
        {
            settingsButton = ConvertToButton(settingsText.gameObject, "Settings");
        }
        
        // Find and convert TUTORIAL text to Controls button
        Transform tutorialText = canvas.transform.Find("TUTORIAL_Container");
        if (tutorialText != null)
        {
            controlsButton = ConvertToButton(tutorialText.gameObject, "Controls");
        }
        
        // Find and convert MENU text to Quit button
        Transform menuText = canvas.transform.Find("MENU_Container");
        if (menuText != null)
        {
            quitButton = ConvertToButton(menuText.gameObject, "Quit");
        }
        
        Debug.Log("Text elements converted to buttons. Now connect them to MainMenuController.");
        
        // Try to automatically connect to MainMenuController
        MainMenuController controller = FindObjectOfType<MainMenuController>();
        if (controller != null)
        {
            if (playButton != null && controller.playButton == null)
                controller.playButton = playButton;
            if (settingsButton != null && controller.settingsButton == null)
                controller.settingsButton = settingsButton;
            if (controlsButton != null && controller.controlsButton == null)
                controller.controlsButton = controlsButton;
            if (quitButton != null && controller.quitButton == null)
                controller.quitButton = quitButton;
                
            Debug.Log("Buttons automatically connected to MainMenuController!");
        }
    }
    
    Button ConvertToButton(GameObject textContainer, string buttonName)
    {
        // Add Button component if not present
        Button button = textContainer.GetComponent<Button>();
        if (button == null)
        {
            button = textContainer.AddComponent<Button>();
        }
        
        // Set up button colors
        ColorBlock colors = button.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = highlightedColor;
        colors.pressedColor = pressedColor;
        colors.selectedColor = highlightedColor;
        colors.fadeDuration = colorTransitionDuration;
        button.colors = colors;
        
        // Add interactive hover effect
        ButtonHoverEffect hoverEffect = textContainer.GetComponent<ButtonHoverEffect>();
        if (hoverEffect == null)
        {
            hoverEffect = textContainer.AddComponent<ButtonHoverEffect>();
        }
        
        // Set button navigation to None to prevent arrow key navigation issues
        Navigation nav = button.navigation;
        nav.mode = Navigation.Mode.None;
        button.navigation = nav;
        
        Debug.Log($"Created {buttonName} button from {textContainer.name}");
        
        return button;
    }
    
    void SetupButtons()
    {
        // Find MainMenuController
        MainMenuController menuController = FindObjectOfType<MainMenuController>();
        if (menuController == null)
        {
            Debug.LogError("MainMenuController not found in scene!");
            return;
        }
        
        // Connect buttons if they exist
        if (playButton != null && menuController.playButton == null)
        {
            menuController.playButton = playButton;
        }
        
        if (settingsButton != null && menuController.settingsButton == null)
        {
            menuController.settingsButton = settingsButton;
        }
        
        if (quitButton != null && menuController.quitButton == null)
        {
            menuController.quitButton = quitButton;
        }
        
        // Add controls button functionality
        if (controlsButton != null)
        {
            controlsButton.onClick.RemoveAllListeners();
            controlsButton.onClick.AddListener(() => 
            {
                if (menuController.buttonClickSound != null)
                {
                    menuController.buttonClickSound.Play();
                }
                Debug.Log("Controls button clicked!");
                // Add your controls screen logic here
            });
        }
    }
}

// Enhanced hover effect for menu buttons
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private TMPro.TextMeshProUGUI[] textComponents;
    private Color[] originalTextColors;
    
    [Header("Scale Animation")]
    public float hoverScale = 1.1f;
    public float clickScale = 0.95f;
    public float animationSpeed = 10f;
    
    [Header("Color Animation")]
    public bool animateColor = true;
    public Color hoverTextColor = new Color(1f, 1f, 0.8f); // Light yellow
    public float colorAnimationSpeed = 8f;
    
    [Header("Position Animation")]
    public bool animatePosition = true;
    public float hoverOffsetY = 5f;
    private Vector2 originalPosition;
    
    private bool isHovered = false;
    private bool isPressed = false;
    private float currentScale = 1f;
    private float targetScale = 1f;
    private float currentOffsetY = 0f;
    private float targetOffsetY = 0f;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;
        
        // Get all text components in children
        textComponents = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        originalTextColors = new Color[textComponents.Length];
        for (int i = 0; i < textComponents.Length; i++)
        {
            originalTextColors[i] = textComponents[i].color;
        }
    }
    
    void Update()
    {
        if (rectTransform != null)
        {
            // Update scale
            currentScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * animationSpeed);
            rectTransform.localScale = originalScale * currentScale;
            
            // Update position
            if (animatePosition)
            {
                currentOffsetY = Mathf.Lerp(currentOffsetY, targetOffsetY, Time.deltaTime * animationSpeed);
                rectTransform.anchoredPosition = originalPosition + new Vector2(0, currentOffsetY);
            }
            
            // Update text color
            if (animateColor && textComponents != null)
            {
                for (int i = 0; i < textComponents.Length; i++)
                {
                    if (textComponents[i] != null)
                    {
                        Color targetColor = isHovered ? hoverTextColor : originalTextColors[i];
                        textComponents[i].color = Color.Lerp(textComponents[i].color, targetColor, Time.deltaTime * colorAnimationSpeed);
                    }
                }
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (!isPressed)
        {
            targetScale = hoverScale;
            targetOffsetY = hoverOffsetY;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (!isPressed)
        {
            targetScale = 1f;
            targetOffsetY = 0f;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        targetScale = clickScale;
        targetOffsetY = -2f;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        targetScale = isHovered ? hoverScale : 1f;
        targetOffsetY = isHovered ? hoverOffsetY : 0f;
    }
}