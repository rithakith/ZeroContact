using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public Button playButton;
    public Button settingsButton;
    public Button controlsButton;
    public Button quitButton;
    
    [Header("Scene Settings")]
    public string gameSceneName = "SampleScene";
    public string settingsSceneName = "Settings";
    
    [Header("Audio")]
    public AudioSource buttonClickSound;
    public AudioSource backgroundMusic;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    public bool loopMusic = true;
    
    void Awake()
    {
        // Check audio settings
        Debug.Log($"Unity Audio Volume: {AudioListener.volume}");
        Debug.Log($"Unity Audio Paused: {AudioListener.pause}");
        
        // Check if there's an AudioListener in the scene
        if (FindObjectOfType<AudioListener>() == null)
        {
            Debug.LogWarning("No AudioListener found in the scene! Adding one to the Main Camera.");
            
            // Try to find main camera and add AudioListener
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                // Check if it already has one before adding
                if (mainCamera.GetComponent<AudioListener>() == null)
                {
                    mainCamera.gameObject.AddComponent<AudioListener>();
                    Debug.Log("AudioListener added to Main Camera.");
                }
            }
            else
            {
                // If no main camera, add to this GameObject
                if (GetComponent<AudioListener>() == null)
                {
                    gameObject.AddComponent<AudioListener>();
                    Debug.Log("AudioListener added to MainMenuController GameObject.");
                }
            }
        }
    }
    
    void Start()
    {
        // Set up button listeners
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        
        if (controlsButton != null)
            controlsButton.onClick.AddListener(OnControlsButtonClicked);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            
        // Start background music if available
        if (backgroundMusic != null)
        {
            if (backgroundMusic.clip != null)
            {
                backgroundMusic.volume = musicVolume;
                backgroundMusic.loop = loopMusic;
                backgroundMusic.Play();
                Debug.Log($"Playing background music: {backgroundMusic.clip.name}, Volume: {musicVolume}, Loop: {loopMusic}");
            }
            else
            {
                Debug.LogError("Background music AudioSource has no AudioClip assigned!");
            }
        }
        else
        {
            Debug.LogWarning("No background music AudioSource assigned to MainMenuController!");
        }
    }
    
    public void OnPlayButtonClicked()
    {
        PlayButtonSound();
        Debug.Log("Loading Game Scene...");
        
        // Add a small delay for button sound
        Invoke(nameof(LoadGameScene), 0.1f);
    }
    
    public void OnSettingsButtonClicked()
    {
        PlayButtonSound();
        Debug.Log("Opening Settings...");
        
        // For now, just log. In a real game, you might load a settings scene
        // or open a settings panel
        Invoke(nameof(OpenSettings), 0.1f);
    }
    
    public void OnControlsButtonClicked()
    {
        PlayButtonSound();
        Debug.Log("Opening Controls...");
        
        // Add controls screen logic here
        // For now, just show a debug message
        Invoke(nameof(ShowControls), 0.1f);
    }
    
    public void OnQuitButtonClicked()
    {
        PlayButtonSound();
        Debug.Log("Quitting Game...");
        
        Invoke(nameof(QuitGame), 0.1f);
    }
    
    private void LoadGameScene()
    {
        // Check if the Game scene exists
        if (Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning($"Scene '{gameSceneName}' not found in Build Settings!");
            // Fallback: try to load SampleScene directly
            if (Application.CanStreamedLevelBeLoaded("SampleScene"))
            {
                Debug.Log("Loading SampleScene as fallback...");
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                Debug.LogError("SampleScene is also not in Build Settings! Please add it via File > Build Settings.");
            }
        }
    }
    
    private void OpenSettings()
    {
        // For now, just show a debug message
        // In a real implementation, you might:
        // 1. Load a settings scene
        // 2. Open a settings panel
        // 3. Show settings UI
        Debug.Log("Settings functionality not implemented yet.");
    }
    
    private void ShowControls()
    {
        // Load the controls tutorial scene
        if (Application.CanStreamedLevelBeLoaded("ControlsScene"))
        {
            SceneManager.LoadScene("ControlsScene");
        }
        else
        {
            Debug.LogWarning("ControlsScene not found in Build Settings! Please add it.");
            Debug.Log("Controls: Use Arrow Keys/WASD to move, Space to jump, E for shield, 1-3 for shield modes, etc.");
        }
    }
    
    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void PlayButtonSound()
    {
        if (buttonClickSound != null)
        {
            buttonClickSound.Play();
        }
    }
    
    // Optional: Add keyboard shortcuts
    void Update()
    {
        // Enter key to start game
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnPlayButtonClicked();
        }
        
        // Escape key to quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnQuitButtonClicked();
        }
    }
} 