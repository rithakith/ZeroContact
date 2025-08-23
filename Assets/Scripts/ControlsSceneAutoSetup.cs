using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

[ExecuteInEditMode]
public class ControlsSceneAutoSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool setupComplete = false;
    public bool runCompleteSetup = false;
    
    [Header("Created References")]
    public Canvas mainCanvas;
    public Camera tutorialCamera;
    public GameObject demoPlayer;
    public ControlsTutorial tutorialController;
    
    #if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && runCompleteSetup && !setupComplete)
        {
            runCompleteSetup = false;
            PerformCompleteSetup();
        }
    }
    #endif
    
    void PerformCompleteSetup()
    {
        Debug.Log("Starting complete ControlsScene setup...");
        
        // 1. Clean up existing objects
        CleanupScene();
        
        // 2. Create main components
        SetupCamera();
        SetupCanvas();
        SetupTutorialArea();
        SetupDemoPlayer();
        
        // 3. Create all UI
        CreateCompleteUI();
        
        // 4. Setup controllers
        SetupControllers();
        
        // 5. Connect everything
        ConnectComponents();
        
        setupComplete = true;
        Debug.Log("ControlsScene setup complete! Enter Play mode to test.");
    }
    
    void CleanupScene()
    {
        // Remove old setup objects
        DestroyIfExists("SceneController");
        DestroyIfExists("TutorialController");
        DestroyIfExists("Canvas");
        DestroyIfExists("TutorialCamera");
        DestroyIfExists("TutorialGround");
        DestroyIfExists("DemoPlayer");
    }
    
    void DestroyIfExists(string objName)
    {
        GameObject obj = GameObject.Find(objName);
        if (obj != null)
            DestroyImmediate(obj);
    }
    
    void SetupCamera()
    {
        // First, remove any existing audio listeners
        AudioListener[] existingListeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener listener in existingListeners)
        {
            DestroyImmediate(listener);
        }
        
        GameObject camObj = new GameObject("TutorialCamera");
        tutorialCamera = camObj.AddComponent<Camera>();
        tutorialCamera.clearFlags = CameraClearFlags.Skybox;
        tutorialCamera.transform.position = new Vector3(0, 5, -8);
        tutorialCamera.transform.rotation = Quaternion.Euler(25, 0, 0);
        
        // Add audio listener (only one in scene now)
        camObj.AddComponent<AudioListener>();
        
        // Add light
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50, -30, 0);
        light.intensity = 1.2f;
    }
    
    void SetupCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
    }
    
    void SetupTutorialArea()
    {
        // Create ground
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "TutorialGround";
        ground.transform.localScale = new Vector3(4, 1, 4);
        ground.transform.position = Vector3.zero;
        
        Renderer groundRenderer = ground.GetComponent<Renderer>();
        Material groundMat = new Material(Shader.Find("Standard"));
        groundMat.color = new Color(0.3f, 0.3f, 0.4f);
        groundRenderer.material = groundMat;
        
        // Add grid texture if possible
        groundMat.mainTextureScale = new Vector2(8, 8);
    }
    
    void SetupDemoPlayer()
    {
        // Create player model
        demoPlayer = new GameObject("DemoPlayer");
        demoPlayer.transform.position = new Vector3(0, 0.5f, 0);
        
        // Body (Capsule)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.SetParent(demoPlayer.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
        
        Renderer bodyRenderer = body.GetComponent<Renderer>();
        Material playerMat = new Material(Shader.Find("Standard"));
        playerMat.color = new Color(0.2f, 0.5f, 0.8f);
        bodyRenderer.material = playerMat;
        
        // Shield visual
        GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shield.name = "ShieldVisual";
        shield.transform.SetParent(demoPlayer.transform);
        shield.transform.localPosition = Vector3.zero;
        shield.transform.localScale = Vector3.one * 2.2f;
        
        Renderer shieldRenderer = shield.GetComponent<Renderer>();
        Material shieldMat = new Material(Shader.Find("Standard"));
        shieldMat.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        SetMaterialTransparent(shieldMat);
        shieldRenderer.material = shieldMat;
        shield.SetActive(false);
        
        // Add PlayerDemonstration component
        PlayerDemonstration demo = demoPlayer.AddComponent<PlayerDemonstration>();
        demo.shieldVisual = shield;
    }
    
    void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
    
    void CreateCompleteUI()
    {
        // 1. Background overlay
        GameObject bgPanel = CreateUIElement("BackgroundPanel", mainCanvas.transform);
        Image bgImage = bgPanel.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f);
        SetFullScreen(bgPanel);
        
        // 2. Instruction Panel
        GameObject instructPanel = CreateUIElement("InstructionPanel", mainCanvas.transform);
        Image panelBg = instructPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.2f, 0.95f);
        
        RectTransform instructRect = instructPanel.GetComponent<RectTransform>();
        instructRect.anchorMin = new Vector2(0.1f, 0.65f);
        instructRect.anchorMax = new Vector2(0.9f, 0.9f);
        instructRect.offsetMin = Vector2.zero;
        instructRect.offsetMax = Vector2.zero;
        
        // 3. Title Text
        GameObject titleObj = CreateUIElement("TutorialTitle", instructPanel.transform);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "CONTROLS TUTORIAL";
        titleText.fontSize = 42;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.2f, 0.8f, 1f);
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.1f, 0.7f);
        titleRect.anchorMax = new Vector2(0.9f, 0.95f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // 4. Instruction Text
        GameObject instructTextObj = CreateUIElement("InstructionText", instructPanel.transform);
        TextMeshProUGUI instructText = instructTextObj.AddComponent<TextMeshProUGUI>();
        instructText.text = "Welcome to Zero Contact! Press NEXT to begin learning the controls.";
        instructText.fontSize = 24;
        instructText.alignment = TextAlignmentOptions.Center;
        instructText.color = Color.white;
        
        RectTransform instructTextRect = instructTextObj.GetComponent<RectTransform>();
        instructTextRect.anchorMin = new Vector2(0.1f, 0.3f);
        instructTextRect.anchorMax = new Vector2(0.9f, 0.7f);
        instructTextRect.offsetMin = Vector2.zero;
        instructTextRect.offsetMax = Vector2.zero;
        
        // 5. Input Prompt
        GameObject promptObj = CreateUIElement("InputPrompt", instructPanel.transform);
        TextMeshProUGUI promptText = promptObj.AddComponent<TextMeshProUGUI>();
        promptText.text = "Press NEXT to continue";
        promptText.fontSize = 20;
        promptText.fontStyle = FontStyles.Italic;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.color = new Color(1f, 1f, 0.5f);
        
        RectTransform promptRect = promptObj.GetComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.1f, 0.05f);
        promptRect.anchorMax = new Vector2(0.9f, 0.3f);
        promptRect.offsetMin = Vector2.zero;
        promptRect.offsetMax = Vector2.zero;
        
        // 6. Navigation Buttons
        CreateNavigationButtons();
        
        // 7. Progress Indicator
        CreateProgressIndicator();
        
        // 8. Control Display Panel
        CreateControlDisplay();
    }
    
    void CreateNavigationButtons()
    {
        GameObject navContainer = CreateUIElement("NavigationButtons", mainCanvas.transform);
        
        RectTransform navRect = navContainer.GetComponent<RectTransform>();
        navRect.anchorMin = new Vector2(0.2f, 0.05f);
        navRect.anchorMax = new Vector2(0.8f, 0.15f);
        navRect.offsetMin = Vector2.zero;
        navRect.offsetMax = Vector2.zero;
        
        // Back Button
        CreateButton("BackButton", navContainer.transform, "< BACK", 
            new Vector2(0f, 0f), new Vector2(0.25f, 1f), 
            new Color(0.8f, 0.3f, 0.3f));
        
        // Menu Button
        CreateButton("MenuButton", navContainer.transform, "MAIN MENU", 
            new Vector2(0.375f, 0f), new Vector2(0.625f, 1f), 
            new Color(0.5f, 0.5f, 0.5f));
        
        // Next Button
        CreateButton("NextButton", navContainer.transform, "NEXT >", 
            new Vector2(0.75f, 0f), new Vector2(1f, 1f), 
            new Color(0.3f, 0.8f, 0.3f));
    }
    
    Button CreateButton(string name, Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject buttonObj = CreateUIElement(name, parent);
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImg = buttonObj.AddComponent<Image>();
        buttonImg.color = color;
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.offsetMin = new Vector2(10, 0);
        buttonRect.offsetMax = new Vector2(-10, 0);
        
        // Button text
        GameObject textObj = CreateUIElement("Text", buttonObj.transform);
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 24;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        SetFullScreen(textObj);
        
        // Button colors
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color * 1.2f;
        colors.pressedColor = color * 0.8f;
        button.colors = colors;
        
        return button;
    }
    
    void CreateProgressIndicator()
    {
        GameObject progressObj = CreateUIElement("ProgressIndicator", mainCanvas.transform);
        TextMeshProUGUI progressText = progressObj.AddComponent<TextMeshProUGUI>();
        progressText.text = "Step 1 / 12";
        progressText.fontSize = 18;
        progressText.alignment = TextAlignmentOptions.Center;
        progressText.color = new Color(0.7f, 0.7f, 0.7f);
        
        RectTransform progressRect = progressObj.GetComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0.4f, 0.01f);
        progressRect.anchorMax = new Vector2(0.6f, 0.04f);
        progressRect.offsetMin = Vector2.zero;
        progressRect.offsetMax = Vector2.zero;
    }
    
    void CreateControlDisplay()
    {
        GameObject controlPanel = CreateUIElement("ControlDisplay", mainCanvas.transform);
        Image panelImg = controlPanel.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        RectTransform controlRect = controlPanel.GetComponent<RectTransform>();
        controlRect.anchorMin = new Vector2(0.02f, 0.3f);
        controlRect.anchorMax = new Vector2(0.25f, 0.6f);
        controlRect.offsetMin = Vector2.zero;
        controlRect.offsetMax = Vector2.zero;
        
        // Title
        GameObject titleObj = CreateUIElement("Title", controlPanel.transform);
        TextMeshProUGUI title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "CURRENT CONTROL";
        title.fontSize = 18;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.8f, 0.8f, 0.8f);
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.1f, 0.8f);
        titleRect.anchorMax = new Vector2(0.9f, 0.95f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // Key Display
        GameObject keyObj = CreateUIElement("KeyDisplay", controlPanel.transform);
        TextMeshProUGUI keyText = keyObj.AddComponent<TextMeshProUGUI>();
        keyText.text = "W A S D";
        keyText.fontSize = 36;
        keyText.fontStyle = FontStyles.Bold;
        keyText.alignment = TextAlignmentOptions.Center;
        keyText.color = new Color(1f, 1f, 0.5f);
        
        RectTransform keyRect = keyObj.GetComponent<RectTransform>();
        keyRect.anchorMin = new Vector2(0.1f, 0.4f);
        keyRect.anchorMax = new Vector2(0.9f, 0.8f);
        keyRect.offsetMin = Vector2.zero;
        keyRect.offsetMax = Vector2.zero;
        
        // Action Display
        GameObject actionObj = CreateUIElement("ActionDisplay", controlPanel.transform);
        TextMeshProUGUI actionText = actionObj.AddComponent<TextMeshProUGUI>();
        actionText.text = "Movement";
        actionText.fontSize = 22;
        actionText.alignment = TextAlignmentOptions.Center;
        actionText.color = Color.white;
        
        RectTransform actionRect = actionObj.GetComponent<RectTransform>();
        actionRect.anchorMin = new Vector2(0.1f, 0.1f);
        actionRect.anchorMax = new Vector2(0.9f, 0.4f);
        actionRect.offsetMin = Vector2.zero;
        actionRect.offsetMax = Vector2.zero;
    }
    
    void SetupControllers()
    {
        // Create TutorialController
        GameObject controllerObj = new GameObject("TutorialController");
        tutorialController = controllerObj.AddComponent<ControlsTutorial>();
        
        // The controller will find UI elements automatically
    }
    
    void ConnectComponents()
    {
        if (tutorialController != null && demoPlayer != null)
        {
            tutorialController.demoPlayer = demoPlayer;
            tutorialController.playerDemo = demoPlayer.GetComponent<PlayerDemonstration>();
            tutorialController.shieldVisual = demoPlayer.transform.Find("ShieldVisual")?.gameObject;
        }
        
        // Connect buttons to tutorial controller
        Button nextBtn = GameObject.Find("NextButton")?.GetComponent<Button>();
        Button backBtn = GameObject.Find("BackButton")?.GetComponent<Button>();
        Button menuBtn = GameObject.Find("MenuButton")?.GetComponent<Button>();
        
        if (tutorialController != null)
        {
            if (nextBtn != null) tutorialController.nextButton = nextBtn;
            if (backBtn != null) tutorialController.backButton = backBtn;
            if (menuBtn != null) tutorialController.menuButton = menuBtn;
        }
    }
    
    GameObject CreateUIElement(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        return obj;
    }
    
    void SetFullScreen(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}