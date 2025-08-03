using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;
    
    [Header("Scene Settings")]
    public string gameSceneName = "Game";
    public string settingsSceneName = "Settings";
    
    [Header("Audio")]
    public AudioSource buttonClickSound;
    public AudioSource backgroundMusic;
    
    void Start()
    {
        // Set up button listeners
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            
        // Start background music if available
        if (backgroundMusic != null)
            backgroundMusic.Play();
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
            // Fallback: try to load by index or show error
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