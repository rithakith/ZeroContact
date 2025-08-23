using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class ResetTutorialContent : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Reset Tutorial Content to Code Values")]
    public static void ResetContent()
    {
        // Find the tutorial manager
        ControlsTutorialManagerEnhanced manager = GameObject.FindObjectOfType<ControlsTutorialManagerEnhanced>();
        
        if (manager == null)
        {
            Debug.LogError("Could not find ControlsTutorialManagerEnhanced in the scene");
            return;
        }
        
        // Force the script to use code values instead of Inspector values
        SerializedObject serializedObject = new SerializedObject(manager);
        SerializedProperty prop = serializedObject.FindProperty("tutorialScreens");
        
        if (prop != null)
        {
            // Clear the Inspector override
            prop.prefabOverride = false;
            serializedObject.ApplyModifiedProperties();
            
            Debug.Log("Reset tutorial content. The code values should now be used.");
            Debug.Log("If you still see old content, try:");
            Debug.Log("1. Right-click on ControlsTutorialManagerEnhanced component in Inspector");
            Debug.Log("2. Select 'Reset'");
            Debug.Log("3. Or manually clear the Tutorial Screens array and let it use defaults");
        }
        
        // Alternative approach - directly modify the values
        if (manager.tutorialScreens != null && manager.tutorialScreens.Length > 2)
        {
            // Update the jump tutorial to use current code content
            manager.tutorialScreens[2].content = "Press SPACE to JUMP\n\nEssential for dodging ground attacks\nand navigating vertical obstacles.";
            Debug.Log("Updated jump tutorial content to use SPACE");
            
            // Force Unity to save the changes
            EditorUtility.SetDirty(manager);
        }
        
        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
    
    [MenuItem("Tools/Show Current Tutorial Content")]
    public static void ShowCurrentContent()
    {
        ControlsTutorialManagerEnhanced manager = GameObject.FindObjectOfType<ControlsTutorialManagerEnhanced>();
        
        if (manager == null)
        {
            Debug.LogError("Could not find ControlsTutorialManagerEnhanced in the scene");
            return;
        }
        
        if (manager.tutorialScreens != null && manager.tutorialScreens.Length > 2)
        {
            Debug.Log("Current Jump Tutorial Content:");
            Debug.Log(manager.tutorialScreens[2].content);
            
            Debug.Log("\nAll tutorial screens:");
            for (int i = 0; i < manager.tutorialScreens.Length; i++)
            {
                Debug.Log($"Screen {i}: {manager.tutorialScreens[i].title}");
                Debug.Log($"Content: {manager.tutorialScreens[i].content}");
                Debug.Log("---");
            }
        }
    }
#endif
}