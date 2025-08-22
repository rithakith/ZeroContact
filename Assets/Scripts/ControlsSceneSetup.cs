using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[ExecuteInEditMode]
public class ControlsSceneSetup : MonoBehaviour
{
    [Header("Scene Setup")]
    public bool createControlsScene = false;
    public string controlsSceneName = "ControlsScene";
    
    [Header("References")]
    public Canvas mainCanvas;
    public Camera tutorialCamera;
    
    #if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && createControlsScene)
        {
            createControlsScene = false;
            SetupControlsScene();
        }
    }
    #endif
    
    void SetupControlsScene()
    {
        // Create or find canvas
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                mainCanvas = canvasObj.AddComponent<Canvas>();
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
        }
        
        // Create camera for 3D tutorial area
        if (tutorialCamera == null)
        {
            GameObject camObj = GameObject.Find("TutorialCamera");
            if (camObj == null)
            {
                camObj = new GameObject("TutorialCamera");
                tutorialCamera = camObj.AddComponent<Camera>();
                tutorialCamera.transform.position = new Vector3(0, 5, -10);
                tutorialCamera.transform.rotation = Quaternion.Euler(20, 0, 0);
                
                // Add audio listener
                if (FindObjectOfType<AudioListener>() == null)
                {
                    camObj.AddComponent<AudioListener>();
                }
            }
            else
            {
                tutorialCamera = camObj.GetComponent<Camera>();
            }
        }
        
        // Create tutorial area
        CreateTutorialArea();
        
        // Create UI elements
        CreateUIElements();
        
        Debug.Log("Controls scene setup complete!");
    }
    
    void CreateTutorialArea()
    {
        // Create ground plane
        GameObject ground = GameObject.Find("TutorialGround");
        if (ground == null)
        {
            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "TutorialGround";
            ground.transform.localScale = new Vector3(5, 1, 5);
            ground.transform.position = Vector3.zero;
            
            // Add a simple material
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            groundRenderer.material.color = new Color(0.2f, 0.2f, 0.3f);
        }
        
        // Create demo player position marker
        GameObject playerMarker = GameObject.Find("PlayerStartPosition");
        if (playerMarker == null)
        {
            playerMarker = new GameObject("PlayerStartPosition");
            playerMarker.transform.position = new Vector3(0, 0.5f, 0);
        }
    }
    
    void CreateUIElements()
    {
        // Create background panel
        GameObject bgPanel = CreateOrFindUIElement("BackgroundPanel", mainCanvas.transform);
        Image bgImage = bgPanel.GetComponent<Image>();
        if (bgImage == null) bgImage = bgPanel.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);
        RectTransform bgRect = bgPanel.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // Create instruction panel
        GameObject instructionPanel = CreateOrFindUIElement("InstructionPanel", mainCanvas.transform);
        RectTransform instructRect = instructionPanel.GetComponent<RectTransform>();
        instructRect.anchorMin = new Vector2(0.5f, 0.7f);
        instructRect.anchorMax = new Vector2(0.5f, 0.9f);
        instructRect.sizeDelta = new Vector2(800, 150);
        instructRect.anchoredPosition = Vector2.zero;
        
        // Add background to instruction panel
        Image panelBg = instructionPanel.GetComponent<Image>();
        if (panelBg == null) panelBg = instructionPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.2f, 0.9f);
        
        // Create title text
        GameObject titleObj = CreateOrFindUIElement("TutorialTitle", instructionPanel.transform);
        TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
        if (titleText == null) titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "CONTROLS TUTORIAL";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(0.2f, 0.8f, 1f);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.6f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;
        
        // Create instruction text
        GameObject instructObj = CreateOrFindUIElement("InstructionText", instructionPanel.transform);
        TextMeshProUGUI instructText = instructObj.GetComponent<TextMeshProUGUI>();
        if (instructText == null) instructText = instructObj.AddComponent<TextMeshProUGUI>();
        instructText.text = "Press NEXT to begin the tutorial";
        instructText.fontSize = 24;
        instructText.alignment = TextAlignmentOptions.Center;
        instructText.color = Color.white;
        RectTransform instructRect2 = instructObj.GetComponent<RectTransform>();
        instructRect2.anchorMin = new Vector2(0, 0);
        instructRect2.anchorMax = new Vector2(1, 0.6f);
        instructRect2.sizeDelta = Vector2.zero;
        instructRect2.anchoredPosition = Vector2.zero;
        
        // Create navigation buttons
        CreateNavigationButtons();
        
        // Create progress indicator
        CreateProgressIndicator();
    }
    
    void CreateNavigationButtons()
    {
        // Create button container
        GameObject buttonContainer = CreateOrFindUIElement("NavigationButtons", mainCanvas.transform);
        RectTransform containerRect = buttonContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.05f);
        containerRect.anchorMax = new Vector2(0.5f, 0.15f);
        containerRect.sizeDelta = new Vector2(600, 80);
        containerRect.anchoredPosition = Vector2.zero;
        
        // Create Back button
        GameObject backButton = CreateOrFindUIElement("BackButton", buttonContainer.transform);
        Button backBtn = backButton.GetComponent<Button>();
        if (backBtn == null) backBtn = backButton.AddComponent<Button>();
        Image backBg = backButton.GetComponent<Image>();
        if (backBg == null) backBg = backButton.AddComponent<Image>();
        backBg.color = new Color(0.8f, 0.2f, 0.2f);
        
        TextMeshProUGUI backText = GetOrCreateTextComponent(backButton, "BACK");
        backText.fontSize = 28;
        
        RectTransform backRect = backButton.GetComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0, 0);
        backRect.anchorMax = new Vector2(0.3f, 1);
        backRect.sizeDelta = Vector2.zero;
        backRect.anchoredPosition = Vector2.zero;
        
        // Create Next button
        GameObject nextButton = CreateOrFindUIElement("NextButton", buttonContainer.transform);
        Button nextBtn = nextButton.GetComponent<Button>();
        if (nextBtn == null) nextBtn = nextButton.AddComponent<Button>();
        Image nextBg = nextButton.GetComponent<Image>();
        if (nextBg == null) nextBg = nextButton.AddComponent<Image>();
        nextBg.color = new Color(0.2f, 0.8f, 0.2f);
        
        TextMeshProUGUI nextText = GetOrCreateTextComponent(nextButton, "NEXT");
        nextText.fontSize = 28;
        
        RectTransform nextRect = nextButton.GetComponent<RectTransform>();
        nextRect.anchorMin = new Vector2(0.7f, 0);
        nextRect.anchorMax = new Vector2(1, 1);
        nextRect.sizeDelta = Vector2.zero;
        nextRect.anchoredPosition = Vector2.zero;
        
        // Create Menu button (center)
        GameObject menuButton = CreateOrFindUIElement("MenuButton", buttonContainer.transform);
        Button menuBtn = menuButton.GetComponent<Button>();
        if (menuBtn == null) menuBtn = menuButton.AddComponent<Button>();
        Image menuBg = menuButton.GetComponent<Image>();
        if (menuBg == null) menuBg = menuButton.AddComponent<Image>();
        menuBg.color = new Color(0.5f, 0.5f, 0.5f);
        
        TextMeshProUGUI menuText = GetOrCreateTextComponent(menuButton, "MAIN MENU");
        menuText.fontSize = 24;
        
        RectTransform menuRect = menuButton.GetComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0.35f, 0);
        menuRect.anchorMax = new Vector2(0.65f, 1);
        menuRect.sizeDelta = Vector2.zero;
        menuRect.anchoredPosition = Vector2.zero;
    }
    
    void CreateProgressIndicator()
    {
        GameObject progressContainer = CreateOrFindUIElement("ProgressIndicator", mainCanvas.transform);
        RectTransform progressRect = progressContainer.GetComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0.5f, 0.02f);
        progressRect.anchorMax = new Vector2(0.5f, 0.05f);
        progressRect.sizeDelta = new Vector2(400, 30);
        progressRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI progressText = progressContainer.GetComponent<TextMeshProUGUI>();
        if (progressText == null) progressText = progressContainer.AddComponent<TextMeshProUGUI>();
        progressText.text = "Step 1 / 10";
        progressText.fontSize = 18;
        progressText.alignment = TextAlignmentOptions.Center;
        progressText.color = new Color(0.7f, 0.7f, 0.7f);
    }
    
    GameObject CreateOrFindUIElement(string name, Transform parent)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.gameObject;
        
        GameObject newObj = new GameObject(name);
        newObj.transform.SetParent(parent, false);
        return newObj;
    }
    
    TextMeshProUGUI GetOrCreateTextComponent(GameObject obj, string defaultText)
    {
        TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);
            text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = defaultText;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
        }
        return text;
    }
}