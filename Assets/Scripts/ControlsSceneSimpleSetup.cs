using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ControlsSceneSimpleSetup : MonoBehaviour
{
    [Header("Quick Setup")]
    public bool setupScene = false;
    
    #if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && setupScene)
        {
            setupScene = false;
            SetupControlsScene();
        }
    }
    #endif
    
    void SetupControlsScene()
    {
        // 1. Clean up old stuff
        Debug.Log("Cleaning up old controls scene objects...");
        
        // Remove old controllers
        DestroyObject("TutorialController");
        DestroyObject("SceneController");
        DestroyObject("AutoSetup");
        DestroyObject("ButtonFixer");
        DestroyObject("TutorialGround");
        DestroyObject("DemoPlayer");
        DestroyObject("TutorialCamera");
        
        // 2. Create Canvas if needed
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            Debug.Log("Created Canvas");
        }
        
        // 3. Create EventSystem if needed
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventObj = new GameObject("EventSystem");
            eventObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("Created EventSystem");
        }
        
        // 4. Create Camera if needed
        if (Camera.main == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            Camera cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.2f);
            cam.tag = "MainCamera";
            
            // Add AudioListener (remove any existing ones first)
            AudioListener[] listeners = FindObjectsOfType<AudioListener>();
            foreach (var listener in listeners)
            {
                DestroyImmediate(listener);
            }
            camObj.AddComponent<AudioListener>();
            
            Debug.Log("Created Main Camera");
        }
        
        // 5. Add background layers if you have them
        MenuBackgroundLayers bgLayers = FindObjectOfType<MenuBackgroundLayers>();
        if (bgLayers == null)
        {
            GameObject bgObj = new GameObject("BackgroundController");
            bgLayers = bgObj.AddComponent<MenuBackgroundLayers>();
            Debug.Log("Added MenuBackgroundLayers - add your background sprites to it");
        }
        
        // 6. Create ControlsDisplay
        GameObject controlsObj = new GameObject("ControlsDisplay");
        controlsObj.AddComponent<ControlsDisplay>();
        
        Debug.Log("Controls scene setup complete!");
        Debug.Log("The ControlsDisplay will create all UI when you enter Play mode");
    }
    
    void DestroyObject(string objName)
    {
        GameObject obj = GameObject.Find(objName);
        if (obj != null)
        {
            DestroyImmediate(obj);
            Debug.Log($"Removed {objName}");
        }
    }
}