using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CopyMainMenuBackground : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Copy MainMenu Background to ControlsScene")]
    public static void CopyBackground()
    {
        // Load the background sprite from the MainMenu
        string spritePath = "Assets/BayatGames/Free Platform Game Assets/Backgrounds/New Background ( Update 1.7 )/png/1920x1080/Background/NewBackground1920x1080.png";
        Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        
        if (backgroundSprite == null)
        {
            Debug.LogError("Could not find background sprite at: " + spritePath);
            return;
        }
        
        // Find the background object in the current scene
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        Image backgroundImage = null;
        
        foreach (GameObject root in rootObjects)
        {
            if (root.name == "Canvas")
            {
                // Navigate through the hierarchy
                Transform screenContainer = root.transform.Find("ScreenContainer");
                if (screenContainer != null)
                {
                    Transform background = screenContainer.Find("Background");
                    if (background != null)
                    {
                        backgroundImage = background.GetComponent<Image>();
                        break;
                    }
                }
            }
        }
        
        if (backgroundImage == null)
        {
            Debug.LogError("Could not find Background Image component in Canvas/ScreenContainer/Background");
            return;
        }
        
        // Apply the sprite and settings
        backgroundImage.sprite = backgroundSprite;
        backgroundImage.type = Image.Type.Simple;
        backgroundImage.preserveAspect = true;
        backgroundImage.color = Color.white;
        
        // Make the background fill the screen
        RectTransform rect = backgroundImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        
        Debug.Log("Successfully copied MainMenu background to ControlsScene!");
        
        // Mark the scene as dirty so changes are saved
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
#endif
}