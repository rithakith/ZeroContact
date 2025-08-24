using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoAddDemoFixer : MonoBehaviour
{
    void Awake()
    {
        // Only run in ControlsScene
        if (SceneManager.GetActiveScene().name.Contains("ControlsScene"))
        {
            // Check if we already have a fixer
            if (FindObjectOfType<ControlsDemoFixer>() == null)
            {
                // Add the fixer to the tutorial manager
                GameObject tutorialManager = GameObject.Find("ControlsTutorialManager");
                if (tutorialManager != null)
                {
                    tutorialManager.AddComponent<ControlsDemoFixer>();
                    Debug.Log("Added ControlsDemoFixer to ControlsTutorialManager");
                    
                    // Add emergency fix - this one will definitely run
                    tutorialManager.AddComponent<EmergencyDemoFix>();
                    Debug.Log("Added EmergencyDemoFix to ControlsTutorialManager");
                    
                    // Add light ray layering fix
                    tutorialManager.AddComponent<FixLightRayLayering>();
                    Debug.Log("Added FixLightRayLayering to ControlsTutorialManager");
                    
                    // Add canvas sorting order fix
                    tutorialManager.AddComponent<FixCanvasSortingOrder>();
                    Debug.Log("Added FixCanvasSortingOrder to ControlsTutorialManager");
                    
                    // Add background hiding fix
                    tutorialManager.AddComponent<HideBackgroundDuringDemo>();
                    Debug.Log("Added HideBackgroundDuringDemo to ControlsTutorialManager");
                }
                else
                {
                    // Create a new GameObject with the fixer
                    GameObject fixerObj = new GameObject("DemoFixer");
                    fixerObj.AddComponent<ControlsDemoFixer>();
                    Debug.Log("Created new GameObject with ControlsDemoFixer");
                }
            }
        }
    }
}

public static class RuntimeDemoFixerInitializer
{
    // This will run automatically when the scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnSceneLoaded()
    {
        if (SceneManager.GetActiveScene().name.Contains("ControlsScene"))
        {
            // Ensure we have the fixer
            if (GameObject.FindObjectOfType<ControlsDemoFixer>() == null)
            {
                GameObject tutorialManager = GameObject.Find("ControlsTutorialManager");
                if (tutorialManager != null)
                {
                    if (tutorialManager.GetComponent<ControlsDemoFixer>() == null)
                    {
                        tutorialManager.AddComponent<ControlsDemoFixer>();
                        Debug.Log("Added ControlsDemoFixer to scene via runtime initialization");
                    }
                    
                    if (tutorialManager.GetComponent<EmergencyDemoFix>() == null)
                    {
                        tutorialManager.AddComponent<EmergencyDemoFix>();
                        Debug.Log("Added EmergencyDemoFix to scene via runtime initialization");
                    }
                }
            }
        }
    }
}