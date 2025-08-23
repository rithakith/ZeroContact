#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class CreateControlsScene : EditorWindow
{
    [MenuItem("Tools/Create Controls Scene")]
    static void CreateScene()
    {
        // Create new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // Add ControlsSceneBuilder
        GameObject builderObj = new GameObject("SceneBuilder");
        ControlsSceneBuilder builder = builderObj.AddComponent<ControlsSceneBuilder>();
        
        // Trigger the build
        builder.buildScene = true;
        
        // Save the scene
        string scenePath = "Assets/Scenes/ControlsScene.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        
        Debug.Log($"ControlsScene created and saved at: {scenePath}");
        Debug.Log("Please remove the SceneBuilder object after the scene is built.");
    }
}
#endif