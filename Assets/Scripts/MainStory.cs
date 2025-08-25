using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class MainStory : MonoBehaviour
{
    private PlayableDirector director;
    
    void Awake()
    {
        Debug.Log("MainStory: Awake called");
    }
    
    void Start()
    {
        Debug.Log("MainStory: Start called");
        
        // Find the PlayableDirector in the scene
        director = FindObjectOfType<PlayableDirector>();
        if (director != null)
        {
            Debug.Log($"MainStory: Found PlayableDirector, duration = {director.duration} seconds");
            director.stopped += OnTimelineFinished;
            
            // Check if timeline is playing
            if (director.state == PlayState.Playing)
            {
                Debug.Log("MainStory: Timeline is currently playing");
            }
            else
            {
                Debug.Log($"MainStory: Timeline state is {director.state}");
            }
        }
        else
        {
            Debug.LogWarning("MainStory: No PlayableDirector found! Using fallback timer");
            // Fallback: wait 36 seconds
            Invoke(nameof(LoadNextScene), 36f);
        }
    }
    
    void OnEnable()
    {
        Debug.Log("MainStory: OnEnable called");
    }
    
    void OnTimelineFinished(PlayableDirector pd)
    {
        Debug.Log("MainStory: Timeline finished!");
        LoadNextScene();
    }
    
    void LoadNextScene()
    {
        Debug.Log("MainStory: LoadNextScene called - Loading MenuScreen...");
        
        // Double-check scene name
        Debug.Log($"MainStory: Build scenes count = {SceneManager.sceneCountInBuildSettings}");
        
        try
        {
            SceneManager.LoadScene("MenuScreen", LoadSceneMode.Single);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MainStory: Failed to load scene: {e.Message}");
        }
    }
    
    void OnDestroy()
    {
        Debug.Log("MainStory: OnDestroy called");
        if (director != null)
        {
            director.stopped -= OnTimelineFinished;
        }
    }
}