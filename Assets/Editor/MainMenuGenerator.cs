using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuGenerator : EditorWindow
{
    private GameObject mainMenuCanvas;
    private GameObject background;
    private GameObject titleText;
    private GameObject playButton;
    private GameObject settingsButton;
    private GameObject quitButton;
    private GameObject globalLight2D;
    private GameObject audioManager;

    [MenuItem("Tools/Generate Main Menu Scene")]
    public static void ShowWindow()
    {
        GetWindow<MainMenuGenerator>("Main Menu Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Main Menu Scene Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Check if Unity is in Play mode
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Cannot generate scene while in Play mode. Stop playing first.", MessageType.Warning);
            GUI.enabled = false;
        }

        if (GUILayout.Button("Generate Main Menu Scene"))
        {
            GenerateMainMenuScene();
        }
        
        GUI.enabled = true;

        GUILayout.Space(10);
        GUILayout.Label("This will create a new scene with:", EditorStyles.label);
        GUILayout.Label("• Main Camera (Orthographic)", EditorStyles.label);
        GUILayout.Label("• Global Light 2D", EditorStyles.label);
        GUILayout.Label("• Background Sprite", EditorStyles.label);
        GUILayout.Label("• Canvas with UI elements", EditorStyles.label);
        GUILayout.Label("• Title, Play, Settings, Quit buttons", EditorStyles.label);
        GUILayout.Label("• Audio Manager with sound effects", EditorStyles.label);
        GUILayout.Label("• Background music support", EditorStyles.label);
    }

    private void GenerateMainMenuScene()
    {
        // Safety check for Play mode
        if (Application.isPlaying)
        {
            Debug.LogError("Cannot generate scene while in Play mode. Stop playing first.");
            EditorUtility.DisplayDialog("Error", "Cannot generate scene while in Play mode.\nPlease stop playing and try again.", "OK");
            return;
        }

        try
        {
            // Create new scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Clear default objects except camera
            GameObject[] defaultObjects = newScene.GetRootGameObjects();
            foreach (GameObject obj in defaultObjects)
            {
                if (obj.name != "Main Camera")
                {
                    DestroyImmediate(obj);
                }
            }

            // Setup camera like in Game.unity
            SetupCamera();
            
            // Create Global Light 2D
            CreateGlobalLight2D();
            
            // Create background as world-space sprite (like in Game.unity)
            CreateBackground();
            
            // Create Audio Manager with sound effects
            CreateAudioManager();
            
            // Create UI Canvas and elements with proper world-space camera setup
            CreateUIElements();
            
            // Ensure Scenes folder exists
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            // Save the scene
            string scenePath = "Assets/Scenes/MainMenu.unity";
            bool saved = EditorSceneManager.SaveScene(newScene, scenePath);
            
            if (saved)
            {
                Debug.Log("Main Menu scene generated successfully at: " + scenePath);
                EditorUtility.DisplayDialog("Success", "Main Menu scene generated successfully!\n\nLocation: " + scenePath, "OK");
                
                // Focus the scene view on the new scene
                EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                Debug.LogError("Failed to save Main Menu scene");
                EditorUtility.DisplayDialog("Error", "Failed to save Main Menu scene.\nCheck console for details.", "OK");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error generating Main Menu scene: {e.Message}");
            EditorUtility.DisplayDialog("Error", $"Error generating Main Menu scene:\n{e.Message}", "OK");
        }
    }

    private void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Set camera properties exactly like in Game.unity
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 5f;
            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.backgroundColor = new Color(0.19215687f, 0.3019608f, 0.4745098f, 0f);
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.nearClipPlane = 0.3f;
            mainCamera.farClipPlane = 1000f;
            mainCamera.fieldOfView = 34f;
            mainCamera.depth = -1;
            
            // Add Universal Render Pipeline camera component if available
            try
            {
                var cameraDataType = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");
                if (cameraDataType != null)
                {
                    var cameraData = mainCamera.gameObject.AddComponent(cameraDataType);
                    // Set basic properties using reflection
                    var renderPostProcessingProp = cameraDataType.GetProperty("renderPostProcessing");
                    if (renderPostProcessingProp != null)
                        renderPostProcessingProp.SetValue(cameraData, false);
                }
            }
            catch (System.Exception)
            {
                // URP not available, continue without it
                Debug.Log("URP not detected, continuing with standard camera setup");
            }
        }
    }

    private void CreateGlobalLight2D()
    {
        globalLight2D = new GameObject("Global Light 2D");
        
        // Add Universal 2D Renderer Light component if available
        try
        {
            var light2DType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
            if (light2DType != null)
            {
                var light2D = globalLight2D.AddComponent(light2DType);
                
                // Set properties using reflection to match Game.unity settings
                var lightTypeProp = light2DType.GetProperty("lightType");
                var intensityProp = light2DType.GetProperty("intensity");
                var colorProp = light2DType.GetProperty("color");
                var blendStyleIndexProp = light2DType.GetProperty("blendStyleIndex");
                var falloffIntensityProp = light2DType.GetProperty("falloffIntensity");
                
                if (lightTypeProp != null)
                {
                    var lightTypeEnum = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D+LightType, Unity.RenderPipelines.Universal.Runtime");
                    if (lightTypeEnum != null)
                    {
                        var globalValue = System.Enum.Parse(lightTypeEnum, "Global");
                        lightTypeProp.SetValue(light2D, globalValue);
                    }
                }
                
                intensityProp?.SetValue(light2D, 1f);
                colorProp?.SetValue(light2D, Color.white);
                blendStyleIndexProp?.SetValue(light2D, 0);
                falloffIntensityProp?.SetValue(light2D, 0.5f);
            }
        }
        catch (System.Exception)
        {
            // 2D Lights not available, continue without it
            Debug.Log("2D Lights not detected, continuing without Global Light 2D");
        }
        
        globalLight2D.transform.position = Vector3.zero;
    }

    private void CreateBackground()
    {
        // Create background as a world-space sprite (matching Game.unity structure)
        background = new GameObject("NewBackground1920x1080");
        background.transform.position = Vector3.zero;
        
        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = background.AddComponent<SpriteRenderer>();
        
        // Try to find the same background sprite used in Game.unity
        string[] backgroundGuids = AssetDatabase.FindAssets("NewBackground1920x1080 t:Sprite");
        if (backgroundGuids.Length == 0)
        {
            // Fallback to any background sprite
            backgroundGuids = AssetDatabase.FindAssets("background t:Sprite");
        }
        
        if (backgroundGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(backgroundGuids[0]);
            Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (backgroundSprite != null)
            {
                spriteRenderer.sprite = backgroundSprite;
            }
        }
        
        // Set background properties to match Game.unity exactly
        spriteRenderer.color = Color.white;
        spriteRenderer.sortingOrder = 0;
        spriteRenderer.sortingLayerID = 123339577; // Match the sorting layer from Game.unity
        
        // Match the scale and size from Game.unity (19.2 x 10.8 matches the orthographic camera view)
        background.transform.localScale = Vector3.one;
        
        // The sprite size in Game.unity shows {x: 19.2, y: 10.8} which perfectly matches 
        // the orthographic camera size (5 * 2 = 10 height, 16:9 ratio gives ~17.78 width)
        // This ensures the background fills the entire camera view
    }

    private void CreateAudioManager()
    {
        audioManager = new GameObject("Audio Manager");
        
        // Create background music source
        GameObject bgMusicObj = new GameObject("Background Music");
        bgMusicObj.transform.SetParent(audioManager.transform);
        AudioSource bgMusicSource = bgMusicObj.AddComponent<AudioSource>();
        
        // Configure background music
        bgMusicSource.loop = true;
        bgMusicSource.playOnAwake = true;
        bgMusicSource.volume = 0.3f;
        bgMusicSource.priority = 0; // High priority
        
        // Try to find background music in project
        string[] musicGuids = AssetDatabase.FindAssets("music t:AudioClip");
        if (musicGuids.Length == 0)
        {
            musicGuids = AssetDatabase.FindAssets("background t:AudioClip");
        }
        if (musicGuids.Length == 0)
        {
            musicGuids = AssetDatabase.FindAssets("bgm t:AudioClip");
        }
        
        if (musicGuids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(musicGuids[0]);
            AudioClip bgMusic = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (bgMusic != null)
            {
                bgMusicSource.clip = bgMusic;
                Debug.Log($"Background music assigned: {bgMusic.name}");
            }
        }
        else
        {
            Debug.Log("No background music found. Add audio files to project and regenerate scene.");
        }
        
        // Create UI sound effects source
        GameObject uiSoundsObj = new GameObject("UI Sounds");
        uiSoundsObj.transform.SetParent(audioManager.transform);
        AudioSource uiSoundsSource = uiSoundsObj.AddComponent<AudioSource>();
        
        // Configure UI sounds
        uiSoundsSource.loop = false;
        uiSoundsSource.playOnAwake = false;
        uiSoundsSource.volume = 0.7f;
        uiSoundsSource.priority = 128; // Default priority
        
        // Create ambient sounds source
        GameObject ambientSoundsObj = new GameObject("Ambient Sounds");
        ambientSoundsObj.transform.SetParent(audioManager.transform);
        AudioSource ambientSoundsSource = ambientSoundsObj.AddComponent<AudioSource>();
        
        // Configure ambient sounds
        ambientSoundsSource.loop = true;
        ambientSoundsSource.playOnAwake = false;
        ambientSoundsSource.volume = 0.2f;
        ambientSoundsSource.priority = 64; // Medium priority
        
        // Add the enhanced MainMenuAudioController
        var audioController = audioManager.AddComponent<MainMenuAudioController>();
        audioController.backgroundMusicSource = bgMusicSource;
        audioController.uiSoundsSource = uiSoundsSource;
        audioController.ambientSoundsSource = ambientSoundsSource;
        
        // Try to find and assign sound effects
        AssignSoundEffects(audioController);
    }

    private void CreateUIElements()
    {
        // Create Canvas with World Space mode to align properly with the background
        mainMenuCanvas = new GameObject("Canvas");
        
        Canvas canvas = mainMenuCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10; // Higher than background
        
        // Position canvas in world space to match camera view
        RectTransform canvasRect = mainMenuCanvas.GetComponent<RectTransform>();
        canvasRect.position = new Vector3(0, 0, -1); // Slightly in front of background
        
        // Set canvas size to match the camera's orthographic view
        // Camera orthographic size = 5, so height = 10, width = 10 * (16/9) ≈ 17.78
        float cameraHeight = Camera.main.orthographicSize * 2f; // 10
        float cameraWidth = cameraHeight * Camera.main.aspect; // ~17.78 for 16:9
        
        canvasRect.sizeDelta = new Vector2(cameraWidth, cameraHeight);
        canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f); // Scale down for proper world space sizing
        
        // Add Canvas Scaler for consistent scaling
        CanvasScaler scaler = mainMenuCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = 1f;
        
        // Add Graphic Raycaster
        mainMenuCanvas.AddComponent<GraphicRaycaster>();
        
        // Create Event System if it doesn't exist
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Create UI elements
        CreateTitle();
        CreatePlayButton();
        CreateSettingsButton();
        CreateQuitButton();
        
        // Add MainMenuController and set up button references
        SetupMainMenuController();
    }

    private void CreateTitle()
    {
        titleText = new GameObject("Title Text");
        titleText.transform.SetParent(mainMenuCanvas.transform, false);
        
        // Add TextMeshPro component
        TextMeshProUGUI titleTMP = titleText.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "ZERO CONTACT";
        titleTMP.fontSize = 120; // Larger for world space
        titleTMP.color = Color.white;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.fontStyle = FontStyles.Bold;
        
        // Position at top center, accounting for world space coordinates
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0, 250); // World space units
        titleRect.sizeDelta = new Vector2(1400, 200);
        
        titleTMP.enableWordWrapping = false;
        titleTMP.overflowMode = TextOverflowModes.Overflow;
    }

    private void CreatePlayButton()
    {
        playButton = CreateButton("Play Button", "PLAY", new Vector2(0, 100), new Vector2(400, 80));
    }

    private void CreateSettingsButton()
    {
        settingsButton = CreateButton("Settings Button", "SETTINGS", new Vector2(0, 0), new Vector2(400, 80));
    }

    private void CreateQuitButton()
    {
        quitButton = CreateButton("Quit Button", "QUIT", new Vector2(0, -100), new Vector2(400, 80));
    }

    private GameObject CreateButton(string name, string text, Vector2 position, Vector2 size)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(mainMenuCanvas.transform, false);
        
        // Add Image component for button background
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        
        // Set up button colors
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        button.colors = colors;
        
        // Position button in world space
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = size;
        
        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        // Add TextMeshPro component
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 48; // Larger for world space
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.textWrappingMode = TextWrappingModes.NoWrap;
        buttonText.overflowMode = TextOverflowModes.Overflow;
        
        // Position text to fill button
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonObj;
    }
    
    private void SetupMainMenuController()
    {
        // Add MainMenuController to the canvas
        var menuController = mainMenuCanvas.AddComponent<MainMenuController>();
        
        // Set up button references
        menuController.playButton = playButton.GetComponent<Button>();
        menuController.settingsButton = settingsButton.GetComponent<Button>();
        menuController.quitButton = quitButton.GetComponent<Button>();
        
        // Set scene names
        menuController.gameSceneName = "Game";
        menuController.settingsSceneName = "Settings";
        
        // Connect audio system
        var audioController = audioManager.GetComponent<MainMenuAudioController>();
        if (audioController != null)
        {
            // Add button sound effects
            AddButtonSoundEffects(menuController, audioController);
        }
    }
    
    private void AssignSoundEffects(MainMenuAudioController audioController)
    {
        Debug.Log("=== AUDIO DEBUG: Starting sound effect assignment ===");
        
        // Search for common sound effect names
        string[] buttonSoundNames = { "button", "click", "ui_click", "menu_click", "select" };
        string[] hoverSoundNames = { "hover", "ui_hover", "menu_hover", "beep" };
        string[] ambientSoundNames = { "ambient", "wind", "atmosphere", "environment" };
        
        // Debug: List all audio files in project
        string[] allAudioGuids = AssetDatabase.FindAssets("t:AudioClip");
        Debug.Log($"=== AUDIO DEBUG: Found {allAudioGuids.Length} audio files in project ===");
        foreach (string guid in allAudioGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip != null)
            {
                Debug.Log($"=== AUDIO DEBUG: Audio file found: {clip.name} at {path} ===");
            }
        }
        
        // Try to find button click sound
        Debug.Log("=== AUDIO DEBUG: Searching for button click sounds ===");
        foreach (string soundName in buttonSoundNames)
        {
            string[] guids = AssetDatabase.FindAssets($"{soundName} t:AudioClip");
            Debug.Log($"=== AUDIO DEBUG: Searching for '{soundName}' - Found {guids.Length} matches ===");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip != null)
                {
                    audioController.buttonClickSound = clip;
                    Debug.Log($"=== AUDIO DEBUG: Button click sound assigned: {clip.name} ===");
                    break;
                }
            }
        }
        
        // Try to find hover sound
        Debug.Log("=== AUDIO DEBUG: Searching for hover sounds ===");
        foreach (string soundName in hoverSoundNames)
        {
            string[] guids = AssetDatabase.FindAssets($"{soundName} t:AudioClip");
            Debug.Log($"=== AUDIO DEBUG: Searching for '{soundName}' - Found {guids.Length} matches ===");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip != null)
                {
                    audioController.buttonHoverSound = clip;
                    Debug.Log($"=== AUDIO DEBUG: Button hover sound assigned: {clip.name} ===");
                    break;
                }
            }
        }
        
        // Try to find ambient sound
        Debug.Log("=== AUDIO DEBUG: Searching for ambient sounds ===");
        foreach (string soundName in ambientSoundNames)
        {
            string[] guids = AssetDatabase.FindAssets($"{soundName} t:AudioClip");
            Debug.Log($"=== AUDIO DEBUG: Searching for '{soundName}' - Found {guids.Length} matches ===");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip != null)
                {
                    audioController.ambientSound = clip;
                    Debug.Log($"=== AUDIO DEBUG: Ambient sound assigned: {clip.name} ===");
                    break;
                }
            }
        }
        
        // Final debug report
        if (audioController.buttonClickSound == null)
            Debug.LogWarning("=== AUDIO DEBUG: No button click sound found. Add audio files with names like 'button', 'click', or 'ui_click' ===");
        if (audioController.buttonHoverSound == null)
            Debug.LogWarning("=== AUDIO DEBUG: No button hover sound found. Add audio files with names like 'hover' or 'ui_hover' ===");
        if (audioController.ambientSound == null)
            Debug.LogWarning("=== AUDIO DEBUG: No ambient sound found. Add audio files with names like 'ambient' or 'wind' ===");
            
        Debug.Log("=== AUDIO DEBUG: Sound effect assignment complete ===");
    }
    
    private void AddButtonSoundEffects(MainMenuController menuController, MainMenuAudioController audioController)
    {
        // Add sound effects to each button
        AddButtonAudio(menuController.playButton, audioController, "Play");
        AddButtonAudio(menuController.settingsButton, audioController, "Settings");
        AddButtonAudio(menuController.quitButton, audioController, "Quit");
    }
    
    private void AddButtonAudio(Button button, MainMenuAudioController audioController, string buttonName)
    {
        if (button == null || audioController == null) return;
        
        // Add click sound event
        button.onClick.AddListener(() => audioController.PlayButtonClick());
        
        // Add hover sound using EventTrigger
        var eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // Add hover enter event
        var hoverEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        hoverEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        hoverEnter.callback.AddListener((data) => audioController.PlayButtonHover());
        eventTrigger.triggers.Add(hoverEnter);
        
        Debug.Log($"{buttonName} button audio events added");
    }
}

// Enhanced Audio Controller for Main Menu
public class MainMenuAudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource uiSoundsSource;
    public AudioSource ambientSoundsSource;
    
    [Header("Sound Effects")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    public AudioClip ambientSound;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.3f;
    [Range(0f, 1f)] public float sfxVolume = 0.7f;
    [Range(0f, 1f)] public float ambientVolume = 0.2f;
    
    private void Start()
    {
        Debug.Log("=== AUDIO DEBUG: MainMenuAudioController Start() called ===");
        
        // Debug audio sources
        Debug.Log($"=== AUDIO DEBUG: backgroundMusicSource: {(backgroundMusicSource != null ? "EXISTS" : "NULL")} ===");
        Debug.Log($"=== AUDIO DEBUG: uiSoundsSource: {(uiSoundsSource != null ? "EXISTS" : "NULL")} ===");
        Debug.Log($"=== AUDIO DEBUG: ambientSoundsSource: {(ambientSoundsSource != null ? "EXISTS" : "NULL")} ===");
        
        // Debug audio clips
        Debug.Log($"=== AUDIO DEBUG: buttonClickSound: {(buttonClickSound != null ? buttonClickSound.name : "NULL")} ===");
        Debug.Log($"=== AUDIO DEBUG: buttonHoverSound: {(buttonHoverSound != null ? buttonHoverSound.name : "NULL")} ===");
        Debug.Log($"=== AUDIO DEBUG: ambientSound: {(ambientSound != null ? ambientSound.name : "NULL")} ===");
        
        // Start background music
        if (backgroundMusicSource != null && backgroundMusicSource.clip != null)
        {
            backgroundMusicSource.volume = musicVolume * masterVolume;
            backgroundMusicSource.Play();
            Debug.Log($"=== AUDIO DEBUG: Background music started: {backgroundMusicSource.clip.name} ===");
        }
        else
        {
            Debug.LogWarning("=== AUDIO DEBUG: Background music source or clip is null ===");
        }
        
        // Start ambient sounds
        if (ambientSoundsSource != null && ambientSound != null)
        {
            ambientSoundsSource.clip = ambientSound;
            ambientSoundsSource.volume = ambientVolume * masterVolume;
            ambientSoundsSource.Play();
            Debug.Log($"=== AUDIO DEBUG: Ambient sound started: {ambientSound.name} ===");
        }
        else
        {
            Debug.LogWarning("=== AUDIO DEBUG: Ambient sound source or clip is null ===");
        }
        
        Debug.Log("=== AUDIO DEBUG: MainMenuAudioController Start() complete ===");
    }
    
    public void PlayButtonClick()
    {
        Debug.Log("=== AUDIO DEBUG: PlayButtonClick() called ===");
        if (uiSoundsSource == null)
        {
            Debug.LogError("=== AUDIO DEBUG: uiSoundsSource is null! ===");
            return;
        }
        if (buttonClickSound == null)
        {
            Debug.LogError("=== AUDIO DEBUG: buttonClickSound is null! ===");
            return;
        }
        
        Debug.Log($"=== AUDIO DEBUG: Playing button click sound: {buttonClickSound.name} ===");
        uiSoundsSource.volume = sfxVolume * masterVolume;
        uiSoundsSource.PlayOneShot(buttonClickSound);
        Debug.Log($"=== AUDIO DEBUG: Button click sound played with volume: {uiSoundsSource.volume} ===");
    }
    
    public void PlayButtonHover()
    {
        Debug.Log("=== AUDIO DEBUG: PlayButtonHover() called ===");
        if (uiSoundsSource == null)
        {
            Debug.LogError("=== AUDIO DEBUG: uiSoundsSource is null! ===");
            return;
        }
        if (buttonHoverSound == null)
        {
            Debug.LogError("=== AUDIO DEBUG: buttonHoverSound is null! ===");
            return;
        }
        
        Debug.Log($"=== AUDIO DEBUG: Playing button hover sound: {buttonHoverSound.name} ===");
        uiSoundsSource.volume = (sfxVolume * 0.5f) * masterVolume; // Quieter for hover
        uiSoundsSource.PlayOneShot(buttonHoverSound);
        Debug.Log($"=== AUDIO DEBUG: Button hover sound played with volume: {uiSoundsSource.volume} ===");
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (backgroundMusicSource != null)
            backgroundMusicSource.volume = musicVolume * masterVolume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
    
    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        if (ambientSoundsSource != null)
            ambientSoundsSource.volume = ambientVolume * masterVolume;
    }
    
    private void UpdateAllVolumes()
    {
        if (backgroundMusicSource != null)
            backgroundMusicSource.volume = musicVolume * masterVolume;
        if (ambientSoundsSource != null)
            ambientSoundsSource.volume = ambientVolume * masterVolume;
    }
    
    public void FadeOutMusic(float duration = 1f)
    {
        if (backgroundMusicSource != null)
            StartCoroutine(FadeAudioSource(backgroundMusicSource, 0f, duration));
    }
    
    public void FadeInMusic(float duration = 1f)
    {
        if (backgroundMusicSource != null)
            StartCoroutine(FadeAudioSource(backgroundMusicSource, musicVolume * masterVolume, duration));
    }
    
    private System.Collections.IEnumerator FadeAudioSource(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
            yield return null;
        }
        
        source.volume = targetVolume;
    }
}