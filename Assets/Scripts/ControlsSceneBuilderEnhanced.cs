using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class ControlsSceneBuilderEnhanced : MonoBehaviour
{
    [Header("Build Controls")]
    public bool buildScene = false;
    public bool clearScene = false;
    
    void Update()
    {
        if (buildScene)
        {
            buildScene = false;
            BuildEnhancedControlsScene();
        }
        
        if (clearScene)
        {
            clearScene = false;
            ClearScene();
        }
    }
    
    void BuildEnhancedControlsScene()
    {
        Debug.Log("Building Enhanced Controls Scene with Split Layout...");
        
        // Create Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create EventSystem
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        
        // Create Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.05f, 0.05f, 0.1f, 1f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Create Main Container
        GameObject containerObj = new GameObject("ScreenContainer");
        containerObj.transform.SetParent(canvasObj.transform, false);
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.05f, 0.05f);
        containerRect.anchorMax = new Vector2(0.95f, 0.95f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;
        
        // Create Title Text (Always visible at top)
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(containerObj.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "TITLE";
        titleText.fontSize = 60;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.font = Resources.Load<TMP_FontAsset>("Fonts/BoldPixels SDF");
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.85f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // Create Left Panel (For Demo)
        GameObject leftPanel = new GameObject("LeftPanel");
        leftPanel.transform.SetParent(containerObj.transform, false);
        RectTransform leftRect = leftPanel.AddComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0, 0.15f);
        leftRect.anchorMax = new Vector2(0.48f, 0.8f);
        leftRect.offsetMin = Vector2.zero;
        leftRect.offsetMax = Vector2.zero;
        
        // Add background to left panel
        Image leftBg = leftPanel.AddComponent<Image>();
        leftBg.color = new Color(0.1f, 0.1f, 0.15f, 0.3f);
        
        // Create Demo Area inside left panel
        GameObject demoArea = new GameObject("DemoArea");
        demoArea.transform.SetParent(leftPanel.transform, false);
        RectTransform demoRect = demoArea.AddComponent<RectTransform>();
        demoRect.anchorMin = new Vector2(0.1f, 0.1f);
        demoRect.anchorMax = new Vector2(0.9f, 0.9f);
        demoRect.offsetMin = Vector2.zero;
        demoRect.offsetMax = Vector2.zero;
        
        // Create Right Panel (For Instructions)
        GameObject rightPanel = new GameObject("RightPanel");
        rightPanel.transform.SetParent(containerObj.transform, false);
        RectTransform rightRect = rightPanel.AddComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(0.52f, 0.15f);
        rightRect.anchorMax = new Vector2(1, 0.8f);
        rightRect.offsetMin = Vector2.zero;
        rightRect.offsetMax = Vector2.zero;
        
        // Add background to right panel
        Image rightBg = rightPanel.AddComponent<Image>();
        rightBg.color = new Color(0.1f, 0.1f, 0.15f, 0.3f);
        
        // Create Demo Instruction Text in right panel
        GameObject demoInstructionObj = new GameObject("DemoInstructionText");
        demoInstructionObj.transform.SetParent(rightPanel.transform, false);
        TextMeshProUGUI demoInstructionText = demoInstructionObj.AddComponent<TextMeshProUGUI>();
        demoInstructionText.text = "Demo instructions appear here";
        demoInstructionText.fontSize = 36;
        demoInstructionText.alignment = TextAlignmentOptions.Center;
        demoInstructionText.color = Color.white;
        demoInstructionText.font = Resources.Load<TMP_FontAsset>("Fonts/BoldPixels SDF");
        RectTransform demoInstructionRect = demoInstructionObj.GetComponent<RectTransform>();
        demoInstructionRect.anchorMin = new Vector2(0.1f, 0.1f);
        demoInstructionRect.anchorMax = new Vector2(0.9f, 0.9f);
        demoInstructionRect.offsetMin = Vector2.zero;
        demoInstructionRect.offsetMax = Vector2.zero;
        
        // Create Content Text (For non-demo screens, centered)
        GameObject contentObj = new GameObject("ContentText");
        contentObj.transform.SetParent(containerObj.transform, false);
        TextMeshProUGUI contentText = contentObj.AddComponent<TextMeshProUGUI>();
        contentText.text = "Content goes here";
        contentText.fontSize = 36;
        contentText.alignment = TextAlignmentOptions.Center;
        contentText.color = Color.white;
        contentText.font = Resources.Load<TMP_FontAsset>("Fonts/BoldPixels SDF");
        RectTransform contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.1f, 0.25f);
        contentRect.anchorMax = new Vector2(0.9f, 0.75f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        // Create Next Button
        GameObject buttonObj = new GameObject("NextButton");
        buttonObj.transform.SetParent(containerObj.transform, false);
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        Button button = buttonObj.AddComponent<Button>();
        
        // Add button enhancer
        ControlsButtonEnhancer enhancer = buttonObj.AddComponent<ControlsButtonEnhancer>();
        enhancer.enableHoverScale = true;
        enhancer.hoverScale = 1.1f;
        enhancer.clickScale = 0.95f;
        enhancer.enableHoverMove = true;
        enhancer.hoverOffsetY = 5f;
        enhancer.enableTextColorChange = true;
        enhancer.hoverTextColor = new Color(1f, 1f, 0.8f);
        
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        colors.selectedColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
        colors.disabledColor = new Color(0.78f, 0.78f, 0.78f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.35f, 0.02f);
        buttonRect.anchorMax = new Vector2(0.65f, 0.12f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        // Create Button Text
        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "NEXT";
        buttonText.fontSize = 36;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        buttonText.font = Resources.Load<TMP_FontAsset>("Fonts/BoldPixels SDF");
        RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        // Create Demo Camera
        GameObject demoCameraObj = new GameObject("DemoCamera");
        Camera demoCamera = demoCameraObj.AddComponent<Camera>();
        demoCamera.clearFlags = CameraClearFlags.SolidColor;
        demoCamera.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
        demoCamera.orthographic = true;
        demoCamera.orthographicSize = 3;
        demoCameraObj.transform.position = new Vector3(-5, 0, -10);
        demoCamera.rect = new Rect(0.05f, 0.2f, 0.43f, 0.5f); // Match left panel position
        
        // Create Main Camera
        GameObject cameraObj = new GameObject("Main Camera");
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.19f, 0.30f, 0.47f, 0);
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cameraObj.transform.position = new Vector3(0, 0, -10);
        cameraObj.tag = "MainCamera";
        cameraObj.AddComponent<AudioListener>();
        
        // Add URP camera component if needed
        System.Type urpCameraType = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");
        if (urpCameraType != null)
        {
            cameraObj.AddComponent(urpCameraType);
            demoCameraObj.AddComponent(urpCameraType);
        }
        
        // Create Demo Spawn Points
        GameObject demoSpawns = new GameObject("DemoSpawnPoints");
        demoSpawns.transform.position = new Vector3(-5, 0, 0);
        
        GameObject playerSpawn = new GameObject("PlayerDemoSpawn");
        playerSpawn.transform.SetParent(demoSpawns.transform);
        playerSpawn.transform.position = new Vector3(-5, 0, 0);
        
        GameObject enemySpawn = new GameObject("EnemyDemoSpawn");
        enemySpawn.transform.SetParent(demoSpawns.transform);
        enemySpawn.transform.position = new Vector3(-2, 0, 0);
        
        // Create Ground for demos
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "DemoGround";
        ground.transform.position = new Vector3(-5, -2, 0);
        ground.transform.localScale = new Vector3(8, 0.5f, 1);
        
        // Set layer if it exists
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1)
        {
            ground.layer = groundLayer;
        }
        else
        {
            Debug.LogWarning("Ground layer not found. Using default layer.");
        }
        
        // Try to set tag, but don't fail if it doesn't exist
        try
        {
            ground.tag = "Ground";
        }
        catch (UnityException e)
        {
            Debug.LogWarning($"Ground tag not defined in Tag Manager. Please add it manually. Error: {e.Message}");
            // Leave as Untagged
        }
        
        // Create 2D Light for demo area
        GameObject lightObj = new GameObject("Demo Light 2D");
        lightObj.transform.position = new Vector3(-5, 0, 0);
        System.Type light2DType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
        if (light2DType != null)
        {
            lightObj.AddComponent(light2DType);
        }
        
        // Add the ControlsTutorialManager (Simple version to avoid compilation issues)
        GameObject managerObj = new GameObject("ControlsTutorialManager");
        ControlsTutorialManagerSimple manager = managerObj.AddComponent<ControlsTutorialManagerSimple>();
        
        // Link references
        manager.screenContainer = containerObj;
        manager.titleText = titleText;
        manager.contentText = contentText;
        manager.nextButton = button;
        manager.buttonText = buttonText;
        manager.demoArea = demoArea;
        manager.leftPanel = leftPanel;
        manager.rightPanel = rightPanel;
        manager.demoInstructionText = demoInstructionText;
        manager.backgroundOverlay = bgImage;
        manager.demoSpawnPoint = playerSpawn.transform;
        manager.enemySpawnPoint = enemySpawn.transform;
        manager.demoCamera = demoCamera;
        
        // Initially hide demo panels
        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
        demoCameraObj.SetActive(false);
        
        Debug.Log("Enhanced Controls Scene built successfully!");
        Debug.Log("Remember to assign the Player prefab to the ControlsTutorialManager!");
    }
    
    void ClearScene()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj != this.gameObject)
            {
                DestroyImmediate(obj);
            }
        }
        Debug.Log("Scene cleared!");
    }
}