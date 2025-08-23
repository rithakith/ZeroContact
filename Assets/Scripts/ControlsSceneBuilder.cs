using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class ControlsSceneBuilder : MonoBehaviour
{
    [Header("Build Controls")]
    public bool buildScene = false;
    public bool clearScene = false;
    
    void Update()
    {
        if (buildScene)
        {
            buildScene = false;
            BuildControlsScene();
        }
        
        if (clearScene)
        {
            clearScene = false;
            ClearScene();
        }
    }
    
    void BuildControlsScene()
    {
        Debug.Log("Building Controls Scene...");
        
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
        containerRect.anchorMin = new Vector2(0.1f, 0.1f);
        containerRect.anchorMax = new Vector2(0.9f, 0.9f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;
        
        // Create Title Text
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(containerObj.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "TITLE";
        titleText.fontSize = 60;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.font = Resources.Load<TMP_FontAsset>("Fonts/BoldPixels SDF");
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.7f);
        titleRect.anchorMax = new Vector2(1, 0.9f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // Create Content Text
        GameObject contentObj = new GameObject("ContentText");
        contentObj.transform.SetParent(containerObj.transform, false);
        TextMeshProUGUI contentText = contentObj.AddComponent<TextMeshProUGUI>();
        contentText.text = "Content goes here";
        contentText.fontSize = 36;
        contentText.alignment = TextAlignmentOptions.Center;
        contentText.color = Color.white;
        contentText.font = Resources.Load<TMP_FontAsset>("Fonts/BoldPixels SDF");
        RectTransform contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.1f, 0.3f);
        contentRect.anchorMax = new Vector2(0.9f, 0.65f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        // Create Demo Area
        GameObject demoAreaObj = new GameObject("DemoArea");
        demoAreaObj.transform.SetParent(containerObj.transform, false);
        Image demoAreaImage = demoAreaObj.AddComponent<Image>();
        demoAreaImage.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);
        RectTransform demoRect = demoAreaObj.GetComponent<RectTransform>();
        demoRect.anchorMin = new Vector2(0.25f, 0.35f);
        demoRect.anchorMax = new Vector2(0.75f, 0.6f);
        demoRect.offsetMin = Vector2.zero;
        demoRect.offsetMax = Vector2.zero;
        demoAreaObj.SetActive(false);
        
        // Create Next Button
        GameObject buttonObj = new GameObject("NextButton");
        buttonObj.transform.SetParent(containerObj.transform, false);
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        Button button = buttonObj.AddComponent<Button>();
        
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
        buttonRect.anchorMin = new Vector2(0.35f, 0.05f);
        buttonRect.anchorMax = new Vector2(0.65f, 0.15f);
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
        
        // Create Demo Spawn Points
        GameObject demoSpawns = new GameObject("DemoSpawnPoints");
        demoSpawns.transform.position = Vector3.zero;
        
        GameObject playerSpawn = new GameObject("PlayerDemoSpawn");
        playerSpawn.transform.SetParent(demoSpawns.transform);
        playerSpawn.transform.position = new Vector3(0, 0, 0);
        
        GameObject enemySpawn = new GameObject("EnemyDemoSpawn");
        enemySpawn.transform.SetParent(demoSpawns.transform);
        enemySpawn.transform.position = new Vector3(3, 0, 0);
        
        // Create Ground for demos
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "DemoGround";
        ground.transform.position = new Vector3(0, -2, 0);
        ground.transform.localScale = new Vector3(10, 0.5f, 1);
        
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
        
        // Add the ControlsTutorialManager (Simple version to avoid compilation issues)
        GameObject managerObj = new GameObject("ControlsTutorialManager");
        ControlsTutorialManagerSimple manager = managerObj.AddComponent<ControlsTutorialManagerSimple>();
        
        // Link references
        manager.screenContainer = containerObj;
        manager.titleText = titleText;
        manager.contentText = contentText;
        manager.nextButton = button;
        manager.buttonText = buttonText;
        manager.demoArea = demoAreaObj;
        manager.backgroundOverlay = bgImage;
        manager.demoSpawnPoint = playerSpawn.transform;
        manager.enemySpawnPoint = enemySpawn.transform;
        
        // Create Camera
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
        }
        
        // Create 2D Light
        GameObject lightObj = new GameObject("Global Light 2D");
        System.Type light2DType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
        if (light2DType != null)
        {
            lightObj.AddComponent(light2DType);
        }
        
        Debug.Log("Controls Scene built successfully!");
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